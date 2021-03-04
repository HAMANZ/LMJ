using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SubmissionFilesAccessor
    {
        public List<SubmissionFile> GetList(long submissionId,bool isSubmission,bool isRevision,bool isCopyEdited)
        {
            try
            {
                List<SubmissionFile> data = new List<SubmissionFile>();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                    && e.isRevision == isRevision && e.isSubmission == isSubmission && e.isCopyedited == isCopyEdited
                    && e.isDeleted == false).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public SubmissionFile Get(long submissionId, bool isSubmission, bool isRevision, bool isCopyEdited)
        {
            try
            {
                SubmissionFile data = new SubmissionFile();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                    && e.isRevision == isRevision && e.isSubmission == isSubmission && e.isCopyedited == isCopyEdited
                    && e.isDeleted == false).FirstOrDefault();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<SubmissionFile> GetAcceptanceFiles(long submissionId,bool isAcceptedforReview,bool isAcceptedforCopyEditing,bool isAcceptedforProduction)
        {
            try
            {
                List<SubmissionFile> data = new List<SubmissionFile>();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                    && e.isAcceptedforReview == isAcceptedforReview && e.isAcceptedforCopyEditing == isAcceptedforCopyEditing && e.isAcceptedforProduction == isAcceptedforProduction
                    && e.isDeleted == false).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<SubmissionFile> GetListNullable(long submissionId, bool? isSubmission, bool? isRevision, bool? isCopyEdited)
        {
            try
            {
                List<SubmissionFile> data = new List<SubmissionFile>();

                using (LMJEntities db = new LMJEntities())
                {
                    if(isSubmission == true)
                    {
                        data = db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                        &&  e.isSubmission == isSubmission 
                        && e.isDeleted == false).ToList();
                    }

                    if (isRevision == true)
                    {
                        data.AddRange( db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                        && e.isRevision == isRevision
                        && e.isDeleted == false).ToList());
                    }

                    if (isCopyEdited == true)
                    {
                        data.AddRange( db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                        && e.isCopyedited == isCopyEdited
                        && e.isDeleted == false).ToList());
                    }
                }
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<SubmissionFile> GetList(bool isAcceptedForReview , bool isAcceptedforCopyEditing, bool isAcceptedforProduction, long submissionId)
        {
            try
            {
                List<SubmissionFile> data = new List<SubmissionFile>();

                using (LMJEntities db = new LMJEntities())
                {
                    data = db.SubmissionFiles.Where(e => e.SubmissionId == submissionId
                    && e.isAcceptedforCopyEditing == isAcceptedforCopyEditing && e.isAcceptedforReview == isAcceptedForReview && e.isAcceptedforProduction == isAcceptedforProduction
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
        public List<SubmissionFile> Add(List<SubmissionFile> toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    
                        db.SubmissionFiles.AddRange(toAdd);
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
        

        #region Update
        public long EditAcceptenceForReviewer(long fileid,string attr)
        {   
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    SubmissionFile submissionfile = db.SubmissionFiles.Where(e => e.Id == fileid).FirstOrDefault();
                    if(attr== "isAcceptedforReview")
                    submissionfile.isAcceptedforReview = true;
                    if (attr == "isAcceptedforCopyEditing")
                        submissionfile.isAcceptedforCopyEditing = true;
                    if (attr == "isAcceptedforProduction")
                        submissionfile.isAcceptedforProduction = true;

                    submissionfile.isSubmission = false;
                    submissionfile.isRevision = false;
                    submissionfile.isCopyedited = false;
                    db.SaveChanges();

                    return fileid;
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
