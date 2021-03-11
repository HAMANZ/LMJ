using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class UserRoleAccessor
    {

        public UserRole Get(string name)
        {
            UserRole role = new UserRole();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    role = db.UserRoles.Where(e => e.Role == name && e.isDeleted == false).FirstOrDefault();
                }

                return role;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserRole> GetList(List<int> Ids)
        {
            try
            {
                List<UserRole> data = new List<UserRole>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRoles.Where(e => Ids.Contains(e.Id) == true 
                    && e.isDeleted == false).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserRole> GetList()
        {
            try
            {
                List<UserRole> data = new List<UserRole>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRoles.Where(e=>e.isDeleted == false).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<UserRole> GetList(List<string> names)
        {
            try
            {
                List<UserRole> data = new List<UserRole>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserRoles.Where(e => names.Contains(e.Role) == true).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public UserRole Add(UserRole toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.UserRoles.Add(toAdd);
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
