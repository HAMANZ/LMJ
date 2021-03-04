using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class RequirmentAccessor
    {
        public List<Requirment> GetList()
        {
            List<Requirment> response = new List<Requirment>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response=db.Requirments.Where(e => e.isDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
   

    }
}
