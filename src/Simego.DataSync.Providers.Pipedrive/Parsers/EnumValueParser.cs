using Newtonsoft.Json.Linq;
using Simego.DataSync.Providers.Pipedrive.Interfaces;
using System;
using System.Collections.Generic;

namespace Simego.DataSync.Providers.Pipedrive.Parsers
{
    public class EnumValueParser : IPipedriveValueParser
    {
        public IDictionary<string, string> Options { get; set; } = new Dictionary<string, string>();

        public object ConvertValue(object value)
        {
            var val = value as string;
            foreach (var v in Options)
            {
                if (string.Equals(v.Value, val, StringComparison.OrdinalIgnoreCase))
                {
                    //If the Key value can be parsed to an Integer then send int value rather than string value.
                    if (int.TryParse(v.Key, out int intVal))
                    {
                        return intVal;
                    }
                    else
                    {
                        return v.Key;
                    }                    
                }
            }
            return null;
        }

        public object ParseValue(JToken token)
        {
            var val = token?.ToObject<object>();
            if (val != null)
            {
                var option = DataSchemaTypeConverter.ConvertTo<string>(val);

                if (Options.TryGetValue(option, out string value))
                {
                    return value;
                }
                
                // Pipedrive seems to return true/false as an enum except the values are 0/1 in the option list and true/false in the data.
                if (string.Equals("true", DataSchemaTypeConverter.ConvertTo<string>(val), StringComparison.OrdinalIgnoreCase))
                    option = 1.ToString();

                if (string.Equals("false", DataSchemaTypeConverter.ConvertTo<string>(val), StringComparison.OrdinalIgnoreCase))
                    option = 0.ToString();

                if (Options.TryGetValue(option, out string boolValue))
                {
                    return boolValue;
                }                
            }
            return null;
        }        
    }
}