using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class ProcessStagesAccessor
    {
        public List<ProcessStage> GetList(long processId,long roleId)
        {
            try
            {
                List<ProcessStage> data = new List<ProcessStage>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.ProcessStages.Where(e => e.ProcessId == processId 
                    && e.RoleId == roleId && e.isDeleted == false).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
