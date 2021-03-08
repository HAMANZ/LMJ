using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Anz.LMJ.BLO.ContentObjects
{
    public class Events
    {
        public long Id { get; set; }
        public string MainImg { get; set; }
        public List<string> SubImg { get; set; }
        public List<HttpPostedFileBase> PostedFile { get; set; }
        public HttpPostedFileBase PostedFileMainImg { get; set; }
        public string MainTitle { get; set; }
        public string EventDate { get; set; }
        public string MainDesc { get; set; }
        public string Location { get; set; }
        


    }
}
