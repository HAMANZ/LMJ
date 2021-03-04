using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class SelectValues
    {
       
        public long Id { get; set; }
        public string Value { get; set; }
        public List<SubSelectValues> SubSelectValues { get; set; }

        public SelectValues(long Id, string Value, List<SubSelectValues> SubSelectValues)
        {
            this.Id = Id;
            this.Value = Value;
            this.SubSelectValues = SubSelectValues;
        }

        public SelectValues()
        {
        }
    }
}
