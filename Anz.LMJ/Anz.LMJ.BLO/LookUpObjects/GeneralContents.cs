using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LookUpObjects
{
    public class GeneralContents<T>
    {

        public List<T> Contents { get; set; }
        public int TotalContent { get; set; }

    }
}
