using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission.Discussion;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Anz.LMJ.BLO.LogicObjects.Submission;

namespace Anz.LMJ.BLL.Logic
{
    public class DiscussionLogic
    {
        private DynamicResponse<DiscussionLO> GetBasicInfo(long discussionId)
        {
            DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();

            #region Accessors
            DiscussionAccessor _DiscussionAccessor = new DiscussionAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion
            try
            {
                DiscussionLO data = new DiscussionLO();

                //get the discussion 
                Discussion discussionModel = _DiscussionAccessor.Get(discussionId);

                //subject and id
                data.Subject = discussionModel.Subject;
                data.Id = discussionModel.Id;


                //original sender 
                #region Original Sender
                User sender = _UserAccessor.Get(discussionModel.SenderId);
                if (sender == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no sender";

                    return response;
                }
                UserLO originalSender = new UserLO();
                originalSender.Email = sender.Email;
                originalSender.FirstName = sender.FirstName;
                originalSender.LastName = sender.LastName;

                data.MainSender = originalSender;

                #endregion

                //isclosed
                data.isClosed = discussionModel.isClosed;

                //number of replies
                List<Discussion> replies = new List<Discussion>();
                replies = _DiscussionAccessor.GetReplies(discussionId);

                data.NumberOfReplies = replies.Count;


                //last replier
                #region last repkier
                if (replies.Count != 0)
                {
                    User lastreplier = _UserAccessor.Get(replies[replies.Count - 1].SenderId);
                    if (lastreplier == null)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Please try again later.";
                        response.ServerMessage = "no sender";

                        return response;
                    }
                    UserLO lastReplier = new UserLO();
                    lastReplier.Email = sender.Email;
                    lastReplier.FirstName = sender.FirstName;
                    lastReplier.LastName = sender.LastName;

                    data.LastSender = lastReplier;

                }

                #endregion

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;

                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        private DynamicResponse<DiscussionLO> GetBasicInfo(Discussion discussionModel)
        {
            DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();
            
            try
            {

                response = GetBasicInfo(discussionModel.Id);
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        private DynamicResponse<List<DiscussionFileLO>> GetDiscussionFile(long  discussionId)
        {
            #region Accessors
            DiscussionFileAccessor _DiscussionFileAccessor = new DiscussionFileAccessor();
            #endregion

            #region Logic
            FileTypeLogic _FileTypeLogic = new FileTypeLogic();
            #endregion
            DynamicResponse<List<DiscussionFileLO>> response = new DynamicResponse<List<DiscussionFileLO>>();
            try
            {
                List<DiscussionsFile> fileModels = _DiscussionFileAccessor.GetList(discussionId);
                List<DiscussionFileLO> files = new List<DiscussionFileLO>();

                foreach (DiscussionsFile item in fileModels)
                {
                    files.Add(new DiscussionFileLO {
                        Name = item.FileName,
                        Type = _FileTypeLogic.GetFileType((long)item.ComponentId),
                    });
                }

                response.Data = files;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        private DynamicResponse<List<DiscussionParticipantsLO>> GetDiscussionParticipants(long discussionId)
        {
            #region Accessors
            DiscussionParticipantAccessor _DiscussionParticipantAccessor = new DiscussionParticipantAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion
            DynamicResponse<List<DiscussionParticipantsLO>> response = new DynamicResponse<List<DiscussionParticipantsLO>>();
            try
            {
                List<DiscussionParticipant> participantsModel = new List<DiscussionParticipant>();
                participantsModel = _DiscussionParticipantAccessor.GetList(discussionId);

                List<DiscussionParticipantsLO> data = new List<DiscussionParticipantsLO>();

                //get user for each useid in the discussion participants
                User user = new User();
                foreach (DiscussionParticipant item in participantsModel)
                {
                    user = new User();
                    user = _UserAccessor.Get(item.UserId);

                    if(user == null)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Please try again later";
                        response.ServerMessage = "no participant";

                        return response;
                    }

                    data.Add(new DiscussionParticipantsLO {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });
                }

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;

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

        public DynamicResponse<List<DiscussionLO>> GetListOfBasicDiscussionsForSubmission(long submissionId, bool isPrereview, bool isReview, bool isCopyEditing, bool isProofReading)
        {
            DynamicResponse<List<DiscussionLO>> response = new DynamicResponse<List<DiscussionLO>>();

            #region Accessors
            DiscussionAccessor _DiscussionAccessor = new DiscussionAccessor();
            #endregion

            List<DiscussionLO> data = new List<DiscussionLO>();
            try
            {
                //get list of discussion
                List<Discussion> disucssionsModel = _DiscussionAccessor.GetList(submissionId, isPrereview, isReview, isCopyEditing, isProofReading);

                //use GetBasicInfo for each discussion
                DynamicResponse<DiscussionLO> discussionLO = new DynamicResponse<DiscussionLO>();
                foreach (Discussion item in disucssionsModel)
                {
                    if (item.ChannelId == null) {
                        discussionLO = new DynamicResponse<DiscussionLO>();

                        discussionLO = GetBasicInfo(item);

                        if (discussionLO.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = HttpStatusCode.InternalServerError;
                            response.Message = "Please try again later.";
                            response.ServerMessage = "no discussion reply";

                            return response;
                        }

                        data.Add(discussionLO.Data);
                    }
                
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;

                return response;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
    
        public DynamicResponse<DiscussionLO> GetWholeDiscussion(long discussionId)
        {
            DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();
            #region Accessor
            DiscussionAccessor _DiscussionAccessor = new DiscussionAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion
            try
            {
                User user = new User();
                DiscussionLO data = new DiscussionLO();
                //get list of discussion
                Discussion main = _DiscussionAccessor.Get(discussionId);
                if(main == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no discussion";

                    return response;
                }

                data.Subject = main.Subject;
                data.DiscussionDate = (DateTime)main.SysDate;
                data.isClosed = main.isClosed;
                data.Message = main.Message;

                user = _UserAccessor.Get(main.SenderId);
                UserLO mainSender = new UserLO();
                mainSender.Email = user.Email;
                mainSender.FirstName = user.FirstName;
                mainSender.LastName = user.LastName;

                data.MainSender = mainSender;

                DynamicResponse<List<DiscussionFileLO>> mainfiles = new DynamicResponse<List<DiscussionFileLO>>();
                //get files
                mainfiles = GetDiscussionFile(main.Id);

                    if (mainfiles.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = mainfiles.HttpStatusCode;
                        response.Message = mainfiles.Message;
                        response.ServerMessage = mainfiles.ServerMessage;

                        return response;
                    }

                data.Files = mainfiles.Data;
                //get replies for the discussion
                #region Replies


                List<Discussion> replies = new List<Discussion>();

                replies = _DiscussionAccessor.GetReplies(discussionId);

                List<DiscussionContentLO> contentOfDiscussion = new List<DiscussionContentLO>();
                DynamicResponse<List<DiscussionFileLO>> files = new DynamicResponse<List<DiscussionFileLO>>();
                foreach (Discussion item in replies)
                {
                    //get files
                    files = GetDiscussionFile(item.Id);

                    if (files.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = files.HttpStatusCode;
                        response.Message = files.Message;
                        response.ServerMessage = files.ServerMessage;

                        return response;
                    }

                    user=_UserAccessor.Get(item.SenderId);
                    UserLO Sender = new UserLO();
                    Sender.Email = user.Email;
                    Sender.FirstName = user.FirstName;
                    Sender.LastName = user.LastName;
                   
                    contentOfDiscussion.Add(new DiscussionContentLO
                    {
                        Body = item.Message,
                        SentDate = (DateTime)item.SysDate,
                        Sender = Sender,
                        Files = files.Data
                    });
                }

                data.Discussions = contentOfDiscussion;

                #endregion

                //get participants
                #region Participants
                DynamicResponse<List<DiscussionParticipantsLO>> participants = GetDiscussionParticipants(discussionId);
                if(participants.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = participants.HttpStatusCode;
                    response.Message = participants.Message;
                    response.ServerMessage = participants.ServerMessage;

                    return response;
                }

                data.Participants = participants.Data;
                #endregion
               

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later";
                response.ServerMessage = ex.Message;
                return response;
          
            }
        }

        public DynamicResponse<DiscussionLO> AddDiscussionContent(DiscussionContentLO toAdd,long submissionid,long userid)
        {
            #region Accessor
            DiscussionAccessor _DiscussionAccessor = new DiscussionAccessor();
            DiscussionFileAccessor _DiscussionFileAccessor = new DiscussionFileAccessor();
            DiscussionParticipantAccessor _DiscussionParticipantAccessor = new DiscussionParticipantAccessor();
            #endregion

            DiscussionLogic _DiscussionLogic = new DiscussionLogic();
            Discussion _Discussion = new Discussion();
            DiscussionLO _DiscussionLO = new DiscussionLO();
            DiscussionsFile _DiscussionFile = new DiscussionsFile();
            List<DiscussionsFile> _DiscussionFileList = new List<DiscussionsFile>();
            DiscussionParticipant _DiscussionParticipant = new DiscussionParticipant();
            List<DiscussionParticipant> _DiscussionParticipantList = new List<DiscussionParticipant>();
            
            DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();

            try
            {
                
                        _Discussion.Subject = toAdd.Subject;
                        _Discussion.Message = toAdd.Body;
                        _Discussion.SubmissionId = submissionid;
                        if (toAdd.ChannelId == 0)
                            _Discussion.ChannelId = null;
                        else
                            _Discussion.ChannelId = toAdd.ChannelId;
                        _Discussion.SenderId = userid;
                        _Discussion.isProofReading = toAdd.isProofReadingReview;
                        _Discussion.isPrereview = toAdd.isPreReview;
                        _Discussion.isReview = toAdd.isReview;
                        _Discussion.isCopyEditing = toAdd.isCopyEditingReview;
                        _Discussion.isClosed = toAdd.isClosed;
                        _Discussion.SysDate = DateTime.Now;

                if (toAdd.Id == null)
                    {
                        _Discussion = _DiscussionAccessor.Add(_Discussion);

                        if (_Discussion == null)
                        {
                            response.HttpStatusCode = HttpStatusCode.InternalServerError;
                            response.Message = "Check the Discussion.";
                            response.ServerMessage = "Discussion can't be added";
                            return response;
                        }
                    if (toAdd.Files!=null)
                    {
                        for (int i = 0; i < toAdd.Files.Count(); i++)
                        {
                            _DiscussionFile = new DiscussionsFile();
                            _DiscussionFile.DiscussionId = _Discussion.Id;
                            _DiscussionFile.FileName = toAdd.Files[i].Name;
                            _DiscussionFile.ComponentId = long.Parse(toAdd.Files[i].Type);
                            _DiscussionFile.isDeleted = false;
                            _DiscussionFile.SysDate = DateTime.Now;
                            _DiscussionFileList.Add(_DiscussionFile);
                        }

                        _DiscussionFileList = _DiscussionFileAccessor.Add(_DiscussionFileList);

                        if (_DiscussionFileList == null || _DiscussionFileList.Count == 0)
                        {
                            response.HttpStatusCode = HttpStatusCode.InternalServerError;
                            response.Message = "Check the Discussion files.";
                            response.ServerMessage = "Discussion files can't added";
                            return response;
                        }
                    }
                    if (toAdd.Participantids != null)
                    {
                        for (int i = 0; i < toAdd.Participantids.Count(); i++)
                        {
                            _DiscussionParticipant = new DiscussionParticipant();
                            _DiscussionParticipant.DiscussionId = _Discussion.Id;
                            _DiscussionParticipant.UserId = toAdd.Participantids[i];
                            _DiscussionParticipantList.Add(_DiscussionParticipant);
                        }
                        _DiscussionParticipantAccessor.Add(_DiscussionParticipantList);
                    }
                }
                    
                response = _DiscussionLogic.GetBasicInfo(_Discussion);
                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = _DiscussionLO;
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


    }
}
