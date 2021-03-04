using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission.Discussion
{
    public class DiscussionParticipantsLO
    {
        /// <summary>
        /// Id refers to userid
        /// </summary>
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }
}
