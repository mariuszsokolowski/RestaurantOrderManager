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
    public enum RatingType
    {
        Menu = 1,
        Waiter = 2

    }
}
