using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class ProcessLO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool isIncludeSkip { get; set; }
        public string StageName { get; set; }
        public string ButtonValue { get; set; }
        public string ButtonBackground { get; set; }
        public bool isPreReview { get; set; }
        public bool isPreCopyediting { get; set; }
        public bool isPreProduction { get; set; }
        public bool isModalRequired { get; set; }
        public long? ProcessIdinModal { get; set; }
        public string ModalName { get; set; }
        public string ModalAction { get; set; }
        public List<SubmissionFilesLO> ModalFiles { get; set; }
    }
}
