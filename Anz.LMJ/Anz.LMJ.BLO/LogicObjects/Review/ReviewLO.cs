using Anz.LMJ.BLO.LogicObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Review
{
    public class ReviewLO
    {
        public long Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public int NbOfStars { get; set; }
        public string Text { get; set; }
        public long SubmissionId { get; set; }

        public bool IsAdmit { get; set; }
        public DateTime Date { get; set; }
    }
}
