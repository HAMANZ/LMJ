using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SubmissionInProcessAccessor
    {
        #region Get
        public List<SubmissionInProcess> GetList(List<long> Id, List<long> processId)
        {
            try
            {
                List<SubmissionInProcess> data = new List<SubmissionInProcess>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionInProcesses.Where(e =>
                    Id.Contains(e.Id) && processId.Contains((long)e.ProcessId)).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<SubmissionInProcess> GetList(long submissionId)
        {
            try
            {
                List<SubmissionInProcess> data = new List<SubmissionInProcess>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionInProcesses.Where(e =>e.SubmissionId == submissionId).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<SubmissionInProcess> GetList(long submissionId, List<long> processId)
        {
            try
            {
                List<SubmissionInProcess> data = new List<SubmissionInProcess>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionInProcesses.Where(e =>
                    e.SubmissionId == submissionId && processId.Contains((long)e.ProcessId)).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public SubmissionInProcess GetLast(long submissionId)
        {
            try
            {
                SubmissionInProcess data = new SubmissionInProcess();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionInProcesses.Where(e => e.SubmissionId == submissionId)
                        .OrderByDescending(o => o.Id).FirstOrDefault();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public SubmissionInProcess Get(long processId)
        {
            try
            {
                SubmissionInProcess data = new SubmissionInProcess();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionInProcesses.Where(e => e.ProcessId == processId).FirstOrDefault();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion


        #region Add
        public SubmissionInProcess Add(SubmissionInProcess toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.SubmissionInProcesses.Add(toAdd);
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
