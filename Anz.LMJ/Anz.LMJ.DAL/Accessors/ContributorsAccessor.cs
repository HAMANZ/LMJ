using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class ContributorsAccessor
    {
        public List<Contributor> GetList(long submissionId)
        {
            try
            {
                List<Contributor> data = new List<Contributor>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Contributors.Where(e => e.SubmissionId == submissionId
                     && e.isDeleted == false ).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Contributor> GetList(long submissionId, bool IsTags)
        {
            try
            {
                List<Contributor> data = new List<Contributor>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Contributors.Where(e => e.SubmissionId == submissionId
                     && e.isDeleted == false && e.IsTag == IsTags).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<Contributor> Add(List<Contributor> toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {

                    db.Contributors.AddRange(toAdd);
                    db.SaveChanges();

                    return toAdd;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }





        public void Delete(long submissionid,bool isTag)
        {
            try
            {
                Contributor current = new Contributor();

                using (LMJEntities db = new LMJEntities())
                {
                    current = db.Contributors.Where(e => e.SubmissionId == submissionid && e.isDeleted == false && e.IsTag== isTag).FirstOrDefault();
                    if (current != null)
                    {
                        db.Contributors.Attach(current);

                        current.isDeleted = true;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #region edit
        public List<Contributor> Edit(long submissionid)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    List<Contributor> contributors = db.Contributors.Where(e => e.SubmissionId == submissionid).ToList();
                    contributors.ForEach(a => a.isDeleted = true);
                    db.SaveChanges();
                    return contributors;
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
