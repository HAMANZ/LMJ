using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Anz.LMJ.DAL.Accessors
{
    public class LookUpMediaAccessor
    {
        #region Get

        public LookUpMedia Get(long lookUpId)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    return db.LookUpMedias.Where(e => e.LookUpId == lookUpId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<LookUpMedia> GetList()
        {
            List<LookUpMedia> response = new List<LookUpMedia>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.LookUpMedias.Where(e => e.isDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<LookUpMedia> Get(long lookUpId, bool isVideo)
        {
            List<LookUpMedia> medias = new List<LookUpMedia>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    medias = db.LookUpMedias
                        .Where(e => e.LookUpId == lookUpId && e.isDeleted == false && e.isVideo == isVideo && e.isDeleted == false)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return medias;
        }


        public List<LookUpMedia> Get(List<long> lookUpIds)
        {
            List<LookUpMedia> medias = new List<LookUpMedia>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    medias = db.LookUpMedias
                        .Where(e => lookUpIds.Contains((long)e.LookUpId) && e.isDeleted == false)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return medias;
        }


        #endregion


        #region Add

        public void Add(LookUpMedia newMedia)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.LookUpMedias.Add(newMedia);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #endregion

        #region Update

        #endregion

        #region Delete


        public void Delete(long lookUpId)
        {
            try
            {
                List<LookUpMedia> toDelete = new List<LookUpMedia>();
                using (LMJEntities db = new LMJEntities())
                {
                    toDelete = db.LookUpMedias.Where(e => e.LookUpId == lookUpId).ToList();

                    foreach (LookUpMedia m in toDelete)
                    {
                        db.LookUpMedias.Attach(m);
                        m.isDeleted = true;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public void Delete(List<long> lookUpId)
        {
            try
            {
                List<LookUpMedia> toDelete = new List<LookUpMedia>();
                using (LMJEntities db = new LMJEntities())
                {
                    toDelete = db.LookUpMedias.Where(e => lookUpId.Contains((long)e.LookUpId)).ToList();

                    foreach (LookUpMedia m in toDelete)
                    {
                        db.LookUpMedias.Attach(m);
                        m.isDeleted = true;
                    }
                    db.SaveChanges();
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
