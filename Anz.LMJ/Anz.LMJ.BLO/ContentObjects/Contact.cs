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
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Address { get; set; }
        public string POBox { get; set; }
        public string Email { get; set; }
        public string AdditionalEmail { get; set; }
        public string Facebook { get; set; }
        public string Twiter { get; set; }
        public string Desc { get; set; }
        public override string ToString()
        {
            return this.Phone1 + ", " + this.Phone2 + ", " + this.Mobile1 + ", " + this.Mobile2 + ", " +
            this.Address + ", " + this.POBox + ", " + this.Email + ", " + this.AdditionalEmail + ", " + this.Facebook + ", " +
            this.Twiter + ", " + this.Desc;
        }
    }
}
