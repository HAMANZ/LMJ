using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class ReviewAccessor
    {


        public List<Review> GetList(long submissionId)
        {
            try
            {
                List<Review> data = new List<Review>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Reviews.Where(e => e.SubmissionId == submissionId
                     && e.IsDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Dictionary<int,int> getmax(long submissionid) {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    Dictionary<int, int> n = new Dictionary<int, int>();
                    int max = (int)db.Reviews.Select(x => x.NbOfStars).DefaultIfEmpty(0).Max();
                    var res = db.Reviews.GroupBy(x => x.NbOfStars).Select(g => new { stars = g.Key, nbstars = g.Count()});
                    foreach (var g in res)
                    {
                        n = new Dictionary<int, int>();
                        n.Add((int)g.stars, g.nbstars);
                    }

                    return n;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
}
        public List<Review> GetList()
        {
            try
            {
                List<Review> data = new List<Review>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Reviews.ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Review Add(Review toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {

                    db.Reviews.Add(toAdd);
                    db.SaveChanges();

                    return toAdd;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }


    }
}
