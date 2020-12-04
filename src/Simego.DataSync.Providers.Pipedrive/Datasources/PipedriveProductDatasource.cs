using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Datasources
{
    public class PipedriveProductDatasource : IPipedriveDatasourceInfo
    {
        public string PipedriveEndpointUrl => $"https://api.pipedrive.com/v1/products?api_token={APIToken}";

        public string APIToken { get; set; }

        public string Name => "Product";

        public string GetPipedriveItemEndpointUrl(int value) => $"https://api.pipedrive.com/v1/products/{value}?api_token={APIToken}";
        public IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper) =>
            PipedriveDataSchema.GetPipedriveDataSchema(helper, Name, $"https://api.pipedrive.com/v1/productFields?api_token={APIToken}");

        public PipedriveProductDatasource(string apiToken)
        {
            APIToken = apiToken;
        }
    }
}