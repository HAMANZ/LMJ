using Anz.LMJ.BLO.LogicObjects;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using static Anz.LMJ.BLL.Logic.Enums;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.User;

namespace Anz.LMJ.BLL.Logic
{
    public class UserLogic
    {
        #region Get User Info
        public DynamicResponse<UserLO> GetBasic(string email ,string password)
        {
            #region Accessor
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion
            DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
            try
            {
                User userModel = _UserAccessor.Get(email,password);

                response = GetUser(userModel);
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public DynamicResponse<UserLO> GetBasic(long userId)
        {
            #region Accessor
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion

            DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();

            User userModel = _UserAccessor.Get(userId);

            response = GetUser(userModel);
            return response;
        }

        private DynamicResponse<UserLO> GetUser(User userModel)
        {
            #region Accessor
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion

            DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();

            try
            {
                if(userModel == null)
                {
                    response.HttpStatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Wrong Credentials";
                    return response;
                }

                //user exists
                UserLO data = new UserLO();

                #region basic info
                data.Id = userModel.Id;
                data.Email = userModel.Email;
                data.FirstName = userModel.FirstName;
                data.LastName = userModel.LastName;
                #endregion





                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                response.ServerMessage = ex.Message;
            }

            return response;
        }


        

        #endregion


        #region Roles
        public DynamicResponse<bool> CheckRole (long userId,Roles role,long journalId)
        {
            #region Accessors
            UserAccessor _UserAccessor = new UserAccessor();
            UserRoleAccessor _UserRoleAccessor = new UserRoleAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            SectionAccessor _SectionAccessor = new SectionAccessor();
            #endregion

            DynamicResponse<bool> response = new DynamicResponse<bool>();
            bool data;
            try
            {

                User usermodel = new User();
                usermodel = _UserAccessor.Get(userId);


                UserRole rolemodel = new UserRole();
                rolemodel = _UserRoleAccessor.Get(role.ToString());
                if(rolemodel == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.ServerMessage = "null role";
                    response.Message = "Please try again later";

                    return response;
                }

                //check if role is sectionated
                UserRolesInJournal userRolesmodel = new UserRolesInJournal();
                if((bool)rolemodel.isSectionated)
                {
                    //get sections
                    List<Section> sections = _SectionAccessor.GetList(journalId);
                    List<long> secitonIds = sections.Select(s => s.Id).ToList();
                    userRolesmodel = _UserRolesInJournalAccessor.Get(usermodel.Id, rolemodel.Id, secitonIds);
                }
                else
                {
                    userRolesmodel = _UserRolesInJournalAccessor.Get(usermodel.Id, rolemodel.Id);
                }
                if(userRolesmodel == null)
                {
                    data= false;
                }
                else
                {
                    data = true;
                }
                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;

                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }
       
        /// <summary>
        /// checks if the user is a subscriber to the journal.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="journalId"></param>
        /// <returns></returns>
        public DynamicResponse<bool> isSubscriber(long userId,long journalId)
        {
            try
            {
                return CheckRole(userId, Roles.customer, journalId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
        public DynamicResponse<bool> isStaff(long userId,long journalId)
        {
            
            try
            {
                DynamicResponse<bool> isRole = new DynamicResponse<bool>();
                //check roles one by one

                //start with author
                isRole = CheckRole(userId, Roles.author, journalId);

                if(isRole.HttpStatusCode !=HttpStatusCode.OK)
                {
                    return isRole;
                }
                else
                {
                    if(isRole.Data)
                    {
                        return isRole;
                    }
                }

                //move to editor
                isRole = CheckRole(userId, Roles.editor, journalId);

                if (isRole.HttpStatusCode != HttpStatusCode.OK)
                {
                    return isRole;
                }
                else
                {
                    if (isRole.Data)
                    {
                        return isRole;
                    }
                }

                //move to reviewer
                isRole = CheckRole(userId, Roles.reviewer, journalId);

                if (isRole.HttpStatusCode != HttpStatusCode.OK)
                {
                    return isRole;
                }
                else
                {
                    if (isRole.Data)
                    {
                        return isRole;
                    }
                }

                return isRole;


            }
            catch (Exception ex)
            {
                DynamicResponse<bool> response = new DynamicResponse<bool>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }
        #endregion


        public DynamicResponse<UserQueueLO> GetQueue(long userId,long journalId)
        {
            try
            {
                DynamicResponse<UserQueueLO> response = new DynamicResponse<UserQueueLO>();
                UserQueueLO data = new UserQueueLO();

                //submissions for author
                //check if user is author
                #region Author

                DynamicResponse<bool> isAuthor = CheckRole(userId, Roles.author, journalId);
                if (isAuthor.HttpStatusCode == HttpStatusCode.OK)
                {
                    if (isAuthor.Data)
                    {
                        data.isAuthor = true;
                        DynamicResponse<List<SubmissionLO>> asAuhtor = GetSubmissionByRole(userId, journalId, Roles.author);

                        if (asAuhtor.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = asAuhtor.HttpStatusCode;
                            response.Message = asAuhtor.Message;
                            response.ServerMessage = asAuhtor.ServerMessage;

                            return response;
                        }
                        else
                        {
                            data.asAuthor = asAuhtor.Data;
                        }

                    }
                    else
                    {
                        data.isAuthor = false;

                    }
                }
                else
                {
                    //return the error
                    response.HttpStatusCode = isAuthor.HttpStatusCode;
                    response.Message = isAuthor.Message;
                    response.ServerMessage = isAuthor.ServerMessage;

                    return response;
                }

                #endregion


                //submissions for editor (User is responsible for the editing task)
                #region Editor

                DynamicResponse<bool> isEditor = CheckRole(userId, Roles.editor, journalId);
                if (isEditor.HttpStatusCode == HttpStatusCode.OK)
                {
                    if (isEditor.Data)
                    {
                        data.isEditor = true;
                        DynamicResponse<List<SubmissionLO>> asEditor = GetSubmissionByRole(userId, journalId, Roles.editor);

                        if (asEditor.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = asEditor.HttpStatusCode;
                            response.Message = asEditor.Message;
                            response.ServerMessage = asEditor.ServerMessage;

                            return response;
                        }
                        else
                        {
                            data.asEditor = asEditor.Data;
                        }

                    }
                    else
                    {
                        data.isEditor = false;

                    }
                }
                else
                {
                    //return the error
                    response.HttpStatusCode = isEditor.HttpStatusCode;
                    response.Message = isEditor.Message;
                    response.ServerMessage = isEditor.ServerMessage;

                    return response;
                }

                #endregion




                //submissions for editor (User is responsible for the editing task)
                #region CopyEditor

                DynamicResponse<bool> isCopyEditor = CheckRole(userId,Roles.copyediting, journalId);
                if (isCopyEditor.HttpStatusCode == HttpStatusCode.OK)
                {
                    if (isCopyEditor.Data)
                    {
                        data.isCopyEditor = true;
                        DynamicResponse<List<SubmissionLO>> asCopyEditor= GetSubmissionByRole(userId, journalId, Roles.copyediting);

                        if (asCopyEditor.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = asCopyEditor.HttpStatusCode;
                            response.Message = asCopyEditor.Message;
                            response.ServerMessage = asCopyEditor.ServerMessage;

                            return response;
                        }
                        else
                        {
                            data.asCopyEditor = asCopyEditor.Data;
                        }

                    }
                    else
                    {
                        data.isCopyEditor = false;

                    }
                }
                else
                {
                    //return the error
                    response.HttpStatusCode = isCopyEditor.HttpStatusCode;
                    response.Message = isCopyEditor.Message;
                    response.ServerMessage = isCopyEditor.ServerMessage;

                    return response;
                }

                #endregion


                //submissions for editor (User is responsible for the editing task)
                #region Proofreader

                DynamicResponse<bool> isProofReader = CheckRole(userId, Roles.proofreading, journalId);
                if (isProofReader.HttpStatusCode == HttpStatusCode.OK)
                {
                    if (isProofReader.Data)
                    {
                        data.isProofReader = true;
                        DynamicResponse<List<SubmissionLO>> asProofreader = GetSubmissionByRole(userId, journalId, Roles.proofreading);

                        if (asProofreader.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = asProofreader.HttpStatusCode;
                            response.Message = asProofreader.Message;
                            response.ServerMessage = asProofreader.ServerMessage;

                            return response;
                        }
                        else
                        {
                            data.asProofReader = asProofreader.Data;
                        }

                    }
                    else
                    {
                        data.isProofReader = false;

                    }
                }
                else
                {
                    //return the error
                    response.HttpStatusCode = isProofReader.HttpStatusCode;
                    response.Message = isProofReader.Message;
                    response.ServerMessage = isProofReader.ServerMessage;

                    return response;
                }

                #endregion



                //submissions for reviewer (User is responsible for the reviewing task)
                #region Reviewer

                DynamicResponse<bool> isReviewer = CheckRole(userId, Roles.reviewer, journalId);
                if (isReviewer.HttpStatusCode == HttpStatusCode.OK)
                {
                    if (isReviewer.Data)
                    {
                        data.isReviewer = true;
                        DynamicResponse<List<SubmissionLO>> asReviewer = GetSubmissionByRole(userId, journalId, Roles.reviewer);

                        if (asReviewer.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = asReviewer.HttpStatusCode;
                            response.Message = asReviewer.Message;
                            response.ServerMessage = asReviewer.ServerMessage;

                            return response;
                        }
                        else
                        {
                            data.asReviewer = asReviewer.Data;
                        }

                    }
                    else
                    {
                        data.isReviewer = false;

                    }
                }
                else
                {
                    //return the error
                    response.HttpStatusCode = isReviewer.HttpStatusCode;
                    response.Message = isReviewer.Message;
                    response.ServerMessage = isReviewer.ServerMessage;

                    return response;
                }

                #endregion




                response.Data=data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                DynamicResponse<UserQueueLO> response = new DynamicResponse<UserQueueLO>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }



    

        public DynamicResponse<SubmissionLO> GetSubmission(long userId,long submissionId)
        {
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            #region Accessors
            UserRoleAccessor _UserRoleAccessor = new UserRoleAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            try
            {
                //get role for user
                List<UserRolesInJournal> rolesOfUser = _UserRolesInJournalAccessor.GetList(userId);
                if(rolesOfUser.Count == 0 )
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }
                //get info for the roles
                List<int> roleIds = rolesOfUser.Select(s => s.RoleId).ToList();
                List<UserRole> roles = _UserRoleAccessor.GetList(roleIds);

                if (roles.Where(e => e.isBlinded == true).FirstOrDefault() != null)
                {
                    response =  _SubmissionLogic.GetSubmission(submissionId, true, roles.Where(e => e.isBlinded == true).FirstOrDefault().Id);
                    return response;
                }
                else
                {
                    response = _SubmissionLogic.GetSubmission(submissionId, false, roles[0].Id);
                    return response;
                }



            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }


        /// <summary>
        /// Returns the submission that a user in working in under a specific role.
        /// all roles of the staff can be tested expect the author
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="journalId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private DynamicResponse<List<SubmissionLO>> GetSubmissionByRole( long userId, long journalId,Roles role)
        {

            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();


            if(role == Roles.author)
            {
                return GetSubmissionForAuthor(userId,journalId);
            }

           

            #region Accessors
            UserRoleAccessor _UserRoleAccessor = new UserRoleAccessor();
            ProcessAccessor _ProcessAccessor = new ProcessAccessor();
            UserResponsibleInProcessAccessor _UserResponsibleInProcessAccessor = new UserResponsibleInProcessAccessor();
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            #endregion

            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion


            try
            {

                //get role
                UserRole userRole = _UserRoleAccessor.Get(role.ToString());

                //get process by role
                List<Process> processes = new List<Process>();
                processes = _ProcessAccessor.GetList(userRole.Id);
                List<UserResponsibleInProcess> responsibilities = new List<UserResponsibleInProcess>();
                //get responsibilities of the user 
               
                   responsibilities = _UserResponsibleInProcessAccessor.GetList(userId);
              
                //get submission in process based on the IDs found in responsibilities
                List<long> submissionProcessId = responsibilities.Select(s => (long)s.SubmissionProcessId).Distinct().ToList();

                //get list of process id
                List<long> proccessId = processes.Select(s => s.Id).Distinct().ToList();

                //get submission in process where the submissionprocess id is in the list and the process id is also in the list
                List<SubmissionInProcess> inProcess = _SubmissionInProcessAccessor.GetList(submissionProcessId, proccessId);

                //get list of submission ids
                List<long> submissionIds = inProcess.Select(s =>(long) s.SubmissionId).Distinct().ToList();

                //get the info of the submissions using the ids
                DynamicResponse <List<SubmissionLO>> submissions = _SubmissionLogic.GetListBasicInfo(submissionIds, (bool)userRole.isBlinded);


                if(submissions.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = submissions.HttpStatusCode;
                    response.Message = submissions.Message;
                    response.ServerMessage = submissions.ServerMessage;

                    return response;
                }
                else
                {
                    response.Data = submissions.Data;
                    response.HttpStatusCode = HttpStatusCode.OK;
                }


                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        private DynamicResponse<List<SubmissionLO>> GetSubmissionForAuthor(long userId,long journalId)
        {


            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();

            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            #region Accessors
            SectionAccessor _SectionAccessor = new SectionAccessor();
            #endregion
            try
            {
                //get sections of this journal
                List<Section> sections = _SectionAccessor.GetList(journalId);
                List<long> sectionIds = sections.Select(s => s.Id).ToList();

                //get submission that have the same userid
                DynamicResponse<List<Submission>> submissions = _SubmissionLogic.GetUnArchivedSubmissionsForAuthor(userId,sectionIds);

                if(submissions.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = submissions.HttpStatusCode;
                    response.Message = submissions.Message;
                    response.ServerMessage = submissions.ServerMessage;
                    return response;
                }

                //get ids of submissions
                DynamicResponse<List<SubmissionLO>> data = _SubmissionLogic.GetListBasicInfo(submissions.Data,false);
                if(data.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = data.HttpStatusCode;
                    response.Message = data.Message;
                    response.ServerMessage = data.ServerMessage;

                    return response;
                }
                else
                {
                    response.HttpStatusCode = HttpStatusCode.OK;
                    response.Data = data.Data;

                    return response;
                }
                

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later!";
                response.ServerMessage = ex.Message;

                return response;
            }
        }


        public DynamicResponse<List<UserLO>> GetParticipant(long submissionid)
        {

            #region Accessors
            UserAccessor _UserAccessor = new UserAccessor();
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            UserResponsibleInProcessAccessor _UserResponsibleInProcessAccessor = new UserResponsibleInProcessAccessor();
            #endregion
            
            List<User> _Users = new List<User>();
            List<long> submissioninprocessids = new List<long>();
            List<long> userids = new List<long>();
            List<UserResponsibleInProcess> _UserResponsibleInProcessList = new List<UserResponsibleInProcess>();
            List<SubmissionInProcess> _SubmissionInProcessList = new List<SubmissionInProcess>();
            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
           
            try
            {
                _SubmissionInProcessList = _SubmissionInProcessAccessor.GetList(submissionid);
                submissioninprocessids=_SubmissionInProcessList.Select(s => s.Id).ToList();
                _UserResponsibleInProcessList=_UserResponsibleInProcessAccessor.GetList(submissioninprocessids);
              
                for (int i = 0; i < _UserResponsibleInProcessList.Count(); i++)
                {
                    userids.Add(_UserResponsibleInProcessList[i].UserId);
                }
                _Users = _UserAccessor.GetList(userids);

                if (_Users == null && _Users.Count() == 0)
                {
                    response.Data = null;
                    response.HttpStatusCode = HttpStatusCode.OK;
                    return response;
                }


                List<UserLO> data = new List<UserLO>();
                foreach (User item in _Users)
                {
                    data.Add(new UserLO
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email
                    });
                }

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later!";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        public DynamicResponse<List<UserLO>> GetUsers()
        {

            #region Accessors
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion

            List<User> _Users = new List<User>();
            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();

            try
            {
                _Users = _UserAccessor.GetList();

                if (_Users == null && _Users.Count() == 0)
                {
                    response.Data = null;
                    response.HttpStatusCode = HttpStatusCode.OK;
                    return response;
                }


                List<UserLO> data = new List<UserLO>();
                foreach (User item in _Users)
                {
                    data.Add(new UserLO
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email
                    });
                }

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later!";
                response.ServerMessage = ex.Message;

                return response;
            }
        }


        public DynamicResponse<List<UserLO>> GetUserByRole(string role)
        {
           
            #region Accessors
            UserRoleAccessor _UserRoleAccessor = new UserRoleAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion

            UserRole _UserRole = new UserRole();
            List<User> _Users = new List<User>();
            List<long> userids = new List<long>();
            List<UserRolesInJournal> _UserRolesInJournal = new List<UserRolesInJournal>();
            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
         
              try{

                //get role id for this role
                _UserRole = _UserRoleAccessor.Get(role);

                //get users id  that have this role id
                _UserRolesInJournal = _UserRolesInJournalAccessor.GetListUserRole(_UserRole.Id);

                for (int i = 0; i < _UserRolesInJournal.Count(); i++) {
                   userids.Add(_UserRolesInJournal[i].UserId);
                }

                _Users=_UserAccessor.GetList(userids);

                if (_Users == null && _Users.Count() == 0) {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the Reviewers .";
                    response.ServerMessage = "Reviewers can't get";
                    return response;
                }


                List<UserLO> data = new List<UserLO>();
                foreach (User item in _Users)
                {
                    data.Add(new UserLO
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email
                    });
                }

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later!";
                response.ServerMessage = ex.Message;

                return response;
            }
}



        #region Add

        //public DynamicResponse<List<UserLO>> AddReviewers(List<long> userids,long roleid)
        //{


        //    try
        //    {



        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.HttpStatusCode = HttpStatusCode.InternalServerError;
        //        response.Message = "Please try again later!";
        //        response.ServerMessage = ex.Message;

        //        return response;
        //    }
        //}

        #endregion

        public DynamicResponse<long> AddReviewByUser(long userid, string text, int nbofstars, long articleid)
        {
            #region Accessor
            ReviewAccessor _ReviewAccessor = new ReviewAccessor();
            #endregion

            Review review = new Review();
            review.UserId = (long)userid;
            review.Text = text;
            review.NbOfStars =nbofstars;
            review.IsDeleted = false;
            review.SysDate = DateTime.Now;
            review.SubmissionId = articleid;
            DynamicResponse<long> response = new DynamicResponse<long>();
            try
            {
                review = _ReviewAccessor.Add(review);

                if (review == null)
                {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the Review  .";
                    response.ServerMessage = "Review can't get";
                    return response;
                }

                response.Data = review.Id;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                response.Message = "Please try again later!";
                response.ServerMessage = ex.Message;
                return response;
            }
        }

        public DynamicResponse<List<UserLO>> AddReviewer(List<long> userids,long submissionId,long userId,Roles role,long processinmodelid)
        {
            #region Accessor
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            UserResponsibleInProcessAccessor _UserResponsibleInProcessAccessor = new UserResponsibleInProcessAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            UserRoleAccessor _UserRoleAccessor = new UserRoleAccessor();
            #endregion

            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            ProcessLogic _ProcessLogic = new ProcessLogic();
            #endregion

            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
            UserResponsibleInProcess _UserResponsibleInProcess = new UserResponsibleInProcess();
            List<UserResponsibleInProcess> _UserResponsibleInProcessList = new List<UserResponsibleInProcess>();
            List<User> _Users = new List<User>();
            UserRole _UserRole = new UserRole();
            try
            {
                SubmissionInProcess submissionInProcess = _SubmissionInProcessAccessor.GetLast(submissionId);
                if( submissionInProcess.ProcessId != processinmodelid)
                {
                    // add new submissioninprocess
                    submissionInProcess = new SubmissionInProcess();
                    submissionInProcess = _SubmissionInProcessAccessor.Add(new SubmissionInProcess {
                        ProcessId = processinmodelid,
                        isDeleted = false,
                        SubmissionId = submissionId,
                        SysDate = DateTime.Now,
                    });
                }

                    for (int i = 0; i < userids.Count(); i++)
                    {
                        _UserResponsibleInProcess.UserId = userids[i];
                        _UserResponsibleInProcess.SubmissionProcessId = submissionInProcess.Id;
                        _UserResponsibleInProcess.isAssignedByManager = false;
                        _UserResponsibleInProcess.ManagerId = null;
                        _UserResponsibleInProcess.isDeleted = false;
                        _UserResponsibleInProcess.SysDate = DateTime.Now;
                        _UserResponsibleInProcess.DueDate = DateTime.Now;
                        _UserResponsibleInProcessList.Add(_UserResponsibleInProcess);
                   
                    }

                _UserResponsibleInProcessList=_UserResponsibleInProcessAccessor.Add(_UserResponsibleInProcessList);
              

                if (_UserResponsibleInProcessList == null && _UserResponsibleInProcessList.Count()==0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the reviwers.";
                    response.ServerMessage = "reviwers can't be Added";
                    return response;
                }

                _Users = _UserAccessor.GetList(userids);

                if (_Users == null && _Users.Count() == 0)
                {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the Reviewers .";
                    response.ServerMessage = "Reviewers can't get";
                    return response;
                }
                
                List<UserLO> data = new List<UserLO>();
                foreach (User item in _Users)
                {
                    data.Add(new UserLO
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Email = item.Email
                    });
                }

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                response.Message = "Please try again later!";
                response.ServerMessage = ex.Message;
                return response;
            }
        }



        public Dictionary<string, string> AuthIsAdmin(string email, string pass)
        {
            UserAccessor _UserAccessor = new UserAccessor();
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                User usr = _UserAccessor.Get(email, pass);

                if (usr == null)
                {
                    result["isadmin"] = "false";
                    result["userid"] = "null";
                }
                else
                {
                    result["isadmin"] = usr.IsAdmin.ToString();
                    result["userid"] = usr.Id.ToString();
                }


                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public Dictionary<string, string> isAdmin(string email, string pass)
        {
            UserAccessor _UserAccessor = new UserAccessor();
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                User usr = _UserAccessor.Get(email, pass);

                if (usr == null || usr.IsAdmin != true)
                {
                    result["found"] = "false";
                    result["userid"] = "null";
                }
                else
                {
                    result["found"] = "true";
                    result["userid"] = usr.Id.ToString();
                }


                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
