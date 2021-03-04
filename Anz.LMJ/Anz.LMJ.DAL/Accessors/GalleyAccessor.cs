using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class GalleyAccessor
    {
        public List<Galley> GetList(long submissionid)
        {
            List<Galley> response = new List<Galley>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Galleys.Where(e => e.SubmissionId == submissionid).ToList();
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
