﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class About_Page
    {
        public long Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Txt { get; set; }
        public string Img { get; set; }
        public HttpPostedFileBase PostedFileImg { get; set; }

    }
}
