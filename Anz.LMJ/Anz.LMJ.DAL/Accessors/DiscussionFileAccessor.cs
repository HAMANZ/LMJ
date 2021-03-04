using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class DiscussionFileAccessor
    {
        public List<DiscussionsFile> GetList(long discussionId)
        {
            try
            {
                List<DiscussionsFile> data = new List<DiscussionsFile>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.DiscussionsFiles.Where(e => e.DiscussionId == discussionId && e.isDeleted == false)
                        .ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #region Add
        public List<DiscussionsFile> Add(List<DiscussionsFile> toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.DiscussionsFiles.AddRange(toAdd);
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
