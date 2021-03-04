using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class LookUpMultiLanguageAccessor
    {
        #region Get


        /// <summary>
        /// Gets all lookups even the unpublished
        /// </summary>
        /// <param name="lookUpId"></param>
        /// <param name="langId"></param>
        /// <returns></returns>
        public List<LookUpMultiLanguage> GetAll(long lookUpId, long langId)
        {
            List<LookUpMultiLanguage> multis = new List<LookUpMultiLanguage>();

            try
            {

                using (LMJEntities db = new LMJEntities())
                {
                    multis = db.LookUpMultiLanguages
                        .Where(e => e.LookUpId == lookUpId && e.LangId == langId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return multis;
        }
        /// <summary>
        /// gets published looks only
        /// </summary>
        /// <param name="lookUpId"></param>
        /// <param name="langId"></param>
        /// <returns></returns>
        public LookUpMultiLanguage Get(long lookUpId, long? langId)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    return db.LookUpMultiLanguages
                        .Where(e => e.LookUpId == lookUpId && e.LangId == langId && e.isDeleted == false)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public LookUpMultiLanguage Get(List<long> lookupIds, long langId, string description)
        {
            try
            {
                LookUpMultiLanguage result = new LookUpMultiLanguage();

                using (LMJEntities db = new LMJEntities())
                {
                    result = db.LookUpMultiLanguages.Where(e => lookupIds.Contains((long)e.LookUpId) && e.isDeleted == true && e.LangId == langId && e.Description == description).FirstOrDefault();
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<LookUpMultiLanguage> GetAll(List<long> lookUpId, long langId)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    return db.LookUpMultiLanguages
                        .Where(e => lookUpId.Contains((long)e.LookUpId) && e.LangId == langId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }




        #endregion


        #region Add
        public LookUpMultiLanguage Add(LookUpMultiLanguage lookupMulti)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.LookUpMultiLanguages.Add(lookupMulti);
                    db.SaveChanges();
                }
                return lookupMulti;
            }
            catch (Exception ex)

            {

                throw;
            }
        }


        public LookUpMultiLanguage Add(string description, long lookupId, int langId, long userId)
        {
            try
            {
                LookUpMultiLanguage add = new LookUpMultiLanguage();
                add.LookUpId = lookupId;
                add.LangId = langId;
                add.SysDate = DateTime.Now;
                add.Description = description;
                add.isDeleted = false;

                add = Add(add);

                return add;

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #endregion

        #region Update

        public void EditDesription(long lookupmultiId, string newDescription)
        {
            try
            {
                LookUpMultiLanguage ml = new LookUpMultiLanguage();
                using (LMJEntities db = new LMJEntities())
                {
                    ml = db.LookUpMultiLanguages.Where(e => e.Id == lookupmultiId).FirstOrDefault();

                    db.LookUpMultiLanguages.Attach(ml);
                    ml.Description = newDescription;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void UpdateDescription(LookUpMultiLanguage old, string newDescription)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.LookUpMultiLanguages.Attach(old);

                    old.Description = newDescription;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #endregion


        #region Delete

        public void Delete(long lookupId)
        {
            try
            {
                LookUpMultiLanguage current = new LookUpMultiLanguage();
                using (LMJEntities db = new LMJEntities())
                {
                    current = db.LookUpMultiLanguages.Where(e => e.LookUpId == lookupId && e.isDeleted == true).FirstOrDefault();
                    if (current != null)
                    {
                        db.LookUpMultiLanguages.Attach(current);

                        current.isDeleted = false;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void DeleteList(List<long> lookupIds)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    List<LookUpMultiLanguage> toDelete = new List<LookUpMultiLanguage>();
                    toDelete = db.LookUpMultiLanguages.Where(e => lookupIds.Contains((long)e.LookUpId) == true).ToList();
                    foreach (LookUpMultiLanguage dl in toDelete)
                    {
                        db.LookUpMultiLanguages.Attach(dl);
                        dl.isDeleted = false;

                        db.SaveChanges();
                    }

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
