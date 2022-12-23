using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networth
{
    public class RealEstate
    {
        [JsonProperty("EstateID")]
        public String EstateID { get; set; }
        [JsonProperty("EstateName")]
        public String EstateName { get; set; }
        [JsonProperty("City")]
        public String City { get; set; }
        [JsonProperty("Type")]
        public String Type { get; set; }
        [JsonProperty("Size")]
        public double Size { get; set; }
        [JsonProperty("OpenDate")]
        public DateTime OpenDate { get; set; }
        [JsonProperty("OpenPrice")]
        public double OpenPrice { get; set; }
        [JsonProperty("CloseDate")]
        public DateTime CloseDate { get; set; }
        [JsonProperty("ClosePrice")]
        public double ClosePrice { get; set; }
        [JsonProperty("IsClosed")]
        public bool IsClosed { get; set; }
    }
}
