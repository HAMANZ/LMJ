using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class DiscussionAccessor
    {


        public List<Discussion> GetList(long submissionId, bool isPrereview, bool isReview, bool isCopyEditing,bool isProofReading)
        {
            try
            {
                List<Discussion> data = new List<Discussion>();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Discussions.Where(e => e.SubmissionId == submissionId &&
                    e.isPrereview == isPrereview && e.isReview == isReview && e.isCopyEditing == isCopyEditing && e.isProofReading==isProofReading
                    ).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Discussion Get(long id)
        {
            try
            {
                Discussion data = new Discussion();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Discussions.Where(e => e.Id == id).FirstOrDefault();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Discussion> GetReplies(long id)
        {
            try
            {
                List<Discussion> data = new List<Discussion>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Discussions.Where(e => e.ChannelId == id).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        #region Add
        public Discussion Add(Discussion toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Discussions.Add(toAdd);
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
