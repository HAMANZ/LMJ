using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.User
{
    public class UserLO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public bool isEditor { get; set; }
        public bool isReviewer { get; set; }
        public bool isCopyEditor { get; set; }
        public bool isProofReader { get; set; }
        public bool isCorresponding { get; set; }
        public bool isSubscriber { get; set; }
    }
}
