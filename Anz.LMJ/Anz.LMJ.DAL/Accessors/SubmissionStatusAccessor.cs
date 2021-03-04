using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SubmissionStatusAccessor
    {
        public List<SubmissionStatu> GetList(long userId,bool isArchived, bool isSkiped)
        {
            try
            {
                List<SubmissionStatu> data = new List<SubmissionStatu>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionStatus.Where(e => e.UserId == userId
                    && e.isArchived ==isArchived && e.isSkip == isSkiped && e.isDeleted== false ).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        #region Add
        public SubmissionStatu Add(SubmissionStatu toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.SubmissionStatus.Add(toAdd);
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
