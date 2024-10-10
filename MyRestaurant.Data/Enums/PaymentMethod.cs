using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Enums
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentMethod
    {
        Cash=1,
        Card=2,
        Blik=3
    }
}
