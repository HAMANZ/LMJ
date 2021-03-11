using Anz.LMJ.BLO.ContentObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class SelectLO
    {
        public List<SelectValues> ArticleType { get; set; }
        public List<SelectValues> Researches { get; set; }
        public List<SelectValues> Subjects { get; set; }
        public List<SelectValues> Questions { get; set; }
        public List<SelectValues> Requirments { get; set; }
        public List<Options> Authors { get; set; }
        public List<Options> Volumes { get; set; }
        public List<Options> Issues { get; set; }
        public List<Options> Category { get; set; }
        public List<Options> IssuesDate { get; set; }


    }
}
