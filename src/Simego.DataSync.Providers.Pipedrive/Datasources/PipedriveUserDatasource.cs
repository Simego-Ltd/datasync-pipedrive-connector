using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveUserDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/users?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "User";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/users/{value}?api_token={APIToken}";

        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper)
        {
            var result = new Dictionary<string, PipedriveDataSchemaItem>();

            result["id"] = new PipedriveDataSchemaItem { Key = "id", Name = "id", Field_Type = PipedriveDataSchemaItemType.FieldInteger };
            result["name"] = new PipedriveDataSchemaItem { Key = "name", Name = "name", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["email"] = new PipedriveDataSchemaItem { Key = "email", Name = "email", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["locale"] = new PipedriveDataSchemaItem { Key = "locale", Name = "locale", Field_Type = PipedriveDataSchemaItemType.FieldString };
            result["created"] = new PipedriveDataSchemaItem { Key = "created", Name = "created", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["modified"] = new PipedriveDataSchemaItem { Key = "modified", Name = "modified", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["last_login"] = new PipedriveDataSchemaItem { Key = "last_login", Name = "last_login", Field_Type = PipedriveDataSchemaItemType.FieldDate };
            result["role_id"] = new PipedriveDataSchemaItem { Key = "role_id", Name = "role_id", Field_Type = PipedriveDataSchemaItemType.FieldInteger };

            return result;
        }
            

        public PipedriveUserDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
