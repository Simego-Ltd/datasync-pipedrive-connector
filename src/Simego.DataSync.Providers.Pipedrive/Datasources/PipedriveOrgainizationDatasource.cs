using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveOrgainizationDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/organizations?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Organization";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/organizations/{value}?api_token={APIToken}";

        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper) =>
            PipedriveDataSchema.GetPipedriveDataSchema(helper, Name, $"https://api.pipedrive.com/v1/organizationFields?api_token={APIToken}");

        public PipedriveOrgainizationDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
