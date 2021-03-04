using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anz.LMJ.StartUp.Controllers
{
    public class DiscussionController : Controller
    {
        // GET: Discussion
        /// <summary>
        /// returns the info of a discussion
        /// </summary>
        /// <param name="id">discussion id</param>
        /// <returns></returns>        
        [HttpGet]
        public JsonResult Index(long id)
        {
            #region Services
            ManagementService _ManagementService = new ManagementService();

            #endregion
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }
    }
}