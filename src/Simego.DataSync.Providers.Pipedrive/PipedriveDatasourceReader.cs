using Simego.DataSync.Interfaces;
using Simego.DataSync.Providers.Pipedrive.Datasources;
using Simego.DataSync.Providers.Pipedrive.Interfaces;
using Simego.DataSync.Providers.Pipedrive.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Simego.DataSync.Providers.Pipedrive
{
    [ProviderInfo(Name = "Pipedrive", Group ="Pipedrive", Description = "Pipedrive Datasource")]
    public class PipedriveDatasourceReader : DataReaderProviderBase, IDataSourceSetup, IDataSourceLookup, IDataSourceRegistry
    {
        private HttpWebRequestHelper helper = new HttpWebRequestHelper();
        private ConnectionInterface _connectionIf;
        private IPipedriveDatasourceInfo DataSourceInfo { get; set; } = PipedriveDatasourceFactory.GetDatasourceInfo("person", null);

        [Category("Settings")]
        [Description("Your Pipedrive API Token.")]
        [ProviderCacheSetting(Name = "PipedriveDatasourceReader.APIToken")]
        public string APIToken {
            get { return DataSourceInfo.APIToken; }
            set { DataSourceInfo.APIToken = value; }
        }

        [Category("Settings")]
        [TypeConverter(typeof(PipedriveListTypeConverter))]
        public string List
        {
            get { return DataSourceInfo.Name; }
            set { DataSourceInfo = PipedriveDatasourceFactory.GetDatasourceInfo(value, APIToken); }
        }

        [Category("Settings")]
        [Description("The number or records to return per request.")]
        public int PageSize { get; set; } = 100;

        [Description("Enable HTTP Request Tracing")]
        [Category("Debug")]
        public bool TraceEnabled { get { return helper.TraceEnabled; } set { helper.TraceEnabled = true; } }

        public override DataTableStore GetDataTable(DataTableStore dt)
        {
            //Set the Datasource Identifier.
            dt.AddIdentifierColumn(typeof(int));

            DataSchemaMapping mapping = new DataSchemaMapping(SchemaMap, Side);
            IList<DataSchemaItem> columns = SchemaMap.GetIncludedColumns();
            var schema = DataSourceInfo.GetPipedriveDataSchema(helper);

            int start = 0;
            bool continue_load = false;
            do
            {
                var result = helper.GetRequestAsJson($"{DataSourceInfo.PipedriveEndpointUrl}&start={start}&limit={PageSize}");
              
                //Loop around your data adding it to the DataTableStore dt object.
                foreach (var item_row in result["data"])
                {
                    if (dt.Rows.AddWithIdentifier(mapping, columns,
                        (item, columnName) =>
                        {
                            var pds = schema[columnName];
                            return pds.Parser.ParseValue(item_row[columnName]);
                        }
                        ,item_row["id"].ToObject<int>()) == DataTableStore.ABORT)
                    {
                        break;
                    }
                }
                
                continue_load = result["additional_data"]?["pagination"] != null && result["additional_data"]["pagination"]["more_items_in_collection"].ToObject<bool>();
                if (continue_load)
                {
                    start = result["additional_data"]["pagination"]["next_start"].ToObject<int>();
                }


            } while (continue_load);

            return dt;
        }
        
        public override DataSchema GetDefaultDataSchema()
        {
            var pd_schema = DataSourceInfo.GetPipedriveDataSchema(helper)
                                .Select(p => p.Value)
                                .OrderBy(o => o.Order_nr)
                                .ToList();

            DataSchema schema = new DataSchema();

            pd_schema.ForEach(p => schema.Map.Add(p.ToDataSchemaItem()));
            
            return schema;
        }

        public override List<ProviderParameter> GetInitializationParameters()
        {
            //Return the Provider Settings so we can save the Project File.
            return new List<ProviderParameter>
                       {
                            new ProviderParameter("RegistryKey", RegistryKey),
                            new ProviderParameter("APIToken", SecurityService.EncryptValue(APIToken), GetConfigKey("APIToken")),
                            new ProviderParameter("List", List, GetConfigKey("List")),
                            new ProviderParameter("PageSize", PageSize.ToString(), GetConfigKey("PageSize"))
                       };
        }

        public override void Initialize(List<ProviderParameter> parameters)
        {
            //Load the Provider Settings from the File.
            foreach (ProviderParameter p in parameters)
            {
                AddConfigKey(p.Name, p.ConfigKey);

                switch (p.Name)
                {
                    case "RegistryKey":
                        {
                            RegistryKey = p.Value;
                            break;
                        }
                    case "APIToken":
                        {
                            APIToken = SecurityService.DecyptValue(p.Value);
                            break;
                        }
                    case "List":
                        {
                            List = p.Value;
                            break;
                        }
                    case "PageSize":
                        {
                            if (int.TryParse(p.Value, out int value))
                            {
                                PageSize = value;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            DataSourceInfo = PipedriveDatasourceFactory.GetDatasourceInfo(List, APIToken);
        }

        public override IDataSourceWriter GetWriter()
        {
            //if your provider is read-only return null here.
            return new PipedriveDataSourceWriter { SchemaMap = SchemaMap };
        }

        #region IDataSourceSetup - Render Custom Configuration UI
        
        public void DisplayConfigurationUI(Control parent)
        {
            if (_connectionIf == null)
            {
                _connectionIf = new ConnectionInterface();
                _connectionIf.PropertyGrid.SelectedObject = new ConnectionProperties(this);
            }

            _connectionIf.Font = parent.Font;
            _connectionIf.Size = new Size(parent.Width, parent.Height);
            _connectionIf.Location = new Point(0, 0);
            _connectionIf.Dock = System.Windows.Forms.DockStyle.Fill;

            parent.Controls.Add(_connectionIf);
        }

        public bool Validate()
        {
            try
            {
                if (string.IsNullOrEmpty(APIToken))
                {
                    throw new ArgumentException("You must specify a valid APIToken.");
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Pipedrive", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;

        }

        public IDataSourceReader GetReader()
        {
            return this;
        }

        #endregion

        #region ConnectionRegistry
        [Category("Connection.Library")]
        [Description("Key Name of the Item in the Connection Library")]
        [DisplayName("Key")]
        public string RegistryKey { get; set; }

        public void InitializeFromRegistry(IDataSourceRegistryProvider provider)
        {
            var registry = provider.Get(RegistryKey);
            if (registry == null) return;

            foreach (var p in registry.Parameters)
            {
                switch (p.Name)
                {
                    case "APIToken":
                        {
                            APIToken = SecurityService.DecyptValue(p.Value);
                            break;
                        }                    
                }
            }
        }

        public List<ProviderParameter> GetRegistryInitializationParameters()
        {
            return new List<ProviderParameter>
                       {
                            new ProviderParameter("APIToken", SecurityService.EncryptValue(APIToken), GetConfigKey("APIToken"))
                       };
        }

        public IDataSourceReader ConnectFromRegistry(IDataSourceRegistryProvider provider)
        {
            InitializeFromRegistry(provider);
            return this;
        }

        public object GetRegistryInterface()
        {
            if (string.IsNullOrEmpty(RegistryKey))
            {
                return this;
            }

            return new PipedriveDatasourceReaderWithRegistry(this);
        }
        #endregion

        public DataTableStore GetLookupTable(DataLookupSource source, List<string> columns)
        {
            var reader = new PipedriveDatasourceReader
            {
                APIToken = source.Config.ContainsKey("APIToken") ? source.Config["APIToken"] : APIToken,                
                PageSize = source.Config.ContainsKey("PageSize") ? DataSchemaTypeConverter.ConvertTo<int>(source.Config["PageSize"]) : PageSize,
                List = source.Config.ContainsKey("List") ? source.Config["List"] : source.Name
            };

            reader.Initialize(SecurityService);

            var defaultSchema = reader.GetDefaultDataSchema();
            reader.SchemaMap = new DataSchema();

            foreach (var dsi in defaultSchema.Map)
            {
                foreach (var column in columns)
                {
                    if (dsi.ColumnName.Equals(column, StringComparison.OrdinalIgnoreCase))
                        reader.SchemaMap.Map.Add(dsi.Copy());
                }
            }

            return reader.GetDataTable();
        }

        public HttpWebRequestHelper GetWebRequestHelper()
        {
            var requestHelper = helper.Copy();            
            return requestHelper;
        }

        public IPipedriveDatasourceInfo GetDatasourceInfo()
        {
            return DataSourceInfo;
        }

        public IList<string> GetObjectNames()
        {
            return new List<string>() { "Activity", "Person", "Deal", "Organization", "User", "Pipeline", "Product", "Note", "Stage" }.OrderBy(p => p).ToList();
        }
    }

    public class PipedriveDatasourceReaderWithRegistry : DataReaderRegistryView<PipedriveDatasourceReader>
    {
        [Category("Connection.Library")]
        [Description("Key Name of the Item in the Connection Library")]
        [DisplayName("Key")]
        public string RegistryKey
        {
            get => _reader.RegistryKey;
            set => _reader.RegistryKey = value;
        }

        [Browsable(false)]
        public string APIToken
        {
            get => _reader.APIToken;
            set => _reader.APIToken = value;
        }

        [Category("Settings")]
        [TypeConverter(typeof(PipedriveListTypeConverter))]
        public string List
        {
            get => _reader.List;
            set => _reader.List = value;
        }

        [Category("Settings")]
        [Description("The number or records to return per request.")]
        public int PageSize
        {
            get => _reader.PageSize;
            set => _reader.PageSize = value;
        }

        [Description("Enable HTTP Request Tracing")]
        [Category("Debug")]
        public bool TraceEnabled
        {
            get => _reader.TraceEnabled;
            set => _reader.TraceEnabled = value;
        }
        
        public PipedriveDatasourceReaderWithRegistry(PipedriveDatasourceReader reader) : base(reader)
        {

        }

        public IList<string> GetObjectNames()
        {
            return _reader.GetObjectNames();
        }
    }
}
