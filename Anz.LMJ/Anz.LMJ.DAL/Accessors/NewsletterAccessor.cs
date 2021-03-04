using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class NewsletterAccessor
    {
        public List<Newsletter> GetList()
        {
            List<Newsletter> response = new List<Newsletter>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Newsletters.Where(e => e.isDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public Newsletter Get(long issueid)
        {
            Newsletter response = new Newsletter();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Newsletters.Where(e => e.Id == issueid && e.isDeleted == false).FirstOrDefault(); ;
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Add
        public  Newsletter Add(Newsletter toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Newsletters.Add(toAdd);
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



        #region Edit   
        public long Edit(Newsletter toEdit)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    Newsletter newsletter = db.Newsletters.Where(e => e.Id == toEdit.Id).FirstOrDefault();
                    
                    newsletter.UserId = toEdit.UserId;
                    newsletter.Name = toEdit.Name;
                    newsletter.ISSN = toEdit.ISSN;
                    newsletter.EISSN = toEdit.EISSN;
                    newsletter.CoverImage = toEdit.CoverImage;
                    newsletter.Volume = toEdit.Volume;
                    newsletter.PublishDate = toEdit.PublishDate;
                    newsletter.isDeleted = toEdit.isDeleted;
                    newsletter.SysDate = toEdit.SysDate;
                    db.SaveChanges();
                    return toEdit.Id;
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
