using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.Review;
using Anz.LMJ.BLO.LogicObjects.Issue;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.WebServices;
using Aspose.Pdf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static Anz.LMJ.BLL.Logic.Enums;
using Syroot.Windows.IO;
using System.Xml.Linq;
using System.Text;

namespace Anz.LMJ.StartUp.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Home Page
        /// </summary>
        /// <returns></returns>
        #region Services
        ContentServices _ContentServices = new ContentServices();
        HomeServices _HomeServices = new HomeServices();
        AdminServices _AdminServices = new AdminServices();
        #endregion



        public ActionResult Index()
        {   
            DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();
            options = _HomeServices.GetOption();
            ViewBag.options = options.Data;
            Hero_Banner banner = _ContentServices.GetContent<Hero_Banner>(ContentServices.ServiceTables.Hero_Banner, 1).Contents.FirstOrDefault();
            ViewBag.Banner = banner;
            DynamicResponse<List<SubmissionLO>> response = _HomeServices.GetSubmissionLatestArticles(3);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Oops");
            }
            ViewBag.latestarticles = response.Data;
            DynamicResponse<IssueLO> latestissues = _HomeServices.GetLatestIssues();
            if (latestissues.HttpStatusCode != HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Oops");
            }
            ViewBag.latestIssues = latestissues.Data;
            DynamicResponse<List<IssueLO>> allissues = _HomeServices.GetAllIssues();
            if (allissues.HttpStatusCode != HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Oops");
            }
            ViewBag.allissues = allissues.Data.OrderByDescending(e => e.Date).ToList(); ;
            IssueFilter issuefilter = new IssueFilter();
            issuefilter = _ContentServices.GetContent<IssueFilter>(ContentServices.ServiceTables.IssueFilter, 1).Contents.FirstOrDefault();
            ViewBag.issuefilter = issuefilter;
            List<Events> ourevents = _ContentServices.GetContent<Events>(ContentServices.ServiceTables.Events, 3).Contents.ToList();
            ourevents = ourevents.OrderBy(e => DateTime.Parse(e.EventDate)).ToList();
            ViewBag.Events = ourevents;
            List<Videos> ourvideos = _ContentServices.GetContent<Videos>(ContentServices.ServiceTables.Videos, 4).Contents.ToList();
            ourvideos = ourvideos.OrderBy(e => DateTime.ParseExact(e.VideoDate, "dd-MM-yyyy",CultureInfo.InvariantCulture)).ToList();
            ViewBag.Videos = ourvideos;
            DynamicResponse<List<SubmissionLO>> submission = _HomeServices.GetAllArticles();
            if (submission.HttpStatusCode != HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Oops");
            }
            ViewBag.articles = submission.Data;
            return View(options);
        }

        #region footerlinks
        public ActionResult FooterLink(long id)
        {
            try
            {
                FooterMenu page = _ContentServices.GetContentOfItem<FooterMenu>(ContentServices.ServiceTables.FooterMenu, 1, id).Contents.FirstOrDefault();
                ViewBag.page = page;
                return View("FooterPage");
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region VideoDetail

        public ActionResult VideoDetail(long id)
        {
           
            try
            {
                List<Videos> ourvideos = _ContentServices.GetContent<Videos>(ContentServices.ServiceTables.Videos,3).Contents.ToList();
                ourvideos = ourvideos.OrderBy(e => DateTime.ParseExact(e.VideoDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList();
                ViewBag.Videos = ourvideos;
                Videos videos = _ContentServices.GetContentOfItem<Videos>(ContentServices.ServiceTables.Videos, 1, id).Contents.FirstOrDefault();
                ViewBag.detail = videos;

                return View();
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region About


      
        public ActionResult About()
        {
           try
            {
                
                About_Page about = _ContentServices.GetContent<About_Page>(ContentServices.ServiceTables.About_Page, 1).Contents.FirstOrDefault();
                ViewBag.about = about;
                return View();
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region  EditorialBoards

        public ActionResult EditorialBoards()
        {
            try
            {
                EditorialBoard editorialBoards = _ContentServices.GetContent<EditorialBoard>(ContentServices.ServiceTables.EditorialBoard, 1).Contents.FirstOrDefault();
                ViewBag.editorialBoards = editorialBoards;
                List<DataType> degrees = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Degree, 9999).Contents.ToList();
                ViewBag.degrees = degrees;
                List<DataType> position = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Position, 9999).Contents.ToList();
                ViewBag.position = position;
                DynamicResponse<List<DataType>> response = _AdminServices.GetRoles();
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.roles = response.Data;


                List<int> ids = new List<int>();
                List<string> roles = new List<string>();
                roles.Add(Roles.management.ToString());
                ids = _AdminServices.GetRoles(roles);
                List<UserLO> users = _AdminServices.GetUsers(ids);
                ViewBag.teams = users;

                ids = new List<int>();
                roles = new List<string>();
                roles.Add(Roles.member.ToString());
                ids = _AdminServices.GetRoles(roles);
                 users = _AdminServices.GetUsers(ids);
                ViewBag.member = users;
                return View();
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

       
        #region contactus   
        public ActionResult Contact()
        {
             try
            {
                Contact contact = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
                ViewBag.contact = contact;
                return View();
            }catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        public ActionResult ContactUs(Contactus c)
        {
            try
            {
                Contact contact = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
                c.Email = "hudaabumayha.ham@gmail.com";
                _HomeServices.ContactUs(c);
                return View("Contact");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }
        
        #endregion

        #region EventDetail

        public ActionResult EventDetail(long id)
        {
           try
            {
                List<Events> ourevents = _ContentServices.GetContent<Events>(ContentServices.ServiceTables.Events, 3).Contents.ToList();
                ourevents = ourevents.OrderBy(e => DateTime.Parse(e.EventDate)).ToList();
                ViewBag.Events = ourevents;
                Events events = _ContentServices.GetContentOfItem<Events>(ContentServices.ServiceTables.Events, 1, id).Contents.FirstOrDefault();
                ViewBag.detail = events;
                
                return View();
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region Article Detail

        public ActionResult ArticleDetails(long id)
        {
            
           
            try
            {

                DynamicResponse<SubmissionLO> submission = _HomeServices.GetArticle((long)id);
                ViewBag.detail = submission.Data;
                if (submission.Data.MaxStars.Count != 0) {
                    ViewBag.rating = (double)GetRating(submission.Data.MaxStars[0].nbstars, submission.Data.MaxStars[0].nbstars, submission.Data.MaxStars[0].nbstars, submission.Data.MaxStars[0].nbstars, submission.Data.MaxStars[0].nbstars);
                 }
                List<CitationType> citation = new List<CitationType>();
                citation = _ContentServices.GetContent<CitationType>(ContentServices.ServiceTables.Citation, 9999).Contents.ToList();
                ViewBag.CitationType = citation;
                DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();
                options = _HomeServices.GetOption();
                ViewBag.options = options.Data;
                DynamicResponse<List<SubmissionLO>> data = _HomeServices.GetRelatedIssues(id,3);
                ViewBag.relatedissue = data.Data;
                UserLogic _UserLogic = new UserLogic();
                DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
                return View("ArticleDetail");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region Search Article 

        [HttpPost]
        public ActionResult SearchArticle(long submissionid, long issueid, long volumeid, long articletype, long author, long sectionid,string issuetitle)
        {

            try
            {

                DynamicResponse<List<SubmissionLO>> submission = _HomeServices.SearchArticle(submissionid, issueid, volumeid, articletype, author, sectionid, issuetitle);
               
                if (submission.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.articles = submission.Data;
                return PartialView("_PartialViewArticles", submission.Data);
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }
        }

        #endregion

        #region Articles
        [HttpGet]
        public ActionResult Articles(long? id)
        {
            DynamicResponse<List<SubmissionLO>> submission = new DynamicResponse<List<SubmissionLO>>();
            DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();

           
           
            try
            {
                if (id == null)
                {
                    submission = _HomeServices.GetAllArticles();
                }
                else {
                    submission = _HomeServices.GetAllArticles((long)id);
                }
               
                options = _HomeServices.GetOption();
                ViewBag.articles = submission.Data.OrderByDescending(e => DateTime.Parse(e.PublishDate.ToString())).ToList();
                ViewBag.options = options.Data;
            
              
                if (submission.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                return View();
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        public ActionResult ArticlesByIssueId(long IssueId)
        {
            DynamicResponse<List<SubmissionLO>> submission = new DynamicResponse<List<SubmissionLO>>();
            DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();
            DynamicResponse<IssueLO> response = new DynamicResponse<IssueLO>();
          

            try
            {
                submission = _HomeServices.ArticlesByIssueId((long)IssueId);
                ViewBag.articles = submission.Data.OrderByDescending(e => DateTime.Parse(e.PublishDate.ToString())).ToList();
                response = _HomeServices.GetIssueInfo((long)IssueId);
                ViewBag.issuedata = response.Data;

                if (submission.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                return View("Articles");
            }


            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }

        #endregion

        #region Login
        public ActionResult LoginForm()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(string email, string password, string view)
        {
            try
            {
                UserLogic _UserLogic = new UserLogic();
                DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
                UserLO user = new UserLO();

                response = _HomeServices.Auth(email, password);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("LoginForm", "Home");
                }

                user = response.Data;
                if (user != null)
                {
                    
                    Session["userId"] = user.Id;
                    //check roles
                    Session["isAuthor"] = _UserLogic.CheckRole(user.Id, Roles.author, 1);
                    Session["isReviewer"] = _UserLogic.CheckRole(user.Id, Roles.reviewer, 1);
                    Session["isEditor"] = _UserLogic.CheckRole(user.Id, Roles.editor, 1);
                    Session["isCopyediting"] = _UserLogic.CheckRole(user.Id, Roles.copyediting, 1);
                    Session["isProofreading"] = _UserLogic.CheckRole(user.Id, Roles.proofreading, 1);
                    //the action maybe the home page or the dashboard page
                    if (view == "true")
                        return RedirectToAction("Index", "Management", user.Id);
                    else
                        return RedirectToAction("Index", "Home", user.Id);
                }

                return RedirectToAction("LoginForm", "Home", user.Id);
            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "Oops");
            }
        }
        #endregion


        public JsonResult savefile(string fileName) {
            string name=fileName.Split('.')[0];
            string path = Server.MapPath("~/files/");
            string htmlfile = path + name + ".html";
            string pdffile = path + name + ".pdf";
            string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path +"/"+ name + ".html";
            Document pdfDocument = new Document(pdffile);
            if (!System.IO.File.Exists(htmlfile))
             {
               
                pdfDocument.Save(htmlfile, SaveFormat.Html);
                pdfDocument.Save(downloadsPath, SaveFormat.Html);
                return Json(htmlfile);
            }

            pdfDocument.Save(downloadsPath, SaveFormat.Html);
            return Json(htmlfile);
        }

        public XDocument generateXml(long submissionid)
        {
            DynamicResponse<SubmissionLO> submission = _HomeServices.GetArticle((long)submissionid);
            XDocument srcTree = new XDocument(
             new XComment("This is a comment"),
             new XElement("Root",
                 new XElement("Title", submission.Data.Title),
                 new XElement("SubTitle", submission.Data.SubTitle),
                 new XElement("Author", submission.Data.Author.FirstName + " " + submission.Data.Author.LastName),
                 new XElement("PublishDate", submission.Data.PublishDate.ToString()),
                 new XElement("ArticleType", submission.Data.Type),
                 new XElement("Category", submission.Data.Specialit)

        ));
            //if (submission.Data.Tags.Count != 0) {

            //    srcTree.Add(new XElement("TagsInfo",
            //         submission.Data.Tags.Select(item =>
            //         new XElement("Tags",
            //         new XElement("FirstName", item.FirstName),
            //         new XElement("LastName", item.LastName)))));
            //}

            XDocument doc = new XDocument(
                new XComment("This is a comment"),
                new XElement("Root",
                    from el in srcTree.Element("Root").Elements()
                    select el
                )
            );
            return doc;
        }

            public JsonResult SaveXml(string fileName,long articleid)
        {
            string name = fileName.Split('.')[0];
            XDocument xmlfile = generateXml(articleid);
            string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path + "/" + name+".xml";
            xmlfile.Save(downloadsPath);

            return Json("error");
        }

        public static double GetRating(int star1, int star2, int star3, int star4, int star5)
        {
           

            double rating = (double)(5 * star5 + 4 * star4 + 3 * star3 + 2 * star2 + 1 * star1) / (star1 + star2 + star3 + star4 + star5);

            rating = Math.Round(rating, 1);

            return rating;
        }

        public JsonResult GetCitationForm(long id,long articleid)
        {
            try
            {
                if (id == 0) {
                    return Json("error");
                }
                CitationType data = new CitationType();
              
                data = _ContentServices.GetContentOfItem<CitationType>(ContentServices.ServiceTables.Citation, 1, id).Contents.FirstOrDefault();
                string source = data.Text;
                List<string> attrname = source.EverythingBetween("[[[", "]]]");
                List<string> attrvalue = new List<string>();

                
                DynamicResponse<SubmissionLO> submission = _HomeServices.GetArticle((long)articleid);
               
                string value = "";
                var t = source;
                //StringBuilder s = StringBuilder(source);
                for (int i=0; i<attrname.Count;i++)
                {
                    value = Helper.GetPropValue<string>(submission.Data, attrname[i]);
                    source = source.Replace("[[["+ attrname [i]+ "]]]", value);
                }

                return Json(source);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }


        

        public JsonResult AddReviewByUser(ReviewLO toAdd)
        {


            try
            {

                DynamicResponse<long> response = _HomeServices.AddReviewByUser(toAdd);

                return Json(response, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                return Json("error");
            }

        }

        public string FindTextBetween(string text, string left, string right)
        {
            // TODO: Validate input arguments

            int beginIndex = text.IndexOf(left); // find occurence of left delimiter
            if (beginIndex == -1)
                return string.Empty; // or throw exception?

            beginIndex += left.Length;

            int endIndex = text.IndexOf(right, beginIndex); // find occurence of right delimiter
            if (endIndex == -1)
                return string.Empty; // or throw exception?

            return text.Substring(beginIndex, endIndex - beginIndex).Trim();
        }
        #region Logout
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                return RedirectToAction("LoginForm");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }
        }

        #endregion



    }
}