using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LookUpObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Anz.LMJ.StartUp.Controllers
{
    public class LayoutController : Controller
    {
        // GET: Layout
        #region Services
        ContentServices _ContentServices = new ContentServices();
        HomeServices _HomeServices = new HomeServices();
        #endregion
        public ActionResult Footer()
        {
            DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();
            DynamicResponse<List<Options>> articlestype = new DynamicResponse<List<Options>>();
            articlestype = _HomeServices.GetArticlesType();
            ViewBag.articlestype = articlestype.Data;
            DynamicResponse<List<SubmissionLO>> data = _HomeServices.GetSubmissionLatestArticles(4);
            ViewBag.Issues = data.Data;
            options = _HomeServices.GetOption();
            ViewBag.options = options.Data;
            return PartialView("_PartialViewFooter",new ViewDataDictionary { new KeyValuePair<string, object>("options", ViewBag.options) , new KeyValuePair<string, object>("articlestype", ViewBag.articlestype), new KeyValuePair<string, object>("Issues", ViewBag.Issues) });
        }


        #region Search Article 

        [HttpPost]
        public ActionResult SearchArticle(long submissionid, long issueid, long volumeid, long articletype, long author, long sectionid, string issuetitle)
        {

            try
            {

                DynamicResponse<List<SubmissionLO>> submission = _HomeServices.SearchArticle(submissionid, issueid, volumeid, articletype, author, sectionid, issuetitle);
                ViewBag.articles = submission.Data;
                if (submission.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

                return PartialView("~/Views/Home/_PartialViewArticles.cshtml", submission.Data);
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }
        }
        #endregion

    }
}