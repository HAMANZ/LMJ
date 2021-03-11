using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.Issue;
using Anz.LMJ.BLO.LogicObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Anz.LMJ.BLL;
using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LogicObjects.Review;

namespace Anz.LMJ.WebServices
{
    public class HomeServices
    {


        public DynamicResponse<SelectLO> GetOption()
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetOption();


        }

        public DynamicResponse<List<Options>> GetArticlesType()
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetArticlesType();

        }
        public DynamicResponse<List<Options>> GetArticlesType(List<long> Ids)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetArticlesType(Ids);

        }
        public DynamicResponse<List<SubmissionLO>> GetSubmissionLatestArticles(int limit)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetSubmissionLatestArticles(limit);

        }

        public DynamicResponse<List<SubmissionLO>> GetArticles(List<long> Ids)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetArticles(Ids);

        }

        public DynamicResponse<List<IssueLO>> GetAllIssues()
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetAllIssues();

        }
        public DynamicResponse<IssueLO> GetLatestIssues()
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetLatestIssues();

        }
        public DynamicResponse<List<SubmissionLO>> GetRelatedIssues(long issueid,int limit)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetRelatedIssues(issueid,limit);


        }


        public DynamicResponse<SubmissionLO> GetArticle(long submissionId)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.GetArticle(submissionId);
        }

        public DynamicResponse<List<SubmissionLO>> GetArticle(List<long> submissionIds)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.GetArticle(submissionIds);
        }
        public DynamicResponse<List<SubmissionLO>> GetAllArticles()
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.GetAllArticles();
        }

        public DynamicResponse<List<SubmissionLO>> GetAllArticles(long articletypeid)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.GetAllArticles(articletypeid);
        }

        public DynamicResponse<List<SubmissionLO>> ArticlesByIssueId(long issueid)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.ArticlesByIssueId(issueid);
        }

        public DynamicResponse<IssueLO> GetIssueInfo(long issueid)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.GetIssueInfo(issueid);
        }

        public DynamicResponse<List<SubmissionLO>> SearchArticle(long submissionid, long issueid, long volumeid, long articletype, long author, long sectionid,string issuetitle)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            return _SubmissionLogic.SearchArticle(submissionid, issueid, volumeid, articletype, author, sectionid, issuetitle);
        }

        public DynamicResponse<List<SubmissionLO>> getListArticleOption()
        {
            HomeServices _HomeServices = new HomeServices();
             return  _HomeServices.GetAllArticles();
        }


        public DynamicResponse<long> UpdateSubmission(long submissionid, long userId, bool isEditorsPick, bool isTopReader,List<long> tagsid)
        {
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            return _SubmissionLogic.UpdateSubmission(submissionid,userId,isEditorsPick,isTopReader, tagsid);
        }

        public DynamicResponse<UserLO> Auth(string email,string pass) {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
            UserLO user = new UserLO();

            response = _UserLogic.GetBasic(email,pass);
           
            return response;
        }

        public DynamicResponse<UserLO> GetUserInfo(long userdid)
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
            UserLO user = new UserLO();

            response = _UserLogic.GetBasic(userdid);
            
            return response;
        }

        public DynamicResponse<long> AddReviewByUser(ReviewLO toAdd)
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<long> response = new DynamicResponse<long>();
            UserLO user = new UserLO();

            response = _UserLogic.AddReviewByUser(toAdd);

            return response;
        }

        public void ContactUs(Contactus c)
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<long> response = new DynamicResponse<long>();
            UserLO user = new UserLO();

            Tools.sendEmail(c.Email, c.Subject, c.Message);

        }
        

    }
}
