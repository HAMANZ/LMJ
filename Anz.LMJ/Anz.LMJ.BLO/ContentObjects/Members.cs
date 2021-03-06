﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.ContentObjects
{
    public class Members
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase PostedFileImage { get; set; }
        public string Position { get; set; }
        public string MainDesc { get; set; }
        public string SubDesc { get; set; }
        public string Pos { get; set; }
        public long RoleId { get; set; }
        public long PositionId { get; set; }
        public string IsMember { get; set; }
        public string IsTeam { get; set; }

    }
}
