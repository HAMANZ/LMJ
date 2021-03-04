using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class LookUpAccessor
    {

        #region Get

        public LookUp Get(long parentId, string code, bool isdiffer)
        {
            try
            {
                LookUp result = new LookUp();

                using (LMJEntities db = new LMJEntities())
                {
                    result = db.LookUps
                        .Where(e => e.ParentId == parentId && e.IsDeleted == false && e.Code == code)
                        .FirstOrDefault();

                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public LookUp Get(string code, long parentId, bool isDiffer)
        {
            try
            {
                LookUp lookup = new LookUp();
                try
                {
                    using (LMJEntities db = new LMJEntities())
                    {
                        lookup = db.LookUps
                            .Where(e => e.Code == code && e.ParentId == parentId && e.IsDeleted == false)
                            .FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                return lookup;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public LookUp Get(long tableId, long lookupId)
        {
            try
            {
                LookUp result = new LookUp();

                using (LMJEntities db = new LMJEntities())
                {

                    result = db.LookUps.Where(e => e.Id == lookupId && e.TableId == tableId && e.IsDeleted == false).FirstOrDefault();

                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        /// <summary>
        /// get last main lookup of table
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public LookUp Get(long tableId, string toDiffer)
        {
            try
            {
                LookUp result = new LookUp();

                using (LMJEntities db = new LMJEntities())
                {

                    result = db.LookUps.Where(e => e.ParentId == null && e.TableId == 49 && e.IsDeleted == false).OrderByDescending(s => s.Id).FirstOrDefault();

                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        /// <summary>
        /// get lookups for a specific table id for pagination
        /// </summary>
        /// <returns></returns>
        public List<LookUp> GetList(long tableId, int offset, int limit)
        {
            List<LookUp> lookups = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookups = db.LookUps
                        .Where(e => e.TableId == tableId)
                        .OrderByDescending(o => o.Id)
                        .Skip(offset)
                        .Take(limit)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookups;

        }

        public List<LookUp> GetList(long parentId, bool NoNeedOnlyToDifferentiate)
        {
            List<LookUp> lookups = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookups = db.LookUps
                        .Where(e => e.ParentId == parentId && e.IsDeleted == false)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookups;

        }
        public List<LookUp> GetList(long parentId, bool NoNeedOnlyToDifferentiate, long tableId)
        {
            List<LookUp> lookups = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookups = db.LookUps
                        .Where(e => e.ParentId == parentId && e.IsDeleted == false && tableId == tableId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookups;

        }


        public List<LookUp> GetList(List<string> codes, long tableid)
        {
            List<LookUp> lookups = new List<LookUp>();

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookups = db.LookUps.Where(e => codes.Contains(e.Code) && e.TableId == tableid && e.IsDeleted == false).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookups;
        }


        /// <summary>
        /// Returns Titles, Descriptions, Images, and Videos
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="columnCode"></param>
        /// <returns></returns>
        public List<LookUp> GetMono(long tableId, string columnCode)
        {
            List<LookUp> looks = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    looks = db.LookUps
                        .Where(e => e.TableId == tableId && e.Code != columnCode && e.ParentId == null && e.IsDeleted == false)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return looks;
        }
        /// <summary>
        /// Gets list of lookups from table using code
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="columnCode"></param>
        /// <returns></returns>
        public List<LookUp> GetList(long tableId, string code)
        {
            List<LookUp> looks = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    looks = db.LookUps
                        .Where(e => e.TableId == tableId && e.Code == code && e.IsDeleted == false)
                        .OrderByDescending(o => o.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return looks;
        }


        /// <summary>
        /// Gets lookup using code and tableid
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public LookUp Get(string code, long tableId)
        {
            LookUp lookup = new LookUp();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookup = db.LookUps
                        .Where(e => e.Code == code && e.TableId == tableId && e.IsDeleted == false)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookup;
        }


        /// <summary>
        /// get lookups for a specific table id 
        /// </summary>
        /// <returns></returns>
        public List<LookUp> GetList(long tableId)
        {
            List<LookUp> lookups = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookups = db.LookUps
                        .Where(e => e.TableId == tableId && e.IsDeleted == false)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookups;

        }

        /// <summary>
        /// get lookup for a specified row id
        /// </summary>
        /// <param name="lookupId"></param>
        /// <returns></returns>
        public LookUp Get(long lookupId)
        {
            LookUp lookup = new LookUp();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookup = db.LookUps
                        .Where(e => e.Id == lookupId)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookup;
        }

        public List<LookUp> GetChildren(long lookUpId)
        {
            List<LookUp> lookups = new List<LookUp>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lookups = db.LookUps
                        .Where(e => e.ParentId == lookUpId && e.IsDeleted == false)
                        .ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return lookups;
        }




        #endregion


        #region Add


        public LookUp Add(LookUp lookup)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.LookUps.Add(lookup);
                    db.SaveChanges();
                }
                return lookup;
            }
            catch (Exception ex)
            {

                throw;
            }


        }


        public LookUp Add(string code, int tableId, long userId)
        {
            try
            {
                LookUp add = new LookUp();

                add.Code = code;
                add.TableId = tableId;
                add.UserId = userId;
                add.SysDate = DateTime.Now;
                add.IsDeleted = false;
                add.ParentId = null;
                add.isPublished = true;

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

        public LookUp UpdateCode(LookUp toUpdate, string newCode)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.LookUps.Attach(toUpdate);
                    toUpdate.Code = newCode;

                    db.SaveChanges();
                }
                return toUpdate;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion


        #region Delete



        public void Delete(string code, long tableId)
        {
            try
            {
                LookUp current = new LookUp();

                using (LMJEntities db = new LMJEntities())
                {
                    current = db.LookUps.Where(e => e.Code == code && e.TableId == tableId && e.IsDeleted == false).FirstOrDefault();
                    if (current != null)
                    {
                        db.LookUps.Attach(current);

                        current.IsDeleted = true;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }







        public void Delete(long lkId)
        {
            try
            {
                LookUp current = new LookUp();

                using (LMJEntities db = new LMJEntities())
                {
                    current = db.LookUps.Where(e => e.Id == lkId && e.IsDeleted == false).FirstOrDefault();
                    if (current != null)
                    {
                        db.LookUps.Attach(current);

                        current.IsDeleted = true;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<LookUp> DeleteContent(bool withParent, long parentId)
        {
            try
            {
                List<LookUp> result = new List<LookUp>();

                using (LMJEntities db = new LMJEntities())
                {

                    if (withParent)
                    {
                        result = db.LookUps.Where(e => (e.Id == parentId || e.ParentId == parentId)).ToList();
                    }
                    else
                    {
                        result = db.LookUps.Where(e => (e.Id == parentId || e.ParentId == parentId)).ToList();
                    }

                    foreach (LookUp l in result)
                    {
                        db.LookUps.Attach(l);
                        l.IsDeleted = true;


                    }

                    db.SaveChanges();
                }




                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        #endregion







    }
}
