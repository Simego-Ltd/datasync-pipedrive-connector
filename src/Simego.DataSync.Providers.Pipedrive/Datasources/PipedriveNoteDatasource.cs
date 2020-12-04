using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveNoteDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/notes?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Note";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/notes/{value}?api_token={APIToken}";
        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper) =>
            PipedriveDataSchema.GetPipedriveDataSchema(helper, Name, $"https://api.pipedrive.com/v1/noteFields?api_token={APIToken}");

        public PipedriveNoteDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}
