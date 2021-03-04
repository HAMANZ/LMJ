using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SubjectAccessor
    {
        public List<Subject> GetList()
        {
            List<Subject> response = new List<Subject>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response=db.Subjects.Where(e => e.isDeleted == false && e.SubId == null).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Subject> GetListSub(long subId)
        {
            List<Subject> response = new List<Subject>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response=db.Subjects.Where(e => e.isDeleted == false && e.SubId == subId).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        #region Add
        public List<Galley> Add(List<Galley> toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {

                    db.Galleys.AddRange(toAdd);
                    db.SaveChanges();

                    return toAdd;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

    }
}
