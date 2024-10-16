using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard_ITCom21.Models
{
    public class AppliQA
    {
        public int id { get; set; }
        public string appointment_code { get; set; }
        public string policies_type { get; set; }
        public string renewal_type { get; set; }
        public string first_policy_year { get; set; }
        public string pure { get; set; }
        public string lead_sourceQA { get; set; }
        public string lead_source { get; set; }
        public string Upsell_plan { get; set; }
        public string rider_upsell { get; set; }
        public int noofcall { get; set; }
        public string from_list { get; set; }
    }
}
