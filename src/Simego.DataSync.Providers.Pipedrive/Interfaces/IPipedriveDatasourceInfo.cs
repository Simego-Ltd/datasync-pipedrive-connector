using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Interfaces
{
    public interface IPipedriveDatasourceInfo
    {
        IDictionary<string, PipedriveDataSchemaItem> GetPipedriveDataSchema(HttpWebRequestHelper helper);
        string PipedriveEndpointUrl { get; }
        string GetPipedriveItemEndpointUrl(int value);
        string APIToken { get; set;  }
        string Name { get; }
    }
}
