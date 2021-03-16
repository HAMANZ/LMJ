using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.FrontEnd;
using System.IO;

namespace Anz.LMJ.StartUp.Controllers
{
    public class ManagementController : Controller
    {
        public void setSession()
        {

            Session["userId"] = "1";
        }

        // GET: Dashboard
        public ActionResult Index()
        {
           
            #region Services
            ManagementService _ManagementService = new ManagementService();
            #endregion
            try
            {
                long userId = long.Parse(Session["userId"].ToString());
                if (Session["userId"] == null)
                    return RedirectToAction("Index", "Admin");
                DynamicResponse<UserQueueLO> queue = _ManagementService.GetQueue(userId, 1);
                DynamicResponse<List<SubmissionLO>> unassigned = _ManagementService.GetUnAssigned(userId);
                

                if (queue.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

                ViewBag.unassigned = unassigned.Data;
                ViewBag.queue = queue.Data;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("");
            }

        }



        [CheckUserSession]
        public ActionResult MainMenu()
        {
            try
            {
                return View("MainMenu");
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

        #region management


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
        #endregion
        #region Return Json
        [ActionName("unassign")]
        [HttpGet]
        /// <summary>
        /// list of unassiged submission that needs editors.
        /// only returns submission when the user is editor or author
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        public JsonResult UnAssignedSubmissionsNeedEditors(long id)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }


        [ActionName("unreview")]
        [HttpGet]
        /// <summary>
        /// list of unreviewed submission that needs reviewers
        /// only returns submission when the user is reviewer or author
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        public JsonResult UnReviewedSubmissionsNeedReviewers(long id)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }




        #endregion

    }
}