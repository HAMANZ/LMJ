using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class Footer
    {
        public long Id { get; set; }
        public string RecentArticleIds { get; set; }
        public string CategoryIds { get; set; }
        public string ContactIds { get; set; }
        public string Issn { get; set; }
        public string Eissn { get; set; }


        public List<long> RecentArticleIdss { get; set; }
        public List<int> CategoryIdss { get; set; }
        public List<string> ContactIdss { get; set; }





    }
}
