using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class Hero_Banner
    {
        public long Id { get; set; }
        public List<string> BackgroundImage { get; set; }
        public List<HttpPostedFileBase> PostedFileBackgroundImage { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }
}
