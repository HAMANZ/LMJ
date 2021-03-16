using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SubmissionKeywordsAccessor
    {
        public List<SubmissionKeyword> GetList(long submissionId)
        {
            try
            {
                List<SubmissionKeyword> data = new List<SubmissionKeyword>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionKeywords.Where(e => e.SubmissionId == submissionId
                     && e.isDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #region Add
        public List<SubmissionKeyword> Add(List<SubmissionKeyword> toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.SubmissionKeywords.AddRange(toAdd);
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


        #region edit
        public List<SubmissionKeyword> Edit(long submissionid)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    List<SubmissionKeyword> keywords = db.SubmissionKeywords.Where(e => e.SubmissionId == submissionid).ToList();
                    keywords.ForEach(a => a.isDeleted = true);
                    db.SaveChanges();
                    return keywords;
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
