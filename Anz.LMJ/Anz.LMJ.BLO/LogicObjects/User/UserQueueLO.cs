using Anz.LMJ.BLO.LogicObjects.Submission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.User
{
    public class UserQueueLO
    {
        public bool isAuthor { get; set; }
        public bool isEditor { get; set; }
        public bool isReviewer { get; set; }
        public bool isCopyEditor { get; set; }
        public bool isProofReader { get; set; }

        public List<SubmissionLO> asAuthor { get; set; }
        public List<SubmissionLO> asEditor { get; set; }
        public List<SubmissionLO> asReviewer { get; set; }
        public List<SubmissionLO> asCopyEditor { get; set; }
        public List<SubmissionLO> asProofReader { get; set; }
    }
}
