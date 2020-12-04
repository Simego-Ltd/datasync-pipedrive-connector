using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedrivePersonDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/persons?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Person";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/persons/{value}?api_token={APIToken}";
        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper) => 
            PipedriveDataSchema.GetPipedriveDataSchema(helper, Name, $"https://api.pipedrive.com/v1/personFields?api_token={APIToken}");

        public PipedrivePersonDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
