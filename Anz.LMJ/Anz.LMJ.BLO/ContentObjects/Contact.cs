using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class Contact
    {
        public long Id { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string AdditionalEmail { get; set; }
        public string POBOX { get; set; }
        public string Facebook { get; set; }
        public string Twiter { get; set; }
        public string Desc { get; set; }

    }
}
