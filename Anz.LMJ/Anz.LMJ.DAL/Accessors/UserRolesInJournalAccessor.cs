using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class UserRolesInJournalAccessor
    {
        public UserRolesInJournal Get(long userId,long roleId,List<long> sectionIds)
        {
            UserRolesInJournal response = new UserRolesInJournal();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.UserRolesInJournals.Where(e => e.isDeleted == false &&
                    e.UserId == userId && sectionIds.Contains((long)e.SectionId)  && e.RoleId == roleId)
                    .FirstOrDefault();
                }

                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public UserRolesInJournal Get(long userId, long roleId)
        {
            UserRolesInJournal response = new UserRolesInJournal();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.UserRolesInJournals.Where(e => e.isDeleted == false &&
                    e.UserId == userId && e.RoleId == roleId)
                    .FirstOrDefault();
                }

                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public List<UserRolesInJournal> GetListUserRole(long roleId)
        {
            try
            {
                List<UserRolesInJournal> data = new List<UserRolesInJournal>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRolesInJournals.Where(e => e.RoleId == roleId && e.isDeleted == false)
                        .ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public UserRolesInJournal Get(long userId)
        {
            try
            {
                UserRolesInJournal data = new UserRolesInJournal();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRolesInJournals.Where(e => e.UserId == userId && e.isDeleted == false).FirstOrDefault();
                }

                return data;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserRolesInJournal> GetList(long userId)
        {
            try
            {
                List<UserRolesInJournal> data = new List<UserRolesInJournal>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRolesInJournals.Where(e => e.UserId == userId && e.isDeleted == false)
                        .ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserRolesInJournal> GetList(long userId,List<int> rolesid)
        {
            try
            {
                List<UserRolesInJournal> data = new List<UserRolesInJournal>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRolesInJournals.Where(e => e.UserId == userId && rolesid.Contains((int)e.RoleId) && e.isDeleted == false)
                        .ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserRolesInJournal> Add(List<UserRolesInJournal> toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.UserRolesInJournals.AddRange(toAdd);
                    db.SaveChanges();
                    return toAdd;
                }
                return toAdd;
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public List<UserRolesInJournal> Edit(long userid)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    List<UserRolesInJournal> rolesjournal = db.UserRolesInJournals.Where(e => e.UserId == userid).ToList();
                    rolesjournal.ForEach(a => a.isDeleted = true);
                    db.SaveChanges();
                    return rolesjournal;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
