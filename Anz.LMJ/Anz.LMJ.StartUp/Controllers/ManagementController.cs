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