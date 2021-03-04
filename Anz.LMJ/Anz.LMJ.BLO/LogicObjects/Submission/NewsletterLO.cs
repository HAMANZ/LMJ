using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class NewsletterLO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        public string Number { get; set; }
        public string Issn { get; set; }
        public string Eissn { get; set; }
        public string Volume { get; set; }
        public DateTime PublishDate { get; set; }
        public SubmissionFilesLO CoverImage { get; set; }
        

    }
   
}
