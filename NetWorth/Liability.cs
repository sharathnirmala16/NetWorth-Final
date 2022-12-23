using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networth
{
    public class Liability
    {
        [JsonProperty("LoanID")] 
        public string LoanID { get; set; }
        [JsonProperty("LoanName")]
        public string LoanName { get; set; }
        [JsonProperty("PrincipleAmount")]
        public double PrincipleAmount { get; set; }
        [JsonProperty("Interest")]
        public double Interest { get; set; }

        [JsonProperty("OpenDate")]
        public DateTime OpenDate { get; set; }
        [JsonProperty("AmountRemaining")]
        public double AmountRemaining { get; set; }
        [JsonProperty("CloseDate")]
        public DateTime CloseDate { get; set; }
    }
}
