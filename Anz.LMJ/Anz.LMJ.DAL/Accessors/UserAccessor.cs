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
                    data = db.Users.ToList();
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

    }
}
