using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class DiscussionParticipantAccessor
    {
        public List<DiscussionParticipant> GetList(long discussionId)
        {
            try
            {
                List<DiscussionParticipant> data = new List<DiscussionParticipant>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.DiscussionParticipants.Where(e => e.DiscussionId == discussionId
                    ).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #region Add
        public List<DiscussionParticipant> Add(List<DiscussionParticipant> toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.DiscussionParticipants.AddRange(toAdd);
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
