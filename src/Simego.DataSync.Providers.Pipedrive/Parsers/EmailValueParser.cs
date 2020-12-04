using Newtonsoft.Json.Linq;
using Simego.DataSync.Providers.Pipedrive.Interfaces;

namespace Simego.DataSync.Providers.Pipedrive.Parsers
{
    public class EmailValueParser : IPipedriveValueParser
    {
        public object ConvertValue(object value)
        {
            return value;
        }

        public object ParseValue(JToken token)
        {
            var array = token as JArray;
            if (array != null)
            {
                foreach (var val in array)
                {
                    if (val["primary"].ToObject<bool>())
                    {
                        var value = val["value"]?.ToObject<string>();
                        return string.IsNullOrWhiteSpace(value) ? null : value;
                    }
                }
            }
            return null;
        }
    }
}
