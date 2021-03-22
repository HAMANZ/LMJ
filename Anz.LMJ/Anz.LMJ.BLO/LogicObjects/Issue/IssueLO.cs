using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.LogicObjects.Issue
{
    public class IssueLO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public long IssueNo { get; set; }
        public string IssuePrintNo { get; set; }
        public DateTime Date { get; set; }
        public string CoverImage { get; set; }

        public HttpPostedFileBase Photo { get; set; }
    }
}
