using Anz.LMJ.BLO.LogicObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission.Discussion
{
    public class DiscussionLO
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime DiscussionDate { get; set; }

        public UserLO MainSender { get; set; }
        public List<DiscussionFileLO> Files { get; set; }
        public int NumberOfReplies { get; set; }
        public UserLO LastSender { get; set; }
        public bool isClosed { get; set; }

        public List<DiscussionParticipantsLO> Participants { get; set; }

        public List<DiscussionContentLO> Discussions { get; set; }


    }
}
