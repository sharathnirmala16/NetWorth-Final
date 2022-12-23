using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networth
{
    public class FinancialAsset
    {
        [JsonProperty("AssetID")]
        public String AssetID { get; set; }
        [JsonProperty("Category")]
        public String Category { get; set; }
        [JsonProperty("Name")]
        public String Name { get; set; }
        [JsonProperty("Shares")]
        public int Shares { get; set; }
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
