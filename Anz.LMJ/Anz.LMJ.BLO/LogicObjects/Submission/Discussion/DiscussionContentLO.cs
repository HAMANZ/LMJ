using Anz.LMJ.BLO.LogicObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission.Discussion
{
    public class DiscussionContentLO
    {
        public long? Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long ChannelId { get; set; }
        public DateTime SentDate { get; set; }
        public List<long> Participantids { get; set; }
        public List<DiscussionFileLO> Files { get; set; }
        public UserLO Sender { get; set; }
        public bool isPreReview { get; set; }
        public bool isClosed { get; set; }
        public bool isReview { get; set; }
        public bool isCopyEditingReview { get; set; }
        public bool isProofReadingReview { get; set; }

    }
}
