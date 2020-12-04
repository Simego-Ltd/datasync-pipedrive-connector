using Newtonsoft.Json.Linq;
using Simego.DataSync.Providers.Pipedrive.Interfaces;

namespace Simego.DataSync.Providers.Pipedrive.Parsers
{
    public class UserValueParser : IPipedriveValueParser
    {
        public object ConvertValue(object value)
        {
            return value;
        }

        public object ParseValue(JToken token)
        {
            var fld = token as JObject;
            if (fld != null)
            {
                return fld["id"].ToObject<int>();
            }
            return token?.ToObject<object>();
        }
    }
}
