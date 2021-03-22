using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.Issue;
using Anz.LMJ.BLO.LogicObjects.Submission.Discussion;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using static Anz.LMJ.BLL.Logic.Enums;
using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LogicObjects.Review;

namespace Anz.LMJ.BLL.Logic
{
    public class SubmissionLogic
    {

        #region Helpers

        private DynamicResponse<List<UserLO>> GetResponsbileUsersForSubmissionPerRole(long submissionId, Roles role)
        {
            #region Accessors
            UserRoleAccessor _UserRoleAccessor = new UserRoleAccessor();
            ProcessAccessor _ProcessAccessor = new ProcessAccessor();
            UserResponsibleInProcessAccessor _UserResponsibleInProcessAccessor = new UserResponsibleInProcessAccessor();
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion
            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
            try
            {
                //get role
                UserRole roleModel = _UserRoleAccessor.Get(role.ToString());

                //get process for these roles
                List<Process> process = _ProcessAccessor.GetList(roleModel.Id);

                List<long> processIds = process.Select(s => s.Id).ToList();
                //get submission process
                List<SubmissionInProcess> submissionInProcesses = _SubmissionInProcessAccessor.GetList(submissionId, processIds);
                List<long> submissionInProcessesIds = submissionInProcesses.Select(s => s.Id).ToList();

                List<UserResponsibleInProcess> userResponsibles = _UserResponsibleInProcessAccessor.GetList(submissionInProcessesIds);

                List<long> userIds = userResponsibles.Select(s => s.UserId).Distinct().ToList();

                //get users
                List<User> users = _UserAccessor.GetList(userIds);

                List<UserLO> data = new List<UserLO>();
                foreach (User item in users)
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
                response.Message = "Please try again later";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        private DynamicResponse<string> GetSubmissionLastStage(long submissionId)
        {
            #region Logic
            ProcessLogic _ProcessLogic = new ProcessLogic();
            #endregion
            DynamicResponse<string> response = new DynamicResponse<string>();

            try
            {

                DynamicResponse<Process> process = _ProcessLogic.GetLastProcessForSubmission(submissionId);

                if (process.HttpStatusCode != HttpStatusCode.OK)
                {
                    //return error
                    response.HttpStatusCode = process.HttpStatusCode;
                    response.Message = process.Message;
                    response.ServerMessage = process.ServerMessage;

                    return response;
                }
                else
                {
                    //return response with the process
                    response.HttpStatusCode = HttpStatusCode.OK;
                    response.Data = process.Data.StageName;

                    return response;
                }

            }
            catch (Exception ex)
            {
                //return error
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }




        private DynamicResponse<SubmissionLO> GetSubmissionBasicInfo(Submission submission, bool isBlinded)
        {

            #region Accessor
            UserAccessor _UserAccessor = new UserAccessor();
            ContributorsAccessor _ContributorsAccessor = new ContributorsAccessor();
            SubmissionKeywordsAccessor _SubmissionKeywordsAccessor = new SubmissionKeywordsAccessor();
            #endregion
            List<SubmissionKeyword> SubmissionKeywords= new List<SubmissionKeyword>();
            try
            {
                DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
                SubmissionLO data = new SubmissionLO();

                //get basic info
                data.Id = submission.Id;
                data.Title = submission.Title;
                data.Prefix = submission.Prefix;
                data.SubmissionDate = (DateTime)submission.SysDate;
                data.AbstractText = submission.Abstract;
                data.CommentsForEditor = submission.CommentsForEditor;
                data.MiniDescription = submission.MiniDescription;
                data.Photo = submission.CoverPhoto;
                data.SourcesOfFunding = submission.SourcesOfFunding;
                //stage
                DynamicResponse<string> stage = GetSubmissionLastStage(submission.Id);
                if (stage.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "can' get stage for submission";
                    return response;
                }

                data.Stage = stage.Data;
                SubmissionKeywords=_SubmissionKeywordsAccessor.GetList(submission.Id);
                List<string> _SubmissionKeywords = new List<string>();
                foreach (SubmissionKeyword item in SubmissionKeywords)
                {
                    _SubmissionKeywords.Add(item.keywords);
                 }

                data.SubmissionKeywords = _SubmissionKeywords;
                

                StudyTypeLO studyType = new StudyTypeLO();
                studyType.Id = (long)submission.StudyTypeId;
                studyType.Name = "static";

                data.StudyType = studyType;

                ArticleTypeLO articleType = new ArticleTypeLO();
                articleType.Id = (long)submission.ArticleTypeId;
                articleType.Name = "static";

                data.ArticleType = articleType;

                if (!isBlinded)
                {
                    //get the author
                    #region Author
                    User userModel = new User();
                    userModel = _UserAccessor.Get(submission.UserId);
                    if (userModel == null)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Please try again later";
                        response.ServerMessage = "no author";
                        return response;
                    }
                    UserLO author = new UserLO();
                    author.FirstName = userModel.FirstName;
                    author.LastName = userModel.LastName;
                    author.Email = userModel.Email;

                    data.Author = author;
                    #endregion

                    //get contributores
                    #region Contributors

                    List<Contributor> contributorsModel = _ContributorsAccessor.GetList(submission.Id,false);
                    List<UserLO> contirbutors = new List<UserLO>();
                    foreach (Contributor item in contributorsModel)
                    {
                        //get the user
                        if(item.UserId != null)
                        {
                            userModel = new User();
                            userModel = _UserAccessor.Get((long)item.UserId);
                            contirbutors.Add(new UserLO
                            {
                                FirstName = userModel.FirstName,
                                LastName = userModel.LastName,
                                Email = userModel.Email,
                               isCorresponding =(bool)item.isCorresponding,
                            });
                        }
                        else
                        {
                            contirbutors.Add(new UserLO
                            {
                                FirstName = item.Fname,
                                LastName = item.Lname,
                                Email =item.Email,
                                isCorresponding= (bool)item.isCorresponding
                            });
                        }

                       


                     
                    }

                    data.Contributors = contirbutors;
                    #endregion

                }

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;

            }
            catch (Exception ex)
            {
                DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }


        private DynamicResponse<SubmissionLO> GetArticleBasicInfo(Submission submission, bool isBlinded)
        {

            #region Accessor
            IssueAccessor _IssueAccessor = new IssueAccessor();
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            ContributorsAccessor _ContributorsAccessor = new ContributorsAccessor();
            SubmissionKeywordsAccessor _SubmissionKeywordsAccessor = new SubmissionKeywordsAccessor();
            NewsletterAccessor _NewsletterAccessor = new NewsletterAccessor();
            ArticleTypeAccessor _ArticleTypeAccessor = new ArticleTypeAccessor();
            SpecialitiesAccessor _SpecialitiesAccessor = new SpecialitiesAccessor();
            ReviewAccessor _ReviewAccessor = new ReviewAccessor();
            #endregion
            List<SubmissionKeyword> SubmissionKeywords = new List<SubmissionKeyword>();
            List<Review> review = new List<Review>();
            List<UserLO> tags = new List<UserLO>();
            SubmissionFile Submissionfile = new SubmissionFile();
            Issue issue = new Issue();
            ArticleType articletype = new ArticleType();
            Newsletter newsletter = new Newsletter();
            Speciality specility = new Speciality();
           
            try
            {
                DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
                SubmissionLO data = new SubmissionLO();

                //get basic info
                data.Id = submission.Id;
                data.Title = submission.Title;
                data.Prefix = submission.Prefix;
                data.SubmissionDate = (DateTime)submission.SysDate;
                data.AbstractText = submission.Abstract;
                data.CommentsForEditor = submission.CommentsForEditor;
                data.MiniDescription = submission.MiniDescription;
                data.Photo = submission.CoverPhoto;
                data.SourcesOfFunding = submission.SourcesOfFunding;
                data.PublishDate = (DateTime)submission.PublishedDate;
                data.isTopReader =(bool) submission.IsTopReader;
                data.ArticleTypeId = (long)submission.ArticleTypeId;
                data.SpecialitiesId = (long)submission.SpecialitiesId;
                data.isEditorsPick = (bool)submission.IsEditorsPick;
                issue = _IssueAccessor.Get((long)submission.IssueId);
                data.IssueNO = issue.IssuePrintNo;
                data.Banner = submission.BannerImage;
                newsletter = _NewsletterAccessor.Get(issue.NewsletterId);
                data.Volume = newsletter.Volume;
                data.Issn = newsletter.ISSN;
                data.Year = (DateTime)newsletter.PublishDate;
                data.yearstring = ((DateTime)newsletter.PublishDate).ToString("yyyy");
                data.UserId = submission.UserId;
                data.Date = ((DateTime)submission.PublishedDate).ToString("MMM.dd.yyyy");
                issue = _IssueAccessor.Get((long)submission.IssueId);
                if (submission.ArticleTypeId != null) {
                    articletype = _ArticleTypeAccessor.Get((long)submission.ArticleTypeId);
                    data.Type = articletype.Title;
                }
                if (submission.SpecialitiesId != null)
                {
                    specility = _SpecialitiesAccessor.Get((long)submission.SpecialitiesId);
                    data.Specialit = specility.Title;
                }
                //stage
                SubmissionKeywords = _SubmissionKeywordsAccessor.GetList(submission.Id);
                Submissionfile = _SubmissionFilesAccessor.Get(submission.Id, false, false, false);
                if (Submissionfile != null)
                {
                    data.FileName = Submissionfile.FileName;
                }
                if (SubmissionKeywords.Count != 0 && SubmissionKeywords != null)
                {

                    List<string> _SubmissionKeywords = new List<string>();
                    foreach (SubmissionKeyword item in SubmissionKeywords)
                    {
                        _SubmissionKeywords.Add(item.keywords);
                    }

                    data.SubmissionKeywords = _SubmissionKeywords;
                }

               
                review = _ReviewAccessor.GetList(submission.Id);
                List<ReviewLO> reviews = new List<ReviewLO>();
                List<star> maxstars=new List<star>();
                Dictionary<int, int> max = new Dictionary<int, int>();
                if (review.Count != 0 && review != null)
                {
                    max = _ReviewAccessor.getmax(submission.Id);
                    foreach (KeyValuePair<int,int> g in max)
                    {

                        maxstars.Add(new star
                        {
                            stars = g.Key,
                            nbstars = g.Value,
                        });
                    }
                    reviews = new List<ReviewLO>();
                
                    foreach (Review item in review)
                    {
                        reviews.Add(new ReviewLO
                        {
                            Id=item.Id,
                            FName= item.FName,
                            LName=item.LName,
                            Text = item.Text,
                            NbOfStars = (int)item.NbOfStars,
                            Date = (DateTime)item.SysDate,
                        });
                    }

                }
                data.Reviews = reviews;
                data.MaxStars = maxstars;

                StudyTypeLO studyType = new StudyTypeLO();
                studyType.Id = (long)submission.StudyTypeId;
                studyType.Name = "static";

                data.StudyType = studyType;

                ArticleTypeLO articleType = new ArticleTypeLO();
                articleType.Id = (long)submission.ArticleTypeId;
                articleType.Name = "static";

                data.ArticleType = articleType;
                UserLO author = new UserLO();
                if (!isBlinded)
                {
                    //get the author
                    #region Author
                    User userModel = new User();
                    userModel = _UserAccessor.Get(submission.UserId);
                    if (userModel == null)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Please try again later";
                        response.ServerMessage = "no author";
                        return response;
                    }
                   
                    author.FirstName = userModel.FirstName;
                    author.LastName = userModel.LastName;
                    author.Email = userModel.Email;

                    data.Author = author;
                    #endregion

                    //get contributores
                    #region Contributors

                    List<Contributor> contributorsModel = _ContributorsAccessor.GetList(submission.Id, false);
                    if (contributorsModel.Count != 0 && contributorsModel != null)
                    {
                        List<UserLO> contirbutors = new List<UserLO>();
                        foreach (Contributor item in contributorsModel)
                        {
                            //get the user
                            if (item.UserId != null)
                            {
                                userModel = new User();
                                userModel = _UserAccessor.Get((long)item.UserId);
                                contirbutors.Add(new UserLO
                                {
                                    FirstName = userModel.FirstName,
                                    LastName = userModel.LastName,
                                    Email = userModel.Email,
                                    isCorresponding = (bool)item.isCorresponding,
                                });
                            }
                            else
                            {
                                contirbutors.Add(new UserLO
                                {
                                    FirstName = item.Fname,
                                    LastName = item.Lname,
                                    Email = item.Email,
                                    isCorresponding = (bool)item.isCorresponding
                                });
                            }





                        }

                        data.Contributors = contirbutors;

                    }
                    #endregion
                    #region Tags
                    User user = new User();
                   
                    List<Contributor> TagsModel = _ContributorsAccessor.GetList(submission.Id, true);
                        if (TagsModel.Count != 0 && TagsModel != null)
                        {
                            tags = new List<UserLO>();
                            foreach (Contributor item in TagsModel)
                            {
                            user=_UserAccessor.Get((long)item.UserId);
                            tags.Add(new UserLO
                            {
                                Id =(long) item.UserId,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email
                            });

                        }

                        

                    }
                    data.Tags = tags;
                    #endregion

                }

                data.AuthorInfo = author.FirstName + " " + author.LastName;
               response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;
            }
            catch (Exception ex)
            {
                DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }



        private DynamicResponse<SubmissionLO> GetSubmissionPartsHelper(long submissionId, bool isSubmission, bool isReview, bool isCopyEditing,bool isProofReading)
        {
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            SubmissionLO data = new SubmissionLO();

            #region Logic
            DiscussionLogic _DisucssionLogic = new DiscussionLogic();
            #endregion

            try
            {
                //get submission files
                #region Files 
                DynamicResponse<List<SubmissionFilesLO>> submissionFiles = GetFiles(submissionId, isSubmission, isReview, isCopyEditing);

                if (submissionFiles.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = submissionFiles.HttpStatusCode;
                    response.Message = submissionFiles.Message;
                    response.ServerMessage = submissionFiles.ServerMessage;

                    return response;
                }
                else
                {
                    data.SubmissionFiles = submissionFiles.Data;
                }

                #endregion

                //get discussions
                #region Discussion
                DynamicResponse<List<DiscussionLO>> preReviewDiscussions = new DynamicResponse<List<DiscussionLO>>();

                preReviewDiscussions = _DisucssionLogic.GetListOfBasicDiscussionsForSubmission(submissionId, isSubmission, isReview, isCopyEditing, isProofReading);
                if (preReviewDiscussions.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = preReviewDiscussions.HttpStatusCode;
                    response.Message = preReviewDiscussions.Message;
                    response.ServerMessage = preReviewDiscussions.ServerMessage;

                    return response;
                }
                else
                {
                    data.PreReviewDiscussion = preReviewDiscussions.Data;
                    data.ProofReadingDiscussion = preReviewDiscussions.Data;
                    data.ReviewDiscussion = preReviewDiscussions.Data;
                }
                #endregion
                if (isProofReading)
                {
                    //get reviewers
                    DynamicResponse<List<UserLO>> ProofReaders = GetResponsbileUsersForSubmissionPerRole(submissionId, Roles.proofreading);

                    if (ProofReaders.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = ProofReaders.HttpStatusCode;
                        response.Message = ProofReaders.Message;
                        response.ServerMessage = ProofReaders.ServerMessage;

                        return response;
                    }

                    data.ProofReaders = ProofReaders.Data;
                }

                if (isReview)
                {
                    //get reviewers
                    DynamicResponse<List<UserLO>> reviewers = GetResponsbileUsersForSubmissionPerRole(submissionId, Roles.reviewer);

                    if (reviewers.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = reviewers.HttpStatusCode;
                        response.Message = reviewers.Message;
                        response.ServerMessage = reviewers.ServerMessage;

                        return response;
                    }

                    data.Reviewers = reviewers.Data;
                }


                if (isCopyEditing)
                {
                    //get reviewers
                    DynamicResponse<List<UserLO>> copyeditors = GetResponsbileUsersForSubmissionPerRole(submissionId, Roles.copyediting);

                    if (copyeditors.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = copyeditors.HttpStatusCode;
                        response.Message = copyeditors.Message;
                        response.ServerMessage = copyeditors.ServerMessage;

                        return response;
                    }

                    data.CopyEditors = copyeditors.Data;
                }


                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;

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


        private DynamicResponse<SubmissionLO> GetSubmissionPart(long submissionId)
        {
            return GetSubmissionPartsHelper(submissionId, true , false, false,false);
        }


        private DynamicResponse<SubmissionLO> GetReviewPart(long submissionId)
        {
            return GetSubmissionPartsHelper(submissionId, false, true, false, false);
        }


        private DynamicResponse<SubmissionLO> GetCopyEditingPart(long submissionId)
        {
            return GetSubmissionPartsHelper(submissionId, false, false, true, false);
        }


        private DynamicResponse<SubmissionLO> GetProofReaderPart(long submissionId)
        {
            return GetSubmissionPartsHelper(submissionId, false, false, false,true);
        }
        

        private DynamicResponse<List<SubmissionFilesLO>> GetGalleys(long submissionId)
        {
            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();

            #region Accessors
            GalleyAccessor _GalleyAccessor = new GalleyAccessor();
            #endregion

            #region Logic
            FileTypeLogic _FileTypeLogic = new FileTypeLogic();
            #endregion
            try
            {

                List<Galley> galleysFiles = _GalleyAccessor.GetList(submissionId);

                List<SubmissionFilesLO> files = new List<SubmissionFilesLO>();

                foreach (Galley item in galleysFiles)
                {
                    files.Add(new SubmissionFilesLO
                    {
                        Id = item.Id,
                        Name = item.FileName,
                        TypeId = (long)item.TypeId,
                        TypeName = _FileTypeLogic.GetFileType((long)item.TypeId)
                    });
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = files;
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


        public DynamicResponse<List<SubmissionFilesLO>> GetFiles(long submissionId, bool isSubmission, bool isRevision, bool isCopyEdited)
        {
            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();

            #region Accessors
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            #endregion

            #region Logic
            FileTypeLogic _FileTypeLogic = new FileTypeLogic();
            #endregion
            try
            {

                List<SubmissionFile> submissionFiles = _SubmissionFilesAccessor.GetList(submissionId, isSubmission, isRevision, isCopyEdited);

                List<SubmissionFilesLO> files = new List<SubmissionFilesLO>();

                foreach (SubmissionFile item in submissionFiles)
                {
                    files.Add(new SubmissionFilesLO
                    {
                        Id = item.Id,
                        Name = item.FileName,
                        TypeId = (long)item.ComponentId,
                        TypeName = _FileTypeLogic.GetFileType((long)item.ComponentId)
                    });
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = files;
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
        

        public DynamicResponse<List<SubmissionFilesLO>> GetAcceptanceFiles(long submissionId, bool isAcceptedforReview, bool isAcceptedforCopyEditing, bool isAcceptedforProduction)
        {
            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();

            #region Accessors
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            #endregion

            #region Logic
            FileTypeLogic _FileTypeLogic = new FileTypeLogic();
            #endregion
            try
            {

                List<SubmissionFile> submissionFiles = _SubmissionFilesAccessor.GetAcceptanceFiles(submissionId, isAcceptedforReview, isAcceptedforCopyEditing, isAcceptedforProduction);

                List<SubmissionFilesLO> files = new List<SubmissionFilesLO>();

                foreach (SubmissionFile item in submissionFiles)
                {
                    files.Add(new SubmissionFilesLO
                    {
                        Id = item.Id,
                        Name = item.FileName,
                        TypeId = (long)item.ComponentId,
                        TypeName = _FileTypeLogic.GetFileType((long)item.ComponentId)
                    });
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = files;
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


        #endregion


        /// <summary>
        /// returns the whole submission 
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public DynamicResponse<SubmissionLO> GetSubmission(long submissionId, bool isBlinded, long roleId)
        {
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            SubmissionLO data = new SubmissionLO();
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #region Accessor
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            try
            {
                Submission submission = _SubmissionAccessor.Get(submissionId);
                //check submission
                if (submission == null)
                {
                    response.HttpStatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Check the submission.";
                    response.ServerMessage = "there is no submission with this id";
                    return response;
                }



                //get basic info
                #region Basic info
                DynamicResponse<SubmissionLO> submissionLo = GetSubmissionBasicInfo(submission, isBlinded);
                if (submissionLo.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = submissionLo.HttpStatusCode;
                    response.Message = submissionLo.Message;
                    response.ServerMessage = submissionLo.ServerMessage;

                    return response;
                }
                else
                {
                    //set the basic info
                    data = submissionLo.Data;
                }
                #endregion

                #region Submission info (submission files and pre-review discussion)
                DynamicResponse<SubmissionLO> submissionLO = GetSubmissionPart(submissionId);
                if (submissionLO.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = submissionLO.HttpStatusCode;
                    response.Message = submissionLO.Message;
                    response.ServerMessage = submissionLO.ServerMessage;

                    return response;
                }
                else
                {
                    //set the basic info
                    data.PreReviewDiscussion = submissionLO.Data.PreReviewDiscussion;
                    data.SubmissionFiles = submissionLO.Data.SubmissionFiles;
                }
                #endregion

                #region review info (review files and review discussion)
                DynamicResponse<SubmissionLO> review = GetReviewPart(submissionId);
                if (review.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = review.HttpStatusCode;
                    response.Message = review.Message;
                    response.ServerMessage = review.ServerMessage;

                    return response;
                }
                else
                {
                    //set the basic info
                    data.ReviewDiscussion = review.Data.PreReviewDiscussion;
                    data.RevisionFiles = review.Data.SubmissionFiles;
                    data.Reviewers = review.Data.Reviewers;

                }
                #endregion

                #region  ProofReading info (ProofReading  and ProofReading discussion)
                DynamicResponse<SubmissionLO> proofreading = GetProofReaderPart(submissionId);
                if (proofreading.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = proofreading.HttpStatusCode;
                    response.Message = proofreading.Message;
                    response.ServerMessage = proofreading.ServerMessage;

                    return response;
                }
                else
                {
                    //set the basic info
                    data.ProofReadingDiscussion = proofreading.Data.PreReviewDiscussion;
                    data.ProofReaders = proofreading.Data.ProofReaders;

                }
                #endregion


                

                #region copyedited info (copyedited files and copyedited discussion)
                DynamicResponse<SubmissionLO> copyedited = GetCopyEditingPart(submissionId);
                if (copyedited.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = copyedited.HttpStatusCode;
                    response.Message = copyedited.Message;
                    response.ServerMessage = copyedited.ServerMessage;

                    return response;
                }
                else
                {
                    //set the basic info
                    data.CopyEditors = copyedited.Data.CopyEditors;
                    data.CopyEditedDiscussion = copyedited.Data.PreReviewDiscussion;
                    data.CopyEditiedFiles = copyedited.Data.CopyEditiedFiles;
                }
                #endregion

                #region Next process buttons

                #region Logic
                ProcessLogic _ProcessLogic = new ProcessLogic();
                #endregion

                DynamicResponse<List<ProcessLO>> nextProcesses = new DynamicResponse<List<ProcessLO>>();
                nextProcesses = _ProcessLogic.GetNextProcessForSubmission(submissionId, roleId);

                if (nextProcesses.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = nextProcesses.HttpStatusCode;
                    response.Message = nextProcesses.Message;
                    response.ServerMessage = nextProcesses.ServerMessage;

                    return response;
                }
                else
                {
                    data.NextProcesses = nextProcesses.Data;
                }
                #endregion
                DynamicResponse<List<SubmissionFilesLO>> galleys = new DynamicResponse<List<SubmissionFilesLO>>();
                galleys = _SubmissionLogic.GetGallys(submissionId);

                if (galleys.HttpStatusCode == HttpStatusCode.NoContent) {
                    data.Galleys = null;
                }else if (galleys.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = galleys.HttpStatusCode;
                    response.Message = galleys.Message;
                    response.ServerMessage = galleys.ServerMessage;

                    return response;
                }
                else
                {
                    data.Galleys = galleys.Data;
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = data;
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


        
   public DynamicResponse<List<SubmissionLO>> GetListArticleBasicInfo(List<long> submissionIds, bool isBlinded)
        {
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            try
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                List<SubmissionLO> data = new List<SubmissionLO>();


                List<Submission> submissions = _SubmissionAccessor.GetList(submissionIds);


                DynamicResponse<SubmissionLO> submissionResponse = new DynamicResponse<SubmissionLO>();
                foreach (Submission item in submissions)
                {
                    submissionResponse = new DynamicResponse<SubmissionLO>();
                    submissionResponse = GetArticleBasicInfo(item, isBlinded);

                    if (submissionResponse.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = submissionResponse.HttpStatusCode;
                        response.Message = submissionResponse.Message;
                        response.ServerMessage = submissionResponse.ServerMessage;

                        return response;
                    }
                    else
                    {
                        data.Add(submissionResponse.Data);
                    }
                }


                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;



            }
            catch (Exception ex)
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }

        }



        public DynamicResponse<List<SubmissionLO>> GetListBasicInfo(List<long> submissionIds, bool isBlinded)
        {
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            try
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                List<SubmissionLO> data = new List<SubmissionLO>();


                List<Submission> submissions = _SubmissionAccessor.GetList(submissionIds);


                DynamicResponse<SubmissionLO> submissionResponse = new DynamicResponse<SubmissionLO>();
                foreach (Submission item in submissions)
                {
                    submissionResponse = new DynamicResponse<SubmissionLO>();
                    submissionResponse = GetSubmissionBasicInfo(item, isBlinded);

                    if (submissionResponse.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = submissionResponse.HttpStatusCode;
                        response.Message = submissionResponse.Message;
                        response.ServerMessage = submissionResponse.ServerMessage;

                        return response;
                    }
                    else
                    {
                        data.Add(submissionResponse.Data);
                    }
                }


                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;



            }
            catch (Exception ex)
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }

        }

        public DynamicResponse<SelectLO> GetOption()
        {
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            NewsletterAccessor _NewsletterAccessor = new NewsletterAccessor();
            IssueAccessor _IssueAccessor = new IssueAccessor();
            SpecialitiesAccessor _SpecialitiesAccessor = new SpecialitiesAccessor();
            ArticleTypeAccessor _ArticleTypeAccessor = new ArticleTypeAccessor();
            #endregion
            #region Login   
            UserLogic _UserLogic = new UserLogic();
            #endregion
            DynamicResponse<List<UserLO>> userresponse = new DynamicResponse<List<UserLO>>();
            DynamicResponse<SelectLO> response = new DynamicResponse<SelectLO>();
            SelectLO  data = new SelectLO();
            List<Options> options = new List<Options>();
            List<ArticleType> articletypes = new List<ArticleType>();
            List<Newsletter> newsletters = new List<Newsletter>();
            List<Submission> submissions = new List<Submission>();
            List<Issue> issues = new List<Issue>();
            List<UserLO> users = new List<UserLO>();
            List<Speciality> speciality = new List<Speciality>();
            
            try
            {

               

                newsletters = _NewsletterAccessor.GetList();

                foreach (Newsletter item in newsletters)
                {

                    options.Add(new Options
                        {
                            Id = item.Id,
                            Value = item.Volume
                        });

                }

                data.Volumes = options;

                 issues = _IssueAccessor.GetList();
                options = new List<Options>();
                foreach (Issue item in issues)
                {

                    options.Add(new Options
                    {
                        Id = item.Id,
                        Value = item.IssuePrintNo
                    });

                }

                data.Issues = options;

                submissions = _SubmissionAccessor.GetPublishedSubmissionList();
                options = new List<Options>();
                foreach (Submission item in submissions)
                {

                    options.Add(new Options
                    {
                        Id = item.Id,
                        Value = item.PublishedDate.ToString()
                    }); ;

                }

                data.IssuesDate = options;
                

                speciality = _SpecialitiesAccessor.GetList();
                options = new List<Options>();
                foreach (Speciality item in speciality)
                {

                    options.Add(new Options
                    {
                        Id = item.Id,
                        Value = item.Title
                    });

                }
                data.Category = options;
               
                    userresponse = _UserLogic.GetUserByRole("author");
                    users = userresponse.Data;
                options = new List<Options>();
                foreach (UserLO item in users)
                    {

                        options.Add(new Options
                        {
                            Id = item.Id,
                            Value = item.FirstName + " " + item.LastName
                        });

                    }

                data.Authors = options;


                articletypes = _ArticleTypeAccessor.GetList();
                List<SelectValues>  List = new List<SelectValues>();
                foreach (ArticleType item in articletypes)
                {
                    List.Add(new SelectValues
                    {
                        Id = item.Id,
                        Value = item.Title
                    });
                }

                data.ArticleType = List;

                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

        public DynamicResponse<List<Options>> GetArticlesType(List<long> Ids)
        {
            #region Accessors
            ArticleTypeAccessor _ArticleTypeAccessor = new ArticleTypeAccessor();
            #endregion
            DynamicResponse<List<Options>> response = new DynamicResponse<List<Options>>();
            List<ArticleType> articles = new List<ArticleType>();
            List<Options> options = new List<Options>();

            try
            {



                articles = _ArticleTypeAccessor.GetList(Ids);

                foreach (ArticleType item in articles)
                {

                    options.Add(new Options
                    {
                        Id = item.Id,
                        Value = item.Title
                    });

                }


                response.Data = options;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }


        public DynamicResponse<List<Options>> GetArticlesType()
        {
            #region Accessors
            ArticleTypeAccessor _ArticleTypeAccessor = new ArticleTypeAccessor();
            #endregion
            DynamicResponse<List<Options>> response = new DynamicResponse<List<Options>> ();
            List<ArticleType> articles = new List<ArticleType>();
            List<Options> options = new List<Options>();

            try
            {



                articles = _ArticleTypeAccessor.GetList();

                foreach (ArticleType item in articles)
                {

                    options.Add(new Options
                    {
                        Id = item.Id,
                        Value = item.Title
                    });

                }


                response.Data = options;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

        

        public DynamicResponse<List<SubmissionLO>> SearchArticle(long submissionid, long issueid, long volumeid, long articletype, long author, long sectionid,string issuetitle)
        {
            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            try
            {
                submission = _SubmissionAccessor.GetList(submissionid, issueid, volumeid, articletype, author, sectionid, issuetitle);
                if (submission == null && submission.Count == 0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }


                List<long> submissionids = submission.Select(s => (long)s.Id).ToList();

                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
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

        
        public DynamicResponse<IssueLO> GetIssueInfo(long issueid)
        {
            DynamicResponse<IssueLO> response = new DynamicResponse<IssueLO>();
            #region Accessors
            IssueAccessor _IssueAccessor = new IssueAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            Issue _Issue = new Issue();
            IssueLO issue = new IssueLO();
            try
            {


                _Issue = _IssueAccessor.Get(issueid);
                if (_Issue == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "issue not found";
                }
                issue.Id = _Issue.Id;
                issue.IssueNo = (long)_Issue.IssueNo;
                issue.CoverImage = _Issue.CoverImage;
                issue.Date = (DateTime)_Issue.SysDate;
                issue.Title = _Issue.Title;
                issue.SubTitle = _Issue.SubTitle;
                issue.IssuePrintNo = _Issue.IssuePrintNo;
                response.Data = issue;
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

        public DynamicResponse<List<SubmissionLO>> GetArticles(List<long> Ids)
        {
            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            try
            {
                submission = _SubmissionAccessor.GetArticles(Ids);
                if (submission == null && submission.Count == 0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no articles ";

                    return response;
                }


                List<long> submissionids = submission.Select(s => (long)s.Id).ToList();

                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
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

        public DynamicResponse<List<SubmissionLO>> ArticlesByIssueId(long issueid)
        {
            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            try
            {
                submission = _SubmissionAccessor.GetArticlesByIssue(issueid);
                if (submission == null && submission.Count == 0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }


                List<long> submissionids = submission.Select(s => (long)s.Id).ToList();

                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
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

        public DynamicResponse<List<SubmissionLO>> GetAllArticles(long articletypeid)
        {
            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            try
            {
                submission = _SubmissionAccessor.GetArticles(articletypeid);
                if (submission == null && submission.Count == 0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }


                List<long> submissionids = submission.Select(s => (long)s.Id).ToList();

                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
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

        public DynamicResponse<List<SubmissionLO>> GetAllArticles()
        {
            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            try
            {
                submission = _SubmissionAccessor.GetArticles();
                if (submission == null && submission.Count==0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }


                List<long> submissionids = submission.Select(s => (long)s.Id).ToList();

                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
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


        public DynamicResponse<List<SubmissionLO>> GetArticle(List<long> articleids)
        {
            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            List<Submission> submission = new List<Submission>();
            try
            {
                submission = _SubmissionAccessor.GetArticleList(articleids);
                if (submission == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }

                List<long> submissionids = submission.Select(s => (long)s.Id).ToList();
                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
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


        public DynamicResponse<SubmissionLO> GetArticle(long articleid)
        {
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            #endregion
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            Submission submission = new Submission();
            try
            {
                submission=_SubmissionAccessor.GetArticle(articleid);
                if (submission== null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "no roles for user";

                    return response;
                }
                
                    response = _SubmissionLogic.GetArticleBasicInfo(submission,false);
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




        public DynamicResponse<List<SubmissionLO>> GetArticleBasicInfo(List<Submission> submissions, bool isBlinded)
        {
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            try
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                List<SubmissionLO> data = new List<SubmissionLO>();


                DynamicResponse<SubmissionLO> submissionResponse = new DynamicResponse<SubmissionLO>();
                foreach (Submission item in submissions)
                {
                    submissionResponse = new DynamicResponse<SubmissionLO>();
                    submissionResponse = GetArticleBasicInfo(item, isBlinded);

                    if (submissionResponse.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = submissionResponse.HttpStatusCode;
                        response.Message = submissionResponse.Message;
                        response.ServerMessage = submissionResponse.ServerMessage;

                        return response;
                    }
                    else
                    {
                        data.Add(submissionResponse.Data);
                    }
                }


                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;



            }
            catch (Exception ex)
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }

        }

        public DynamicResponse<List<SubmissionLO>> GetListBasicInfo(List<Submission> submissions, bool isBlinded)
        {
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            try
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                List<SubmissionLO> data = new List<SubmissionLO>();


                DynamicResponse<SubmissionLO> submissionResponse = new DynamicResponse<SubmissionLO>();
                foreach (Submission item in submissions)
                {
                    submissionResponse = new DynamicResponse<SubmissionLO>();
                    submissionResponse = GetSubmissionBasicInfo(item, isBlinded);

                    if (submissionResponse.HttpStatusCode != HttpStatusCode.OK)
                    {
                        response.HttpStatusCode = submissionResponse.HttpStatusCode;
                        response.Message = submissionResponse.Message;
                        response.ServerMessage = submissionResponse.ServerMessage;

                        return response;
                    }
                    else
                    {
                        data.Add(submissionResponse.Data);
                    }
                }


                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;



            }
            catch (Exception ex)
            {
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }

        }
        

        public DynamicResponse<List<Submission>> GetUnArchivedSubmissionsForAuthor(long userId, List<long> sectionIds)
        {

            DynamicResponse<List<Submission>> response = new DynamicResponse<List<Submission>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            SubmissionStatusAccessor _SubmissionStatusAccessor = new SubmissionStatusAccessor();
            #endregion
            try
            {
                List<Submission> submissions = _SubmissionAccessor.GetListByUserId(userId);
                List<SubmissionStatu> status = _SubmissionStatusAccessor.GetList(userId, true, false);

                List<long> archivedSubmissionIds = status.Select(s => (long)s.SuibmissionId).ToList();

                submissions = submissions.Where(e => archivedSubmissionIds.Contains(e.Id) == false
                && sectionIds.Contains((long)e.SectionId) == true).ToList();

                response.Data = submissions;
                response.HttpStatusCode = HttpStatusCode.OK;

                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

        public DynamicResponse<SelectLO> GetSelect()
        {

            DynamicResponse<SelectLO> response = new DynamicResponse<SelectLO>();
            SelectLO data = new SelectLO();

            #region Accessor
            QuestionAccessor _QuestionAccessor = new QuestionAccessor();
            ArticleTypeAccessor _ArticleTypeAccessor = new ArticleTypeAccessor();
            ResearchAccessor _ResearchAccessor = new ResearchAccessor();
            SubjectAccessor _SubjectAccessor = new SubjectAccessor();
            RequirmentAccessor _RequirmentAccessor = new RequirmentAccessor();
            #endregion
            List<Question> _Questions = new List<Question>();
            List<Requirment> _Requirment = new List<Requirment>();
            List<ArticleType> _ArticleTypes = new List<ArticleType>();
            List<Research> _Researches = new List<Research>();
            List<Research> _SubResearches = new List<Research>();
            List<Research> _SubSubResearches = new List<Research>();
            List<Subject> _Subjects = new List<Subject>();
            List<Subject> _SubSubjects = new List<Subject>();

            try
            {

                _Requirment = _RequirmentAccessor.GetList();
                List<SelectValues> List = new List<SelectValues>();
                foreach (Requirment item in _Requirment)
                {
                    List.Add(new SelectValues
                    {
                        Id = item.Id,
                        Value = item.Title
                    });
                }
                data.Requirments = List;

                _Questions =_QuestionAccessor.GetList();
                 List = new List<SelectValues>();
                foreach (Question item in _Questions)
                {
                    List.Add(new SelectValues
                    {
                        Id = item.Id,
                        Value = item.Title
                    });
                }
                data.Questions = List;

                _ArticleTypes = _ArticleTypeAccessor.GetList();
                List = new List<SelectValues>();
                foreach (ArticleType item in _ArticleTypes)
                {
                    List.Add(new SelectValues
                    {
                        Id = item.Id,
                        Value = item.Title
                    });
                }
                data.ArticleType = List;


                _Subjects = _SubjectAccessor.GetList();
                List = new List<SelectValues>();
                List<SubSelectValues>  ListSub = new List<SubSelectValues>();

                foreach (Subject item in _Subjects)
                {
                    _SubSubjects=new List<Subject>();
                    ListSub = new List<SubSelectValues>();
                    _SubSubjects = _SubjectAccessor.GetListSub(item.Id);
                    foreach (Subject subitem in _SubSubjects)
                    {
                        ListSub.Add(new SubSelectValues
                        {
                            Id = subitem.Id,
                            Value = subitem.Title

                        });
                    }
                    
                    List.Add(new SelectValues
                    {
                        Id = item.Id,
                        Value = item.Title,
                        SubSelectValues=ListSub
                    });
                }

                data.Subjects = List;



                _Researches = _ResearchAccessor.GetList();
                List = new List<SelectValues>();
                ListSub = new List<SubSelectValues>();
                List<SubSubSelectValues>  ListSubSub = new List<SubSubSelectValues>();
                foreach (Research item in _Researches)
                {
                    _SubResearches = new List<Research>();
                    ListSub= new List<SubSelectValues>();
                    _SubResearches = _ResearchAccessor.GetListSub(item.Id);
                    foreach (Research subitem in _SubResearches)
                    {
                        _SubSubResearches = new List<Research>();
                        ListSubSub = new List<SubSubSelectValues>();
                        _SubSubResearches = _ResearchAccessor.GetListSubSub(subitem.Id);
                        foreach (Research subsubitem in _SubSubResearches)
                        {
                            ListSubSub.Add(new SubSubSelectValues
                            {
                                Id = (long)subsubitem.Id,
                                Value = subsubitem.Title

                            });
                        }
                        ListSub.Add(new SubSelectValues
                        {
                            Id = subitem.Id,
                            Value = subitem.Title,
                            SubSubSelectValues = ListSubSub
                        });
                    }
                    List.Add(new SelectValues
                    {
                        Id = item.Id,
                        Value = item.Title,
                        SubSelectValues = ListSub
                    });
                }
                data.Researches = List;


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


        public DynamicResponse<List<SubmissionFilesLO>> GetGallys(long submissionId)
        {
            #region Accessors
            GalleyAccessor _GalleyAccessor = new GalleyAccessor();
            #endregion
            SubmissionFilesLO Galley = new SubmissionFilesLO();
            List<SubmissionFilesLO> GallysList = new List<SubmissionFilesLO>();
            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();
            try
            {

                List<Galley> Galleys = _GalleyAccessor.GetList(submissionId);

                if (Galleys.Count == 0 || Galleys == null)
                {
                    response.HttpStatusCode = HttpStatusCode.NoContent;
                    response.Message = "Check the Galleys files.";
                    response.ServerMessage = "Galleys can't finded";
                    return response;
                }


                foreach (Galley item in Galleys)
                {
                    Galley = new SubmissionFilesLO();
                    Galley.Name = item.FileName;
                    Galley.TypeId = (long)item.TypeId;
                    GallysList.Add(Galley);
                }


                response.Data = GallysList;
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

        public DynamicResponse<NewsletterLO> GetNewsLetter(long id)
        {
            #region Accessors
            NewsletterAccessor _NewsletterAccessor = new NewsletterAccessor();
            #endregion
            NewsletterLO Newsletter = new NewsletterLO();
            DynamicResponse<NewsletterLO> response = new DynamicResponse<NewsletterLO>();
            try
            {

                Newsletter Newsletters = _NewsletterAccessor.Get(id);

                if (Newsletters == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the Newsletters.";
                    response.ServerMessage = "Newsletters can't finded";
                    return response;
                }


            
                    Newsletter = new NewsletterLO();
                    Newsletter.Id = Newsletters.Id;
                    Newsletter.Name = Newsletters.Name;
                    Newsletter.Issn = Newsletters.ISSN;
                    Newsletter.Eissn = Newsletters.EISSN;
                    Newsletter.Volume = Newsletters.Volume;
                    Newsletter.Image = Newsletters.CoverImage;
                    Newsletter.PublishDate = (DateTime)Newsletters.PublishDate;
               

                response.Data = Newsletter;
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


        public DynamicResponse<List<NewsletterLO>> GetNewsLetter()
        {
            #region Accessors
            NewsletterAccessor _NewsletterAccessor = new NewsletterAccessor();
            #endregion
            NewsletterLO Newsletter = new NewsletterLO();
            List<NewsletterLO> NewsletterList = new List<NewsletterLO>();
            DynamicResponse<List<NewsletterLO>> response = new DynamicResponse<List<NewsletterLO>>();
            try
            {

                List<Newsletter> Newsletters = _NewsletterAccessor.GetList();

                if (Newsletters.Count == 0 || Newsletters == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the Newsletters.";
                    response.ServerMessage = "Newsletters can't finded";
                    return response;
                }


                foreach (Newsletter item in Newsletters)
                {
                    Newsletter = new NewsletterLO();
                    Newsletter.Id = item.Id;
                    Newsletter.Name = item.Name;
                    Newsletter.Issn = item.ISSN;
                    Newsletter.Eissn = item.EISSN;
                    Newsletter.Volume = item.Volume;
                    Newsletter.Image = item.CoverImage;
                    Newsletter.PublishDate = (DateTime)item.PublishDate;
                    NewsletterList.Add(Newsletter);
                }


                response.Data = NewsletterList;
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

        /// <summary>
        /// only for author
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// 


        public DynamicResponse<List<IssueLO>> GetAllIssues()
        {

            DynamicResponse<List<IssueLO>> response = new DynamicResponse<List<IssueLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            IssueAccessor _IssueAccessor = new IssueAccessor();
            #endregion
            List<Issue> _Issue = new List<Issue>();
            List<IssueLO> _Issues = new List<IssueLO>();
            IssueLO issue = new IssueLO();
            try
            {


                _Issue = _IssueAccessor.GetList();
                if (_Issue == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "issue not found";
                }

                for (int i = 0; i < _Issue.Count; i++) {
                    issue = new IssueLO();
                    issue.Id = _Issue[i].Id;
                    issue.IssuePrintNo = _Issue[i].IssuePrintNo;
                    issue.IssueNo = (long)_Issue[i].IssueNo;
                    issue.CoverImage = _Issue[i].CoverImage;
                    issue.Date = (DateTime)_Issue[i].SysDate;
                    issue.Title = _Issue[i].Title;
                    issue.SubTitle = _Issue[i].SubTitle;
                    _Issues.Add(issue);
                }
           
                
                response.Data = _Issues;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

        public DynamicResponse<IssueLO> GetLatestIssues()
        {

            DynamicResponse<IssueLO> response = new DynamicResponse<IssueLO>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            IssueAccessor _IssueAccessor = new IssueAccessor();
            #endregion
            Issue _Issue = new Issue();
            IssueLO issue = new IssueLO();
            try
            {


                _Issue = _IssueAccessor.GetLatestIssue();
                if (_Issue == null) {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "issue not found";
                }
                issue.Id = _Issue.Id;
                issue.IssuePrintNo = _Issue.IssuePrintNo;
                issue.IssueNo = (long)_Issue.IssueNo;
                issue.CoverImage = _Issue.CoverImage;
                issue.Date = (DateTime)_Issue.SysDate;
                issue.Title = _Issue.Title;
                issue.SubTitle = _Issue.SubTitle;

                response.Data = issue;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }
        
       public DynamicResponse<List<string>> GetSubmissionFieldName()
        {

            DynamicResponse<List<string>> response = new DynamicResponse<List<string>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion

            List<string> data = new List<string>();
            try
            {


                data = _SubmissionAccessor.GetSubmissionFieldName();

                if (data.Count==0) {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "field name unable to get in submiaaion table";
                }

                
                response.Data = data;
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }
        public DynamicResponse<List<SubmissionLO>> GetSubmissionLatestArticles(int limit)
        {

            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            IssueAccessor _IssueAccessor = new IssueAccessor();
            #endregion
            Issue _Issue = new Issue();
            List<Issue> _Issues = new List<Issue>();
            try
            {


                _Issue = _IssueAccessor.GetLatestIssue();
                _Issues= _IssueAccessor.GetList((long)_Issue.IssueNo);
                List<long> _IssuesIds = _Issues.Select(s => (long)s.Id).ToList();
                List<Submission> submissions = _SubmissionAccessor.GetListByIssueId(_IssuesIds, limit);
                List<long> submissionids = submissions.Select(s => (long)s.Id).ToList();
                response = _SubmissionLogic.GetListArticleBasicInfo(submissionids, false);
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

      public DynamicResponse<List<SubmissionLO>> GetRelatedIssues(long issueid,int limit)
        {

            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            IssueAccessor _IssueAccessor = new IssueAccessor();

            #endregion
            Submission _Submission = new Submission();
            List<Issue> _Issues = new List<Issue>();
            Issue _Issue = new Issue();
            try
            {


                _Submission = _SubmissionAccessor.Get(issueid);
                _Issue = _IssueAccessor.Get((long)_Submission.IssueId);
                _Issues = _IssueAccessor.GetList((long)_Issue.IssueNo);
                List<long> _IssuesIds = _Issues.Select(s => (long)s.Id).ToList();
                List<Submission> submissions = _SubmissionAccessor.GetListByIssueId(_IssuesIds, limit);
                response = GetArticleBasicInfo(submissions, true);
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

        public DynamicResponse<List<SubmissionLO>> GetUnAssignedSubmissions(long userId)
        {

            DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();
            SubmissionInProcess _SubmissionInProcess = new SubmissionInProcess();
            List<Submission> _UnAssignedSubmission = new List<Submission>();
            #region Accessors
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            UserRolesInJournalAccessor _UserRolesInJournalAccessor = new UserRolesInJournalAccessor();
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            #endregion
            UserRolesInJournal _UserRolesInJournal = new UserRolesInJournal();
            try
            {
              

                _UserRolesInJournal = _UserRolesInJournalAccessor.Get(userId);
                List<Submission> submissions = _SubmissionAccessor.GetListBySectionId((long)_UserRolesInJournal.SectionId);

                for (int i = 0; i < submissions.Count(); i++)
                {
                    _SubmissionInProcess = _SubmissionInProcessAccessor.GetLast(submissions[i].Id);
                    if (_SubmissionInProcess == null)
                    {
                        _UnAssignedSubmission.Add(submissions[i]);
                    }

                }
                response = GetListBasicInfo(_UnAssignedSubmission, true); 
                response.HttpStatusCode = HttpStatusCode.OK;
                return response;

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                response.Message = "Please try again later!";
                return response;
            }
        }

        #region Add
        public DynamicResponse<SubmissionLO> AddSubmission(SubmissionLO submission)
        {

            //SubmissionTable-SubmissionFiles-SubmissionContributors-
            //3 Accessors subA- SubFileA- ContributorA- 
            #region Accessor
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            ContributorsAccessor _ContributorsAccessor = new ContributorsAccessor();
            SubmissionKeywordsAccessor _SubmissionKeywordsAccessor = new SubmissionKeywordsAccessor();
            UserAccessor _UserAccessor = new UserAccessor();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();


            Submission _Submission = new Submission();
            List<SubmissionFile> _SubmissionFiles = new List<SubmissionFile>();
            SubmissionFile _SubmissionFile = new SubmissionFile();
            List<Contributor> _Contributors = new List<Contributor>();
            Contributor _Contributor = new Contributor();
            List<SubmissionKeyword> _SubmissionKeywords = new List<SubmissionKeyword>();
            SubmissionKeyword _SubmissionKeyword = new SubmissionKeyword();
            _Submission.UserId = submission.UserId;
            _Submission.SectionId = 1;
            _Submission.CommentsForEditor = submission.CommentsForEditor;
            _Submission.Prefix = submission.Prefix;
            _Submission.Title = submission.Title;
            _Submission.Subtitle = submission.SubTitle;
            _Submission.Abstract = submission.AbstractText;
            _Submission.ArticleTypeId = submission.ArticleTypeId;
            _Submission.StudyTypeId = 1;
            _Submission.ResearchId = submission.ResearchId;
            _Submission.SubjectId = submission.SubjectId;
            _Submission.QuestionId = submission.QuestionId;
            _Submission.IssueId = null;
            _Submission.isDeleted = false;
            _Submission.IsEditorsPick = false;
            _Submission.IsTopReader = false;
            _Submission.SysDate = DateTime.Now;
            _Submission.GUID = null;
            _Submission.LibraryId = null;
            _Submission.MiniDescription = submission.MiniDescription;
            _Submission.Significance = submission.Significance;
            _Submission.SourcesOfFunding = submission.SourcesOfFunding;
            _Submission.ConflictsOfInterests = submission.ConflictsOfInterests;
            _Submission.CoverPhoto= submission.Photo;
            _Submission.isDraft = true;
            _Submission.isApproved = submission.isApproved;
            _Submission.BannerImage = submission.Banner;

            try
            {
                if (submission.Id == null) 
                _Submission = _SubmissionAccessor.Add(_Submission);
                else
                _Submission = _SubmissionAccessor.Edit(_Submission);

                if (_Submission == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission.";
                    response.ServerMessage = "submission can't be added";
                    return response;
                }

                if (submission.ArticleComponentId!=null) {
                    for (int i = 0; i < submission.ArticleComponentId.Count(); i++)
                    {
                        _SubmissionFile = new SubmissionFile();
                        _SubmissionFile.SubmissionId = _Submission.Id;
                        _SubmissionFile.FileName = submission.FilesToUpload[i].FileName;
                        _SubmissionFile.Caption = submission.Caption[i];
                        _SubmissionFile.isDeleted = false;
                        _SubmissionFile.UserId = _Submission.UserId;
                        _SubmissionFile.ComponentId = long.Parse(submission.ArticleComponentId[i]);
                        _SubmissionFile.isSubmission = true;
                        _SubmissionFile.isAcceptedforCopyEditing = false;
                        _SubmissionFile.isAcceptedforProduction = false;
                        _SubmissionFile.isCopyedited = false;
                        _SubmissionFile.isRevision = false;
                        _SubmissionFile.isAcceptedforReview = false;
                        _SubmissionFile.SysDate = DateTime.Now;
                        _SubmissionFiles.Add(_SubmissionFile);
                    }

                    _SubmissionFiles = _SubmissionFilesAccessor.Edit(_Submission.Id);
                    if (_SubmissionFiles == null || _SubmissionFiles.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the submission files.";
                        response.ServerMessage = "submission files can't deleted";
                        return response;
                    }
                    _SubmissionFiles = _SubmissionFilesAccessor.Add(_SubmissionFiles);
                    if (_SubmissionFiles == null || _SubmissionFiles.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the submission files.";
                        response.ServerMessage = "submission files can't added";
                        return response;
                    }
                }
                if ( submission.Keywords!=null) {
                    for (int i = 0; i < submission.Keywords.Count(); i++)
                    {
                        _SubmissionKeyword = new SubmissionKeyword();
                        _SubmissionKeyword.SubmissionId = _Submission.Id;
                        _SubmissionKeyword.keywords = submission.Keywords[i];
                        _SubmissionKeyword.SysDate = DateTime.Now;
                        _SubmissionKeyword.isDeleted = false;

                        _SubmissionKeywords.Add(_SubmissionKeyword);

                    }
                    _SubmissionKeywords = _SubmissionKeywordsAccessor.Edit(_Submission.Id);

                    if (_SubmissionKeywords == null || _SubmissionKeywords.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the Keywords.";
                        response.ServerMessage = "Keywords can't deleted.";
                        return response;
                    }
                    _SubmissionKeywords = _SubmissionKeywordsAccessor.Add(_SubmissionKeywords);


                    if (_SubmissionKeywords == null || _SubmissionKeywords.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the Keywords.";
                        response.ServerMessage = "Keywords can't added.";
                        return response;
                    }
                }

                if (submission.ContributorFname!=null) {
                    User user = new User();
                    for (int i = 0; i < submission.ContributorFname.Count(); i++)
                    {

                        user = new User();
                        _Contributor = new Contributor();
                        user = _UserAccessor.Get(submission.ContributorEmail[i]);
                        if (user == null)
                        {
                            _Contributor.UserId = null;
                            _Contributor.SubmissionId = _Submission.Id;
                            _Contributor.Fname = submission.ContributorFname[i];
                            _Contributor.Mname = submission.ContributoMname[i];
                            _Contributor.Lname = submission.ContributorFname[i];
                            _Contributor.Email = submission.ContributorEmail[i];
                            _Contributor.Affiliation = submission.ContributorAffilation[i];
                            _Contributor.Institution = submission.Institution[i];
                            _Contributor.Degrees = submission.Degrees[i];
                            _Contributor.isCorresponding = submission.isCorresponding[i];
                            _Contributor.CityId = submission.City[i];
                            _Contributor.CountryId = submission.Country[i];
                            _Contributor.DepartmentId = submission.Department[i];
                            _Contributor.ORCID = submission.ORCID[i];
                            _Contributor.Order = i;
                            _Contributor.SysDate = DateTime.Now;
                            _Contributor.isDeleted = false;
                        }
                        else
                        {
                            _Contributor.UserId = submission.UserId;
                            _Contributor.SubmissionId = _Submission.Id;
                            _Contributor.Fname = submission.ContributorFname[i];
                            _Contributor.Mname = submission.ContributoMname[i];
                            _Contributor.Lname = submission.ContributorFname[i];
                            _Contributor.Email = submission.ContributorEmail[i];
                            _Contributor.Affiliation = submission.ContributorAffilation[i];
                            _Contributor.Institution = submission.Institution[i];
                            _Contributor.Degrees = submission.Degrees[i];
                            _Contributor.isCorresponding = submission.isCorresponding[i];
                            _Contributor.CityId = submission.City[i];
                            _Contributor.CountryId = submission.City[i];
                            _Contributor.DepartmentId = submission.City[i];
                            _Contributor.ORCID = submission.ORCID[i];
                            _Contributor.Order = i;
                            _Contributor.SysDate = DateTime.Now;
                            _Contributor.isDeleted = false;
                        }
                        _Contributors.Add(_Contributor);
                        //todo:add contributor in a list

                    }

                    _Contributors = _ContributorsAccessor.Edit(_Submission.Id);
                    if (_Contributors == null || _Contributors.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the contributers.";
                        response.ServerMessage = "contributers can't deleted.";
                        return response;
                    }
                    _Contributors = _ContributorsAccessor.Add(_Contributors);
                    //todo: add the contributor list to database

                    //check the response of the addition of the contibutors
                    if (_Contributors == null || _Contributors.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the contributers.";
                        response.ServerMessage = "contributers can't added.";
                        return response;
                    }
                }
                submission.Id = _Submission.Id;
                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = submission;
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


        public DynamicResponse<SubmissionLO> AddProcessSubmission(long submissionId, long userid, long processId)
        {
            #region Accessor
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            UserResponsibleInProcessAccessor _UserResponsibleInProcessAccessor = new UserResponsibleInProcessAccessor();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            SubmissionInProcess _SubmissionInProcess = new SubmissionInProcess();
            UserResponsibleInProcess _UserResponsibleInProcess = new UserResponsibleInProcess();
            Submission _Submission = new Submission();
            try
            {
                _SubmissionInProcess.SubmissionId = submissionId;
                _SubmissionInProcess.ProcessId = processId;
                _SubmissionInProcess.isDeleted = false;
                _SubmissionInProcess.SysDate = DateTime.Now;
                _SubmissionInProcess = _SubmissionInProcessAccessor.Add(_SubmissionInProcess);

                _UserResponsibleInProcess.SubmissionProcessId = _SubmissionInProcess.Id;
                _UserResponsibleInProcess.UserId = userid;
                _UserResponsibleInProcess.isAssignedByManager = false;
                _UserResponsibleInProcess.isDeleted = false;
                _UserResponsibleInProcess.SysDate = DateTime.Now;
                _UserResponsibleInProcess.DueDate = DateTime.Now;
                _UserResponsibleInProcess = _UserResponsibleInProcessAccessor.Add(_UserResponsibleInProcess);

                if (_UserResponsibleInProcess == null && _SubmissionInProcess == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission assign.";
                    response.ServerMessage = "submission can't be assigned";
                    return response;
                }

                _Submission = _SubmissionAccessor.Get(submissionId);
                response.HttpStatusCode = HttpStatusCode.OK;
                response = GetSubmissionBasicInfo(_Submission, true);
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


        public DynamicResponse<SubmissionLO> AddProcessSubmission(long managerid, long submissionId, List<long> usersid, long processId)
        {
            #region Accessor
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            UserResponsibleInProcessAccessor _UserResponsibleInProcessAccessor = new UserResponsibleInProcessAccessor();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            SubmissionInProcess _SubmissionInProcess = new SubmissionInProcess();
            UserResponsibleInProcess _UserResponsibleInProcess = new UserResponsibleInProcess();
            List<UserResponsibleInProcess> _UserResponsibleInProcessList = new List<UserResponsibleInProcess>();
            Submission _Submission = new Submission();
            try
            {
                _SubmissionInProcess.SubmissionId = submissionId;
                _SubmissionInProcess.ProcessId = processId;
                _SubmissionInProcess.isDeleted = false;
                _SubmissionInProcess.SysDate = DateTime.Now;
                _SubmissionInProcess = _SubmissionInProcessAccessor.Add(_SubmissionInProcess);


                for (int i = 0; i < usersid.Count(); i++)
                {
                    _UserResponsibleInProcess = new UserResponsibleInProcess();
                    _UserResponsibleInProcess.SubmissionProcessId = processId;
                    _UserResponsibleInProcess.UserId = usersid[i];
                    _UserResponsibleInProcess.isAssignedByManager = true;
                    _UserResponsibleInProcess.isDeleted = false;
                    _UserResponsibleInProcess.SysDate = DateTime.Now;
                    _UserResponsibleInProcess.DueDate = DateTime.Now;
                    _UserResponsibleInProcessList.Add(_UserResponsibleInProcess);
                }

                _UserResponsibleInProcessList = _UserResponsibleInProcessAccessor.Add(_UserResponsibleInProcessList);
                if (_UserResponsibleInProcessList.Count == 0 && _UserResponsibleInProcessList == null && _SubmissionInProcess == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission assign.";
                    response.ServerMessage = "submission can't be assigned";
                    return response;
                }

                _Submission = _SubmissionAccessor.Get(submissionId);
                response.HttpStatusCode = HttpStatusCode.OK;
                response = GetSubmissionBasicInfo(_Submission, true);
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


        public DynamicResponse<SubmissionLO> AddSubmissionFiles(List<SubmissionFilesLO> submissionfiles, long submissionid, long userid,string attr)
        {

            #region Accessor
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion

            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            List<SubmissionFile> _SubmissionFiles = new List<SubmissionFile>();
            SubmissionFile _SubmissionFile = new SubmissionFile();
            Submission _Submission = new Submission();

            try
            {

                for (int i = 0; i < submissionfiles.Count(); i++)
                {
                    _SubmissionFile = new SubmissionFile();
                    _SubmissionFile.SubmissionId = submissionid;
                    _SubmissionFile.FileName = submissionfiles[i].Name;
                    _SubmissionFile.isDeleted = false;
                    _SubmissionFile.UserId = userid;
                    _SubmissionFile.ComponentId = submissionfiles[i].TypeId;
                    if(attr== "isSubmission")
                    _SubmissionFile.isSubmission = true;
                    else
                    _SubmissionFile.isSubmission = false;
                    if (attr == "isCopyedited")
                        _SubmissionFile.isCopyedited = true;
                    else
                        _SubmissionFile.isCopyedited = false;
                    if (attr == "isRevision")
                        _SubmissionFile.isRevision = true;
                    else
                        _SubmissionFile.isRevision = false;
                    _SubmissionFile.isAcceptedforCopyEditing = false;
                    _SubmissionFile.isAcceptedforProduction = false;
                    _SubmissionFile.isAcceptedforReview = false;
                    _SubmissionFile.SysDate = DateTime.Now;
                    _SubmissionFiles.Add(_SubmissionFile);
                }
                _SubmissionFiles = _SubmissionFilesAccessor.Add(_SubmissionFiles);

                if (_SubmissionFiles == null || _SubmissionFiles.Count == 0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission files.";
                    response.ServerMessage = "submission files can't added";
                    return response;
                }

                _Submission = _SubmissionAccessor.Get(submissionid);
                response = GetSubmissionBasicInfo(_Submission, true);
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

        
              public DynamicResponse<long> EditNewsletter(NewsletterLO toEdit)
        {
            #region Accessor
            NewsletterAccessor _NewsletterAccessor = new NewsletterAccessor();
            #endregion
            long id;
            Newsletter _Newsletter = new Newsletter();
            DynamicResponse<long> response = new DynamicResponse<long>();
            try
            {
                _Newsletter.Id = toEdit.Id;
                _Newsletter.UserId = toEdit.UserId;
                _Newsletter.Name = toEdit.Name;
                _Newsletter.ISSN = toEdit.Issn;
                _Newsletter.EISSN = toEdit.Eissn;
                _Newsletter.CoverImage = toEdit.Image;
                _Newsletter.Volume = toEdit.Volume;
                _Newsletter.PublishDate = toEdit.PublishDate;
                _Newsletter.isDeleted = false;
                _Newsletter.SysDate = DateTime.Now; 
                 id = _NewsletterAccessor.Edit(_Newsletter);
                if (_Newsletter == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the newsletter.";
                    response.ServerMessage = "newsletter can't edited";
                    return response;
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = toEdit.Id;
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

        public DynamicResponse<NewsletterLO> AddNewsletter(NewsletterLO toAdd, long userid)
        {
            #region Accessor
            NewsletterAccessor _NewsletterAccessor = new NewsletterAccessor();
            #endregion
            
            Newsletter _Newsletter = new Newsletter();
            DynamicResponse<NewsletterLO> response = new DynamicResponse<NewsletterLO>();
            try
            {
                _Newsletter.Name = toAdd.Name;
                _Newsletter.ISSN = toAdd.Issn;
                _Newsletter.EISSN = toAdd.Eissn;
                _Newsletter.CoverImage = toAdd.PostedFileImage.FileName;
                _Newsletter.PublishDate = toAdd.PublishDate;
                _Newsletter.UserId = userid;
                _Newsletter.isDeleted =false;
                _Newsletter.SysDate =DateTime.Now;
                _Newsletter = _NewsletterAccessor.Add(_Newsletter);
                if (_Newsletter == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the newsletter.";
                    response.ServerMessage = "newsletter can't added";
                    return response;
                }
                
                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = toAdd;
            return response;

            }catch (Exception ex)
            {
                        response.HttpStatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Please try again later!";
                        response.ServerMessage = ex.Message;
                        return response;
                    }
           }

        public DynamicResponse<long> AddSubmissionStatu(long userid,long submissionid)
        {
            #region Accessor
            SubmissionStatusAccessor _SubmissionStatusAccessor = new SubmissionStatusAccessor();
            #endregion

            SubmissionStatu _SubmissionStatu = new SubmissionStatu();
            DynamicResponse<long> response = new DynamicResponse<long>();
            try
            {
                _SubmissionStatu.SuibmissionId = submissionid;
                _SubmissionStatu.UserId = userid;
                _SubmissionStatu.isSkip = true;
                _SubmissionStatu.isArchived = false;
                _SubmissionStatu.isReed = false;
                _SubmissionStatu.isDeleted =false;
                _SubmissionStatu.SysDate = DateTime.Now;
                _SubmissionStatu = _SubmissionStatusAccessor.Add(_SubmissionStatu);
                if (_SubmissionStatu == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission status.";
                    response.ServerMessage = "submission status can't added";
                    return response;
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = _SubmissionStatu.Id;
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

        public DynamicResponse<List<SubmissionFilesLO>> AddGalleys(List<SubmissionFilesLO> galleysfiles, long submissionid, long userid)
        {

            #region Accessor
            GalleyAccessor _GalleyAccessor = new GalleyAccessor();
            #endregion
            List<Galley> _GalleysFiles = new List<Galley>();
            Galley _Galley = new Galley();
            FileTypeLogic _FileTypeLogic = new FileTypeLogic();
            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();
            try
            {

                for (int i = 0; i < galleysfiles.Count(); i++)
                {
                    _Galley = new Galley();
                    _Galley.SubmissionId = submissionid;
                    _Galley.FileName = galleysfiles[i].Name;
                    _Galley.isDeleted = false;
                    _Galley.UserId = userid;
                    _Galley.TypeId =(int)galleysfiles[i].TypeId;
                    _Galley.SysDate = DateTime.Now;
                    _GalleysFiles.Add(_Galley);
                }
                _GalleysFiles = _GalleyAccessor.Add(_GalleysFiles);

                if (_GalleysFiles == null || _GalleysFiles.Count == 0)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission files.";
                    response.ServerMessage = "submission files can't added";
                    return response;
                }


                List<SubmissionFilesLO> files = new List<SubmissionFilesLO>();

                foreach (Galley item in _GalleysFiles)
                {
                    files.Add(new SubmissionFilesLO
                    {
                        Id = item.Id,
                        Name = item.FileName,
                        TypeId = (long)item.TypeId,
                        TypeName = _FileTypeLogic.GetFileType((long)item.TypeId)
                    });
                }

                response.HttpStatusCode = HttpStatusCode.OK;
                response.Data = files;
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

        #endregion

      

        #region Update
         public DynamicResponse<SubmissionLO> UpdateAcceptedForProcess(List<long> filesid, long submissionid,string attr)
        {
            #region Accessor
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            Submission _Submission = new Submission();
            long _fileid;
            List<long> files = new List<long>();
            try
            {
                for (int i = 0; i < filesid.Count(); i++)
                {
                    _fileid = _SubmissionFilesAccessor.EditAcceptenceForReviewer(filesid[i], attr);
                    files.Add(_fileid);
                }
                if (files.Count == 0 && files == null)
                {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission files.";
                    response.ServerMessage = "submission files can't added";
                    return response;
                }

                _Submission = _SubmissionAccessor.Get(submissionid);
                response = GetSubmissionBasicInfo(_Submission, true);
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


         public DynamicResponse<long> LinkToNewsletter(long submissionid, long newletterid)
        {
            #region Accessor
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            #endregion
            DynamicResponse<long> response = new DynamicResponse<long>();
            long newletters;
            try
            {

                newletters = _SubmissionAccessor.LinkToNewsletter(submissionid, newletterid);
                if (newletters == 0)
                {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission.";
                    response.ServerMessage = "submission Link to newsletter can't updated";
                    return response;
                }

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


        public DynamicResponse<long> UpdateSubmission(SubmissionLO submission)
        {
            #region Accessor
            SubmissionAccessor _SubmissionAccessor = new SubmissionAccessor();
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            ContributorsAccessor _ContributorsAccessor = new ContributorsAccessor();
            #endregion
            DynamicResponse<long> response = new DynamicResponse<long>();
            List<Contributor> _Contributors = new List<Contributor>();
            Submission _Submission = new Submission();
            List<SubmissionFile> files = new List<SubmissionFile>();
            long id = 0;
            _Submission.Id =(long) submission.Id;
            _Submission.UserId = submission.UserId;
            //_Submission.SectionId = 1;
            //_Submission.CommentsForEditor = submission.CommentsForEditor;
            //_Submission.Prefix = submission.Prefix;
            _Submission.Title = submission.Title;
            _Submission.Subtitle = submission.SubTitle;
            _Submission.Abstract = submission.AbstractText;
            _Submission.ArticleTypeId = submission.ArticleTypeId;
            _Submission.SpecialitiesId = submission.SpecialitiesId;
            //_Submission.StudyTypeId = 1;
            //_Submission.ResearchId = submission.ResearchId;
            //_Submission.SubjectId = submission.SubjectId;
            //_Submission.QuestionId = submission.QuestionId;
            //_Submission.IssueId = null;
            //_Submission.isDeleted = false;
            _Submission.IsEditorsPick = submission.isEditorsPick;
            _Submission.IsTopReader = submission.isTopReader;
            //_Submission.SysDate = DateTime.Now;
            //_Submission.GUID = null;
            //_Submission.LibraryId = null;
            //_Submission.MiniDescription = submission.MiniDescription;
            //_Submission.Significance = submission.Significance;
            //_Submission.SourcesOfFunding = submission.SourcesOfFunding;
            //_Submission.ConflictsOfInterests = submission.ConflictsOfInterests;
            _Submission.CoverPhoto = submission.Photo;
            //_Submission.isDraft = true;
            //_Submission.isApproved = submission.isApproved;
            _Submission.BannerImage = submission.Banner;
            try
            {


                id = _SubmissionAccessor.Update(_Submission);
                      if (id == 0)
                {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the submission.";
                    response.ServerMessage = "submission can't updated";
                    return response;
                }
                if (submission.FileName != null) {
                    SubmissionFile file = new SubmissionFile();
                    file.SubmissionId = submission.Id;
                    file.FileName = submission.FileName;
                    file.isAcceptedforProduction = true;
                    file.isDeleted = false;
                    file.SysDate = DateTime.Now;
                    files.Add(file);
                    files=_SubmissionFilesAccessor.Add(files);

                    if (files.Count == 0) {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the submission files.";
                        response.ServerMessage = "files can't updated";
                        return response;
                    }

                }
               

                Contributor _Contributor = new Contributor();
                if (submission.TagsIds.Count != 0)
                {
                    for (int i = 0; i < submission.TagsIds.Count(); i++)
                    {
                        _Contributor = new Contributor();
                        _Contributor.UserId = (long)submission.TagsIds[i];
                        _Contributor.SubmissionId = (long)submission.Id;
                        _Contributor.Fname = null;
                        _Contributor.Mname = null;
                        _Contributor.Lname = null;
                        _Contributor.Email = null;
                        _Contributor.Affiliation = null;
                        _Contributor.Institution = null;
                        _Contributor.Degrees = null;
                        _Contributor.isCorresponding = null;
                        _Contributor.CityId = null;
                        _Contributor.CountryId = null;
                        _Contributor.DepartmentId = null;
                        _Contributor.ORCID = null;
                        _Contributor.Order = i;
                        _Contributor.SysDate = DateTime.Now;
                        _Contributor.isDeleted = false;
                        _Contributor.IsTag = true;

                        _Contributors.Add(_Contributor);

                    }



                    _Contributors = _ContributorsAccessor.Add(_Contributors);
                    //check the response of the addition of the contibutors
                    if (_Contributors == null || _Contributors.Count == 0)
                    {
                        response.HttpStatusCode = HttpStatusCode.InternalServerError;
                        response.Message = "Check the contributers.";
                        response.ServerMessage = "contributers can't added.";
                        return response;
                    }

                    _ContributorsAccessor.Delete(id, true);
                }
                response.Data = (long)submission.Id;
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



        public DynamicResponse<long> UpdateIssue(IssueLO issue)
        {
            #region Accessor
            IssueAccessor _IssueAccessor = new IssueAccessor();
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            ContributorsAccessor _ContributorsAccessor = new ContributorsAccessor();
            #endregion
            DynamicResponse<long> response = new DynamicResponse<long>();
            Issue _Issue = new Issue();

            long id = 0;
            _Issue.Id = (long)issue.Id;
            _Issue.CoverImage = issue.CoverImage;
            _Issue.Title = issue.Title;
            _Issue.SubTitle = issue.SubTitle;
            _Issue.IssueNo = issue.IssueNo;
            try
            {


                _Issue = _IssueAccessor.Edit(_Issue);
                if (_Issue == null)
                {

                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Check the Issue.";
                    response.ServerMessage = "Issue can't updated";
                    return response;
                }
                response.Data = (long)_Issue.Id;
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
        #endregion

    }
}