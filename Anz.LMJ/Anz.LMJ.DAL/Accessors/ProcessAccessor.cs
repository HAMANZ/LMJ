using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class ProcessAccessor
    {
        public List<Process> GetList(long roleId)
        {
            try
            {
                List<Process> processes = new List<Process>();

                using(LMJEntities db = new LMJEntities())
                {
                    processes = db.Processes.Where(e => e.RoleId == roleId).ToList();
                }
                return processes;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public Process Get(string code)
        {
            try
            {
                Process data = new Process();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Processes.Where(e => e.Code == code && e.isDeleted == false).FirstOrDefault();

                    return data;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public Process Get(long id)
        {
            try
            {
                Process data = new Process();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Processes.Where(e => e.Id == id && e.isDeleted == false).FirstOrDefault();

                    return data;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
