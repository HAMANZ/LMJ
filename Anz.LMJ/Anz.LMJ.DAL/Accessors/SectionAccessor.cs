using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SpecialitiesAccessor
    {
        public List<Speciality> GetList()
        {
            List<Speciality> response = new List<Speciality>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Specialities.Where(e => e.IsDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Speciality Get(long id)
        {
            Speciality response = new Speciality();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Specialities.Where(e => e.Id == id && e.IsDeleted == false).FirstOrDefault(); ;
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
