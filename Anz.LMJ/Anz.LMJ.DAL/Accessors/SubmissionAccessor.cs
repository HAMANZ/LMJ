using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SubmissionAccessor
    {
        #region Get

        public List<Submission> GetList(List<long> Ids)
        {            
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => Ids.Contains(e.Id) && e.isDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<Submission> GetListByUserId(long userId)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => e.UserId == userId && e.isDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Submission> GetListBySectionId(long sectionId)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => e.SectionId == sectionId && e.isDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Submission> GetListByIssueId(List<long> IssueIds,int limit)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => IssueIds.Contains((long)e.IssueId) && e.isDeleted == false).OrderByDescending(o => o.PublishedDate)
                        .Take(limit).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        

        public Submission Get(long id)
        {
            try
            {
                Submission submission = new Submission();
                using (LMJEntities db = new LMJEntities())
                {
                    submission = db.Submissions.Where(e => e.Id == id).FirstOrDefault();
                }

                return submission;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Submission> GetPublishedSubmissionList()
        {
            List<Submission> response = new List<Submission>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Submissions.Where(e => e.isDeleted == false && e.IssueId != null && e.IssueId!=0 ).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Submission> GetList(long submissionid, long issueid,long volumeid,long articletype,long author,long sectionid,string issuetitle)
        {
            try
            {
                string query;
                List<Submission> data = new List<Submission>();
                query = "Select * from Submissions S where S.IssueId != 0 and S.isDeleted = 'False'";
                using (LMJEntities db = new LMJEntities())
                {


                    if (issueid != 0)
                    {
                        query += " and S.issueid =" + issueid.ToString();
                    }


                    if (articletype != 0)
                    {
                        query += " and S.articletypeid =" + articletype.ToString();
                    }


                    if (sectionid != 0)
                    {
                        query += " and S.SpecialitiesId =" + sectionid.ToString();
                    }


                    if (submissionid != 0)
                    {
                        query += " and S.Id =" + submissionid;
                    }

                    if (volumeid != 0)
                    {
                        query += " and S.IssueId in (select Id from Issues where NewsletterId = " + volumeid.ToString() + ")";
                    }

                    if (issuetitle !=null)
                    {
                        query += " and S.Title like '%" + issuetitle + "%'";
                    }

                    

                    data = db.Submissions.SqlQuery(query).ToList<Submission>();

                   
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

      
        public List<string> GetSubmissionFieldName()
        {
            try
            {
                List<string> data = new List<string>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = typeof(Submission).GetProperties().Select(property => property.Name).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
            public List<Submission> GetArticles()
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e=>e.IssueId != null && e.isDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Submission> GetArticles(List<long> Ids)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => Ids.Contains(e.Id) && e.isDeleted == false & e.IssueId != null).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<Submission> GetArticlesByIssue(long id)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => e.IssueId ==id && e.isDeleted == false ).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<Submission> GetArticles(long articleid)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => e.IssueId != null && e.isDeleted == false && e.ArticleTypeId==articleid).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Submission> GetArticleList(List<long> Ids)
        {
            try
            {
                List<Submission> data = new List<Submission>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Submissions.Where(e => Ids.Contains(e.Id) && e.isDeleted == false && e.IssueId != null).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        

        public Submission GetArticle(long id)
        {
            try
            {
                Submission submission = new Submission();
                using (LMJEntities db = new LMJEntities())
                {
                    submission = db.Submissions.Where(e => e.Id == id && e.IssueId!=null && e.isDeleted == false).FirstOrDefault();
                }

                return submission;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region Add
        public Submission Add(Submission toAdd)
        {

            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Submissions.Add(toAdd);
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

        #region Update
        public long LinkToNewsletter(long submissionid, long newsletterid)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    Submission submissios = db.Submissions.Where(e => e.Id == submissionid).FirstOrDefault();
                    submissios.IssueId = newsletterid;
                    db.SaveChanges();
                    return newsletterid;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public Submission Edit(Submission s)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    Submission submissios = db.Submissions.Where(e => e.Id == s.Id).FirstOrDefault();
                    submissios.IsEditorsPick = s.IsEditorsPick;
                    submissios.IsTopReader = s.IsTopReader;
                    submissios.UserId = s.UserId;
                    submissios.Prefix = s.Prefix;
                    submissios.SectionId = s.SectionId;
                    submissios.StudyTypeId = s.StudyTypeId;
                    submissios.Library = s.Library;
                    submissios.GUID = s.GUID;
                    submissios.Title = s.Title;
                    submissios.Subtitle = s.Subtitle; 
                    submissios.Abstract = s.Abstract;
                    submissios.MiniDescription = s.MiniDescription;
                    submissios.ResearchId = s.ResearchId;
                    submissios.QuestionId = s.QuestionId;
                    submissios.ArticleTypeId = s.ArticleTypeId;
                    submissios.CommentsForEditor = s.CommentsForEditor;
                    submissios.CoverPhoto = s.CoverPhoto;
                    submissios.BannerImage = s.BannerImage;
                    submissios.SourcesOfFunding = s.SourcesOfFunding;
                    submissios.ConflictsOfInterests = s.ConflictsOfInterests;
                    submissios.Significance = s.Significance;
                    submissios.SpecialitiesId = s.SpecialitiesId;
                    submissios.ConflictsOfInterests = s.ConflictsOfInterests;
                    submissios.Significance = s.Significance;
                    submissios.SpecialitiesId = s.SpecialitiesId;
                     db.SaveChanges();
                    return submissios;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

       

        public long Update(Submission submission)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    Submission _Submission = db.Submissions.Where(e => e.Id == submission.Id).FirstOrDefault();
                    //_Submission.UserId = submission.UserId;
                    //_Submission.SectionId = 1;
                    //_Submission.CommentsForEditor = submission.CommentsForEditor;
                    //_Submission.Prefix = submission.Prefix;
                    _Submission.Title = submission.Title;
                    _Submission.SpecialitiesId = submission.SpecialitiesId;
                    _Submission.Subtitle = submission.Subtitle;
                    _Submission.Abstract = submission.Abstract;
                    _Submission.ArticleTypeId = submission.ArticleTypeId;
                    //_Submission.StudyTypeId = 1;
                    //_Submission.ResearchId = submission.ResearchId;
                    //_Submission.SubjectId = submission.SubjectId;
                    //_Submission.QuestionId = submission.QuestionId;
                    //_Submission.IssueId = null;
                    //_Submission.isDeleted = false;
                    _Submission.IsEditorsPick = submission.IsEditorsPick;
                    _Submission.IsTopReader = submission.IsTopReader;
                    //_Submission.SysDate = DateTime.Now;
                    //_Submission.GUID = null;
                    //_Submission.LibraryId = null;
                    //_Submission.MiniDescription = submission.MiniDescription;
                    //_Submission.Significance = submission.Significance;
                    //_Submission.SourcesOfFunding = submission.SourcesOfFunding;
                    //_Submission.ConflictsOfInterests = submission.ConflictsOfInterests;
                    _Submission.CoverPhoto = submission.CoverPhoto;
                    //_Submission.isDraft = true;
                    //_Submission.isApproved = submission.isApproved;
                    _Submission.BannerImage = submission.BannerImage;
                    db.SaveChanges();
                    return submission.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #endregion

        #region Delete

        #endregion
    }
}
