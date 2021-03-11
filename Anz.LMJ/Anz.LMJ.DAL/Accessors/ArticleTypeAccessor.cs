using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class ArticleTypeAccessor
    {
        public List<ArticleType> GetList()
        {
            List<ArticleType> response = new List<ArticleType>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response=db.ArticleTypes.Where(e => e.isDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<ArticleType> GetList(List<long> Ids)
        {
            List<ArticleType> response = new List<ArticleType>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.ArticleTypes.Where(e => e.isDeleted == false & Ids.Contains(e.Id)).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ArticleType Get(long articleid)
        {
            ArticleType response = new ArticleType();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.ArticleTypes.Where(e => e.Id == articleid && e.isDeleted == false).FirstOrDefault(); ;
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
