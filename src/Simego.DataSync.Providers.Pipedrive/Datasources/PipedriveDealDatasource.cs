using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveDealDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/deals?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Deal";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/deals/{value}?api_token={APIToken}";

        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper) => 
            PipedriveDataSchema.GetPipedriveDataSchema(helper, Name, $"https://api.pipedrive.com/v1/dealFields?api_token={APIToken}");

        public PipedriveDealDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
