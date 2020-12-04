using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveActivityDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/activities?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Activity";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/activities/{value}?api_token={APIToken}";
        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper) =>
            PipedriveDataSchema.GetPipedriveDataSchema(helper, Name, $"https://api.pipedrive.com/v1/activityFields?api_token={APIToken}");

        public PipedriveActivityDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}

