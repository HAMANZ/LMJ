using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class UserResponsibleInProcessAccessor
    {


        public UserResponsibleInProcess Get(long userId)
        {
            try
            {
                UserResponsibleInProcess data = new UserResponsibleInProcess();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserResponsibleInProcesses.Where(e => e.UserId == userId && e.isDeleted == false).FirstOrDefault();
                }

                return data;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserResponsibleInProcess> GetList(long userId)
        {
            try
            {
                List<UserResponsibleInProcess> data = new List<UserResponsibleInProcess>();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserResponsibleInProcesses.Where(e => e.UserId == userId && e.isDeleted == false).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<UserResponsibleInProcess> GetList()
        {
            try
            {
                List<UserResponsibleInProcess> data = new List<UserResponsibleInProcess>();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserResponsibleInProcesses.Where(e=>e.isDeleted == false).ToList();
                }

                return data;

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<UserResponsibleInProcess> GetList(List<long> SubmissionProcessId)
        {
            try
            {
                List<UserResponsibleInProcess> data = new List<UserResponsibleInProcess>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.UserResponsibleInProcesses.Where(e => SubmissionProcessId.Contains((long)e.SubmissionProcessId) == true
                    && e.isDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Add
        public UserResponsibleInProcess Add(UserResponsibleInProcess toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.UserResponsibleInProcesses.Add(toAdd);
                    db.SaveChanges();
                }
                return toAdd;
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public List<UserResponsibleInProcess> Add(List<UserResponsibleInProcess> toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.UserResponsibleInProcesses.AddRange(toAdd);
                    db.SaveChanges();
                }
                return toAdd;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion
    }
}
