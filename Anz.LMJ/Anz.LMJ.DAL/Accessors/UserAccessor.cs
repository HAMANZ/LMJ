using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class UserAccessor
    {

        public User Get(string email,string password)
        {
            User user = new User();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    user = db.Users.Where(e => e.Email == email && e.Password == password)
                        .FirstOrDefault();
                }
                return user;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public User Get(string email)
        {
            User user = new User();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    user = db.Users.Where(e => e.Email == email ).FirstOrDefault();
                }
                return user;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<User> GetList(List<long> userIds)
        {
            try
            {
                List<User> data = new List<User>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Users.Where(e => userIds.Contains(e.Id)).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<User> GetList()
        {
            try
            {
                List<User> data = new List<User>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Users.Where(e => e.IsDeleted==false).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public User Get(long userId)
        {
            User user = new User();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    user = db.Users.Where(e => e.Id == userId)
                        .FirstOrDefault();
                }
                return user;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public User Add(User toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Users.Add(toAdd);
                    db.SaveChanges();
                }
                return toAdd;
            }
            catch (Exception ex)
            {

                throw;
            }

        }



        public long Edit(User toEdit)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    User usr = db.Users.Where(e => e.Id == toEdit.Id).FirstOrDefault();
                    usr.FirstName = toEdit.FirstName;
                    usr.LastName = toEdit.LastName;
                    usr.Email = toEdit.Email;
                    usr.Username = toEdit.Username;
                    usr.Affiliation = toEdit.Affiliation;
                    usr.ProfilePicture = toEdit.ProfilePicture;
                    usr.IsAdmin = toEdit.IsAdmin;
                    usr.Email2 = toEdit.Email2;
                    usr.Phone1 = toEdit.Phone1;
                    usr.Phone2 = toEdit.Phone2;
                    usr.Mobile1 = toEdit.Mobile1;
                    usr.Mobile2 = toEdit.Mobile2;
                    usr.POB = toEdit.POB;
                    usr.ORCID = toEdit.ORCID;
                    usr.PositionId = (int)toEdit.PositionId;
                    usr.DegreeIds = toEdit.DegreeIds;
                    usr.Desc = toEdit.Desc;
                    usr.Pos = (int)toEdit.Pos;
                    usr.IsDeleted = false;
                    db.SaveChanges();
                    return toEdit.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public long Edit(long userid)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    User usr = db.Users.Where(e => e.Id == userid).FirstOrDefault();
                    usr.IsDeleted = true;
                    db.SaveChanges();
                    return usr.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}
