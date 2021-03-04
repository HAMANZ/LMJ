using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.Submission.Discussion;
using Anz.LMJ.BLO.LogicObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace Anz.LMJ.WebServices
{
    public class ManagementService
    {
        public DynamicResponse<UserQueueLO> GetQueue(long userId,long journalId)
        {
            #region Logic
            UserLogic _UserLogic = new UserLogic();
            #endregion
            return _UserLogic.GetQueue(userId, journalId);
            
        }

        public DynamicResponse<SelectLO> GetSelect()
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetSelect();

        }

        public DynamicResponse<List<SubmissionLO>> GetUnAssigned(long userId)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            return _SubmissionLogic.GetUnAssignedSubmissions(userId);

        }

        public DynamicResponse<SubmissionLO> GetSubmission(long userId,long submissionId)
        {
            #region Logic
            UserLogic _UserLogic = new UserLogic();
            #endregion

            return _UserLogic.GetSubmission(userId, submissionId);
        }

        public DynamicResponse<DiscussionLO> GetDiscussion(long discussionId)
        {
            #region Logic
            DiscussionLogic _DiscussionLogic = new DiscussionLogic();
            #endregion

            DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();
            response = _DiscussionLogic.GetWholeDiscussion(discussionId);
            return response;
        }
        

       

    }
}
