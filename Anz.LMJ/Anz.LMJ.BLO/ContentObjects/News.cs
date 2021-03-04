using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class News
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase PostedFileImage { get; set; }
        public string NewsDate { get; set; }
        public string MainDesc { get; set; }
        public string SubDesc { get; set; }

    }
}
