using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class SubSelectValues
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public List<SubSubSelectValues> SubSubSelectValues { get; set; }
        public SubSelectValues(long Id, string Value, List<SubSubSelectValues> SubSubSelectValues)
        {
            this.Id = Id;
            this.Value = Value;
            this.SubSubSelectValues = SubSubSelectValues;
        }
        public SubSelectValues() { }
    }
}
