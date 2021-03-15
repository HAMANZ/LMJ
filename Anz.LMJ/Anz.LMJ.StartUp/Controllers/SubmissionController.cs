using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Anz.LMJ.BLO.LogicObjects;
using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.LogicObjects.Submission.Discussion;
using static Anz.LMJ.BLL.Logic.Enums;
using System.IO;
using Anz.LMJ.FrontEnd;

namespace Anz.LMJ.StartUp.Controllers
{
    public class SubmissionController : Controller
    {
        // GET: Submission
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">submission id</param>
        /// <returns></returns>


        [HttpGet]
        public ActionResult Index(long? id)
        {
            #region Services
            ManagementService _ManagementService = new ManagementService();
            #endregion

            if (Session["userId"] == null)
                return RedirectToAction("LoginForm", "Home");

            long userId = long.Parse(Session["userId"].ToString());
            

            if (id == null)
            {


                DynamicResponse<SelectLO> select = _ManagementService.GetSelect();
                ViewBag.GetSelect = select.Data;
                return View("SubmissionForm");
            }

            else
            {
                try
                {
                    DynamicResponse<UserQueueLO> queue = _ManagementService.GetQueue(userId, 1);
                    DynamicResponse<SubmissionLO> submission = _ManagementService.GetSubmission(userId, (long)id);

                    if (submission.HttpStatusCode != HttpStatusCode.OK)
                    {
                        return RedirectToAction("Index", "Oops");
                    }
                    ViewBag.queue = queue.Data;
                    return View(submission.Data);
                }


                catch (Exception ex)
                {
                    return RedirectToAction("Index", "Oops");
                }
            }
        }

