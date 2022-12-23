using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networth
{
    public class NetWorthComponent
    {
        [JsonProperty("Category")]
        public String Category { get; set; }
        [JsonProperty("TotalValue")]
        public double TotalValue { get; set; }

    }
}
