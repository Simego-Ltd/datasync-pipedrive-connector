using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveStageDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/stages?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Stage";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/stages/{value}?api_token={APIToken}";

        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper)
        {
            var result = new Dictionary<string, PipedriveDataSchemaItem>();

            result["id"] = new PipedriveDataSchemaItem { Key = "id", Name = "id", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["order_nr"] = new PipedriveDataSchemaItem { Key = "order_nr", Name = "order_nr", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["name"] = new PipedriveDataSchemaItem { Key = "name", Name = "name", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["active_flag"] = new PipedriveDataSchemaItem { Key = "active_flag", Name = "active_flag", Field_Type = PipedriveDataSchemaItemType.FieldBoolean };
            result["deal_probability"] = new PipedriveDataSchemaItem { Key = "deal_probability", Name = "deal_probability", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["pipeline_id"] = new PipedriveDataSchemaItem { Key = "pipeline_id", Name = "pipeline_id", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["rotten_flag"] = new PipedriveDataSchemaItem { Key = "rotten_flag", Name = "rotten_flag", Field_Type = PipedriveDataSchemaItemType.FieldBoolean };
            result["rotten_days"] = new PipedriveDataSchemaItem { Key = "rotten_days", Name = "rotten_days", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["add_time"] = new PipedriveDataSchemaItem { Key = "add_time", Name = "add_time", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["update_time"] = new PipedriveDataSchemaItem { Key = "update_time", Name = "update_time", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["pipeline_name"] = new PipedriveDataSchemaItem { Key = "pipeline_name", Name = "pipeline_name", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["pipeline_deal_probability"] = new PipedriveDataSchemaItem { Key = "pipeline_deal_probability", Name = "pipeline_deal_probability", Field_Type = PipedriveDataSchemaItemType.FieldBoolean };

            return result;
        }


        public PipedriveStageDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
