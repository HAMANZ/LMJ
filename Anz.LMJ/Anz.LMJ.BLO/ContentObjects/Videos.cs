using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class Videos
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase PostedFileImage { get; set; }
        public string MainDesc { get; set; }
        public string SubDesc { get; set; }
        public string Pos { get; set; }
        public string Src { get; set; }
        public string VideoDate { get; set; }
        
        public HttpPostedFileBase PostedFileVideo { get; set; }
    }
}
