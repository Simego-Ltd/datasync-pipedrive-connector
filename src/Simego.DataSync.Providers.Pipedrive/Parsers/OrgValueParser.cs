using Newtonsoft.Json.Linq;
using Simego.DataSync.Providers.Pipedrive.Interfaces;

namespace Simego.DataSync.Providers.Pipedrive.Parsers
{
    public class OrgValueParser : IPipedriveValueParser
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
                return fld["value"].ToObject<int>();
            }
            return token?.ToObject<object>();
        }
    }
}

