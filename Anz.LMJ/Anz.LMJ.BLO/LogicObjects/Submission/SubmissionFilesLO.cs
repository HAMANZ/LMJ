using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class SubmissionFilesLO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TypeId { get; set; }
        public string Caption { get; set; }
        public string TypeName { get; set; }
        public HttpPostedFileBase[] FilesToUpload { get; set; }
        public string[] Captions { get; set; }
        public long[] TypesId { get; set; }
        public long submissionid { get; set; }
        public string attr { get; set; }

    }
}
