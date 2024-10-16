using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard_ITCom21.Models
{
    class AppliAdmin
    {
        public int id { get; set; }
        public string appointment_code { get; set; }
        public string contract_code { get; set; }
        public DateTime ?from_date { get; set; }
        public DateTime ?to_date { get; set; }
        public DateTime ?appointment_date { get; set; }
        public DateTime ?date_of_payment { get; set; }
        public string payments { get; set; }
        public string rounder { get; set; }
        public string pending_reason { get; set; }
        public string note_private { get; set; }
        public string note_admin { get; set; }
        public string proposer_address { get; set; }
        public string Insured_address { get; set; }
    }
}