        [HttpPost]
        public ActionResult AddSubmission(SubmissionLO submission)
        {
            #region Logics 
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();

            
            long userId = long.Parse(Session["userId"].ToString());
            submission.UserId = userId;
            try
            {

                string path = Server.MapPath("~/files/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int i=0; i <submission.FilesToUpload.Length; i++) {
                    if (submission.FilesToUpload[i] != null)
                    {
                        string extension = Path.GetExtension(submission.FilesToUpload[i].FileName);
                        string Name = Path.GetFileNameWithoutExtension(submission.FilesToUpload[i].FileName);
                        string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        submission.FilesToUpload[i].SaveAs(path + newName);
                    }

                }

                string path_images = Server.MapPath("~/Images/");
                if (!Directory.Exists(path_images))
                {
                    Directory.CreateDirectory(path_images);
                }

                    if (submission.CoverPhoto != null)
                    {
                        string extension = Path.GetExtension(submission.CoverPhoto.FileName);
                        string Name = Path.GetFileNameWithoutExtension(submission.CoverPhoto.FileName);
                        string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        submission.CoverPhoto.SaveAs(path_images + newName);
                    }


                response = _SubmissionLogic.AddSubmission(submission);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

                return RedirectToAction("Index", "Management", userId);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        [HttpPost]
        public JsonResult AddProcessToSubmission(long processid, long submissionid)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                response = _SubmissionLogic.AddProcessSubmission(submissionid, userId, processid);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult EditAcceptedForFiles(List<long> Filesid, long submissionid, string attr, long processid)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                response = _SubmissionLogic.UpdateAcceptedForProcess(Filesid, submissionid, attr);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        [ActionName("AddSubmissionFiles")]
        public ActionResult AddSubmissionFiles(SubmissionFilesLO files,long submissionid)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<SubmissionLO> response = new DynamicResponse<SubmissionLO>();
            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                string path = Server.MapPath("~/files/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                List<SubmissionFilesLO> filesupload = new List<SubmissionFilesLO>();

                if (files.FilesToUpload.Length != 0)
                {


                    FileTypeLogic _FileTypeLogic = new FileTypeLogic();
                    filesupload = new List<SubmissionFilesLO>();

                    for (int i = 0; i < files.TypesId.Length; i++)
                    {
                        
                        string extension = Path.GetExtension(files.FilesToUpload[i].FileName);
                        string Name = Path.GetFileNameWithoutExtension(files.FilesToUpload[i].FileName);
                        string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        files.FilesToUpload[i].SaveAs(path + newName);

                        filesupload.Add(new SubmissionFilesLO
                        {
                            Name = newName,
                            TypeId = (long)files.TypesId[i],
                            TypeName = _FileTypeLogic.GetFileType((long)files.TypesId[i])
                        });

                        
                    }

                }

              
                response = _SubmissionLogic.AddSubmissionFiles(filesupload, files.submissionid, userId, files.attr);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

                 return RedirectToAction("index", new { id = submissionid });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        [HttpPost]
        public JsonResult AddGalleys(List<SubmissionFilesLO> filestoupload, long submissionid, string attr)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();
            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                response = _SubmissionLogic.AddGalleys(filestoupload, submissionid, userId);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult AddReveiwers(List<long> userids, long submissionid, Roles role, long processidinmodel)
        {
            #region Logics
            UserLogic _UserLogic = new UserLogic();
            #endregion

            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
            long userId = long.Parse(Session["userId"].ToString());

            try
            {

                response = _UserLogic.AddReviewer(userids, submissionid, userId, role, processidinmodel);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }


        [HttpPost]
        public JsonResult GetGallys(long submissionid)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();
            try
            {
                response = _SubmissionLogic.GetGallys(submissionid);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult GetUserByRole(string role)
        {
            #region Logic
            UserLogic _UserLogic = new UserLogic();
            #endregion

            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
            try
            {
                response = _UserLogic.GetUserByRole(role);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult GetParticipant(long submissionid)
        {
            #region Logic
            UserLogic _UserLogic = new UserLogic();
            #endregion

            DynamicResponse<List<UserLO>> response = new DynamicResponse<List<UserLO>>();
            try
            {
                response = _UserLogic.GetParticipant(submissionid);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }


        [HttpPost]
        public JsonResult GetAcceptanceFiles(long submissionid, bool isAcceptedforReview, bool isAcceptedforCopyEditing, bool isAcceptedforProduction)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();
            try
            {
                response = _SubmissionLogic.GetAcceptanceFiles(submissionid, isAcceptedforReview, isAcceptedforCopyEditing, isAcceptedforProduction);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult GetFiles(long submissionid, bool isSubmission, bool isRevision, bool isCopyEdited)
        {
            #region Logic
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            DynamicResponse<List<SubmissionFilesLO>> response = new DynamicResponse<List<SubmissionFilesLO>>();
            try
            {
                response = _SubmissionLogic.GetFiles(submissionid, isSubmission, isRevision, isCopyEdited);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult AddDiscussion(DiscussionContentLO toAdd, long submissionid)
        {
            #region Logics
            DiscussionLogic _DiscussionLogic = new DiscussionLogic();
            #endregion

            DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();
            long userId = long.Parse(Session["userId"].ToString());

            try
            {
                response = _DiscussionLogic.AddDiscussionContent(toAdd, submissionid, userId);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        

        public JsonResult GetDiscussionDetail(long discussionId)
        {
            #region Logics
            DiscussionLogic _DiscussionLogic = new DiscussionLogic();
            #endregion

            try
            {


                DynamicResponse<DiscussionLO> response = new DynamicResponse<DiscussionLO>();
                response = _DiscussionLogic.GetWholeDiscussion(discussionId);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        
        [HttpPost]
        public JsonResult LinkToNewsletter(long submissionid, long newsletterid)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<long> response= new DynamicResponse<long>();
            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                response = _SubmissionLogic.LinkToNewsletter(submissionid, newsletterid);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }

        public JsonResult GetAllNewsLetter()
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            try
            {

                DynamicResponse<List<NewsletterLO>> response = new DynamicResponse<List<NewsletterLO>>();
                response = _SubmissionLogic.GetNewsLetter();
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }

        }



        public JsonResult NewsLetter()
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            try
            {

                DynamicResponse<List<NewsletterLO>> response = new DynamicResponse<List<NewsletterLO>>();
                response = _SubmissionLogic.GetNewsLetter();
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [CheckUserSession]
        public ActionResult GetNewsletter(long id)
        {
            try
            {

                #region Logics
                SubmissionLogic _SubmissionLogic = new SubmissionLogic();
                #endregion

                if (Session["userId"] == null)
                    return RedirectToAction("Index", "Admin");
                DynamicResponse<NewsletterLO> response = new DynamicResponse<NewsletterLO>();
                response = _SubmissionLogic.GetNewsLetter(id);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                return View(response.Data);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }
           
        }

        [CheckUserSession]
        public ActionResult Newsletters()
        {
            

            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion

            if (Session["userId"] == null)
                return RedirectToAction("Index", "Admin");
            try
            {
                DynamicResponse<List<NewsletterLO>> response = new DynamicResponse<List<NewsletterLO>>();
                response = _SubmissionLogic.GetNewsLetter();
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                
                return View(response.Data);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        
        [CheckUserSession]
        public ActionResult AddNewsletterForm()
        {
            try
            {
                return View("NewsletterForm");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        public ActionResult AddNewsletter(NewsletterLO toAdd)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<NewsletterLO> response = new DynamicResponse<NewsletterLO>();
           
            if (Session["userId"] == null)
                 return Json("session-null");

            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                string path_images = Server.MapPath("~/NewsletterImages/");
                if (!Directory.Exists(path_images))
                {
                    Directory.CreateDirectory(path_images);
                }

                if (toAdd.PostedFileImage != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileImage.SaveAs(path_images + newName);
                    toAdd.Image = newName;
                }
              
                response = _SubmissionLogic.AddNewsletter(toAdd, userId);
                if (response.HttpStatusCode != HttpStatusCode.OK) {
                    return RedirectToAction("Index", "Oops");
                }
                return RedirectToAction("Newsletters");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }
            
        }

        public ActionResult EditNewsletter(NewsletterLO toEdit)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<long> response = new DynamicResponse<long>();

            if (Session["userId"] == null)
                return RedirectToAction("Index", "Admin");

            long userId = long.Parse(Session["userId"].ToString());
          
            try
            {
                string path_images = Server.MapPath("~/NewsletterImages/");
                if (!Directory.Exists(path_images))
                {
                    Directory.CreateDirectory(path_images);
                }

                if (toEdit.PostedFileImage != null)
                {
                    string extension = Path.GetExtension(toEdit.PostedFileImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toEdit.PostedFileImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toEdit.PostedFileImage.SaveAs(path_images + newName);
                    toEdit.Image = newName;
                }
                toEdit.UserId = userId;
                response = _SubmissionLogic.EditNewsletter(toEdit);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                return RedirectToAction("Newsletters");
            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "Oops");
            }

        }

        public JsonResult AddSubmissionStatu(long submissionid)
        {
            #region Logics
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            #endregion
            DynamicResponse<long> response = new DynamicResponse<long>();

            if (Session["userId"] == null)
                return Json("session-null");

            long userId = long.Parse(Session["userId"].ToString());
            try
            {
                response = _SubmissionLogic.AddSubmissionStatu(userId, submissionid);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("ERROR");
            }

        }




    }
}