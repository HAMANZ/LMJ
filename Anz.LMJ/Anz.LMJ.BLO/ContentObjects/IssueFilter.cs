using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Anz.LMJ.BLO.ContentObjects
{
    public class IssueFilter
    {
        public long Id { get; set; }
        public string CurrentFromDate { get; set; }
        public string CurrentToDate { get; set; }
        public string RecentFromDate { get; set; }
        public string RecentToDate { get; set; }
        public string ArchiveFromDate { get; set; }
        public string ArchiveToDate { get; set; }
        public string IndexFromDate { get; set; }
        public string IndexToDate { get; set; }
      }
}
