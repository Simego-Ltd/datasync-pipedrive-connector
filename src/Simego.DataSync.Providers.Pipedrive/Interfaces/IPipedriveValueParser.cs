using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simego.DataSync.Providers.Pipedrive.Interfaces
{
    public interface IPipedriveValueParser
    {
        object ParseValue(JToken token);
        object ConvertValue(object value);
    }
}
