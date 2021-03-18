using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;

namespace Anz.LMJ.StartUp.Controllers
{
    public class LayoutController : Controller
    {
        // GET: Layout
        #region Services
        ContentServices _ContentServices = new ContentServices();
        HomeServices _HomeServices = new HomeServices();
        #endregion

        #region Footer
        public ActionResult Footer()
        {
          try{
                List<long> ids = new List<long>();
                DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();
                DynamicResponse<List<Options>> articlestype = new DynamicResponse<List<Options>>();
                DynamicResponse<List<SubmissionLO>> response = new DynamicResponse<List<SubmissionLO>>();

                options = _HomeServices.GetOption();
                ViewBag.options = options.Data;
                Footer footer = _ContentServices.GetContent<Footer>(ContentServices.ServiceTables.Footer, 1).Contents.FirstOrDefault();
                ViewBag.footer = footer;
                string[] arr = footer.CategoryIds.Split(',');
                foreach (string id in (arr))
                {
                    ids.Add(long.Parse(id));
                }
                articlestype = _HomeServices.GetArticlesType(ids);
                if (articlestype.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.articlestype = articlestype.Data;
                arr = footer.RecentArticleIds.Split(',');
                foreach (string id in (arr))
                {
                    ids.Add(long.Parse(id));
                }
                response = _HomeServices.GetArticles(ids);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.Issues = response.Data;
                arr = footer.ContactIds.Split(',');
                ViewBag.contact = arr;
                var html = new StringBuilder("");

                Contact contact = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
                for (int i = 0; i < arr.Length; i++)
                {
                    html.Append("<strong>" + arr[i].Trim() + "</strong>: " + Helper.GetPropValue<string>(contact, arr[i].Trim()) + "<br/>");
                }
                ViewBag.contactdata = html;
                ViewBag.contact = contact;
                List<FooterMenu> footerlinks = _ContentServices.GetContent<FooterMenu>(ContentServices.ServiceTables.FooterMenu, 9999).Contents.ToList();
                ViewBag.footerlinks = footerlinks;

                return PartialView("_PartialViewFooter", new ViewDataDictionary { new KeyValuePair<string, object>("footer", ViewBag.footer), new KeyValuePair<string, object>("footerlinks", ViewBag.footerlinks), new KeyValuePair<string, object>("contactdata", ViewBag.contactdata), new KeyValuePair<string, object>("contact", ViewBag.contact), new KeyValuePair<string, object>("options", ViewBag.options), new KeyValuePair<string, object>("articlestype", ViewBag.articlestype), new KeyValuePair<string, object>("Issues", ViewBag.Issues) });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region Header
        public ActionResult Header()
        {
            try
            {
                Contact contact = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
                ViewBag.contact = contact;
                return PartialView("_PartialViewHeader", new ViewDataDictionary { new KeyValuePair<string, object>("contact", ViewBag.contact) });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

       }
        #endregion

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