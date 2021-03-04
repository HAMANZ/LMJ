using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{
    public class FileTypeLogic
    {

        #region Type Helper
        public string GetFileType(long typeId)
        {
            try
            {
                return "static type";
            }
            catch (Exception ex)
            {

                throw;
            }
        }

       #endregion


    }
}
