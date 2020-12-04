using Simego.DataSync.Providers.Pipedrive.TypeConverters;
using System.Collections.Generic;
using System.ComponentModel;

namespace Simego.DataSync.Providers.Pipedrive
{
    class ConnectionProperties
    {
        private readonly PipedriveDatasourceReader _reader;
        
        [Category("Settings")]
        [Description("Your Pipedrive API Token.")]
        public string APIToken { get { return _reader.APIToken; } set { _reader.APIToken = value; } }

        [Category("Settings")]
        [TypeConverter(typeof(PipedriveListTypeConverter))]
        public string List
        {
            get { return _reader.List; }
            set { _reader.List = value; }
        }

        public ConnectionProperties(PipedriveDatasourceReader reader)
        {
            _reader = reader;
        }

        public IList<string> GetObjectNames()
        {
            return _reader.GetObjectNames();
        }
    }
}
