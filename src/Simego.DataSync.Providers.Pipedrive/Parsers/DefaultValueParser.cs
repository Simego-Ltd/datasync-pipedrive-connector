using Newtonsoft.Json.Linq;
using Simego.DataSync.Providers.Pipedrive.Interfaces;

namespace Simego.DataSync.Providers.Pipedrive.Parsers
{
    public class DefaultValueParser : IPipedriveValueParser
    {        
        public object ConvertValue(object value)
        {
            return value;
        }

        public object ParseValue(JToken token)
        {
            return token?.ToObject<object>();
        }
    }
}
