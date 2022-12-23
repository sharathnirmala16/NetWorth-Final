using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networth
{
    public class Transaction
    {
        [JsonProperty("TransactionID")]
        public String TransactionID { get; set; }
        [JsonProperty("Credit")]
        public String Credit { get; set; }
        [JsonProperty("Amount")]
        public double Amount { get; set; }
        [JsonProperty("Purpose")]
        public string Purpose { get; set; }
        [JsonProperty("Date")]
        public DateTime Date { get; set; }
    }
}
