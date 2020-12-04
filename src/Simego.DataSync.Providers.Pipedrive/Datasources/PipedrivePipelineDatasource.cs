using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedrivePipelineDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/pipelines?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Pipeline";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/pipelines/{value}?api_token={APIToken}";

        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper)
        {
            var result = new Dictionary<string, PipedriveDataSchemaItem>();

            result["id"] = new PipedriveDataSchemaItem { Key = "id", Name = "id", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["name"] = new PipedriveDataSchemaItem { Key = "name", Name = "name", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["url_title"] = new PipedriveDataSchemaItem { Key = "url_title", Name = "url_title", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["order_nr"] = new PipedriveDataSchemaItem { Key = "order_nr", Name = "order_nr", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["active"] = new PipedriveDataSchemaItem { Key = "active", Name = "active", Field_Type = PipedriveDataSchemaItemType.FieldBoolean };
            result["deal_probability"] = new PipedriveDataSchemaItem { Key = "deal_probability", Name = "deal_probability", Field_Type = PipedriveDataSchemaItemType.FieldBoolean };
            result["add_time"] = new PipedriveDataSchemaItem { Key = "add_time", Name = "add_time", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["update_time"] = new PipedriveDataSchemaItem { Key = "update_time", Name = "update_time", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["selected"] = new PipedriveDataSchemaItem { Key = "selected", Name = "selected", Field_Type = PipedriveDataSchemaItemType.FieldBoolean };

            return result;
        }


        public PipedrivePipelineDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
