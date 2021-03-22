using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.Issue;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.BLO.LogicObjects.Review;
using Anz.LMJ.FrontEnd;
using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static Anz.LMJ.BLL.Logic.Enums;
using DataType = Anz.LMJ.BLO.ContentObjects.DataType;

namespace Anz.LMJ.StartUp.Controllers
{
    public class AdminController : Controller
    {
        #region Services
        ContentServices _ContentServices = new ContentServices();
        HomeServices _HomeServices = new HomeServices();
        LoggerServices _LoggerServices = new LoggerServices();
        AdminServices _AdminServices = new AdminServices();
        ManagementService _ManagementService = new ManagementService();
        #endregion

        [HttpGet]
        /// <summary>
        /// login form
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("ViewLogin");
        }

        #region Login 

        public ActionResult Login()
        {
            try
            {
                return View("ViewLogin");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult SignIn(string email, string password, string view)
        {
            try
            {

                long userid;
                Dictionary<string, string> result = _AdminServices.AuthIsAdmin(email, password);

                if (result["userid"] != "null")
                {
                    userid = long.Parse(result["userid"]);
                    Session["userId"] = userid;
                    //check roles
                    Session["isAuthor"] = _AdminServices.CheckRole(userid, Roles.author, 1);
                    Session["isReviewer"] = _AdminServices.CheckRole(userid, Roles.reviewer, 1);
                    Session["isEditor"] = _AdminServices.CheckRole(userid, Roles.editor, 1);
                    Session["isCopyediting"] = _AdminServices.CheckRole(userid, Roles.copyediting, 1);
                    Session["isProofreading"] = _AdminServices.CheckRole(userid, Roles.proofreading, 1);
                    //the action maybe the home page or the dashboard page
                    if (view == "true")
                        return RedirectToAction("Index", "Management", userid);
                    else
                    {
                        if (result["isadmin"] == "True")
                            return RedirectToAction("HomeBanner");
                        else return RedirectToAction("Login", "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:SignIn", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("SignIn", LoggerServices.ActionTypes.Read, "email:" + email + " password:" + password, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return RedirectToAction("Login");

        }

        #endregion

        #region IssueFilter

        [HttpGet]
        [CheckUserSession]
        public ActionResult IssueFilter()
        {
            IssueFilter issuefilter = new IssueFilter();
            try
            {
                issuefilter = _ContentServices.GetContent<IssueFilter>(ContentServices.ServiceTables.IssueFilter, 1).Contents.FirstOrDefault();

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:IssueFilter", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("IssueFilter", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewIssueFilter", issuefilter);
        }

        [HttpPost]
        [CheckUserSession]
        public ActionResult IssueFilter(IssueFilter toAdd)
        {
            string json = "";

            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { CFromDate = toAdd.CurrentFromDate, CToDate = toAdd.CurrentToDate,
                    RFromDate = toAdd.RecentFromDate,
                    RToDate = toAdd.RecentToDate,
                    AFromDate = toAdd.ArchiveFromDate,
                    AToDate = toAdd.ArchiveToDate,
                    IFromDate = toAdd.IndexFromDate,
                    IToDate = toAdd.IndexToDate
                });

                _AdminServices.AddContent<IssueFilter>(toAdd, AdminServices.ServiceAdminTables.IssueFilter);


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:HomeBanner", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("ViewIssueFilter", LoggerServices.ActionTypes.Add, "home banner inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewIssueFilter", toAdd);
        }

        #endregion

        #region HomeBanner

        [HttpGet]
        [CheckUserSession]
        public ActionResult HomeBanner()
        {
            Hero_Banner banner = new Hero_Banner();
            try
            {
                banner = _ContentServices.GetContent<Hero_Banner>(ContentServices.ServiceTables.Hero_Banner, 1).Contents.FirstOrDefault();

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:HomeBanner", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("HomeBanner", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(banner);
        }


        [HttpPost]
        [CheckUserSession]
        public ActionResult HomeBanner(Hero_Banner toAdd)
        {
            string json = "";

            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { title = toAdd.Title, subtitle = toAdd.SubTitle });
                string path = Server.MapPath("~/Images/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //List of images

                List<string> imgs = new List<string>();


                if (toAdd.PostedFileBackgroundImage != null)
                {
                    for (int j = 0; j < toAdd.PostedFileBackgroundImage.Count; j++)
                    {
                        if (toAdd.PostedFileBackgroundImage[j] != null)
                        {
                            string fileName = Path.GetFileName(toAdd.PostedFileBackgroundImage[j].FileName);

                            string extension = Path.GetExtension(toAdd.PostedFileBackgroundImage[j].FileName);
                            string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileBackgroundImage[j].FileName);
                            string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                            toAdd.PostedFileBackgroundImage[j].SaveAs(path + newName);
                            //toAdd.BackgroundImage[0] = newName;
                            imgs.Add(newName);
                        }
                    }
                }


                if (toAdd.BackgroundImage != null)
                {
                    for (int k = 0; k < toAdd.BackgroundImage.Count; k++)
                    {
                        if (toAdd.BackgroundImage[k] != "")
                        {
                            imgs.Add(toAdd.BackgroundImage[k]);
                        }

                    }
                }
                toAdd.BackgroundImage = imgs;

                _AdminServices.AddContent<Hero_Banner>(toAdd, AdminServices.ServiceAdminTables.Home_Banner);


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:HomeBanner", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                // _LoggerServices.Error("HomeBanner", LoggerServices.ActionTypes.Add, "home banner inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(toAdd);
        }

        #endregion

        #region Footer Sextion
        [HttpGet]
        [CheckUserSession]
        public ActionResult Footer()
        {
            Footer footerpage = new Footer();
            try
            {

                DynamicResponse<List<Options>> articlestypes = new DynamicResponse<List<Options>>();
                articlestypes = _HomeServices.GetArticlesType();
                ViewBag.articlestypes = articlestypes.Data;
                DynamicResponse<List<SubmissionLO>> submission = _HomeServices.GetAllArticles();
                if (submission.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.articles = submission.Data;
                footerpage = _ContentServices.GetContent<Footer>(ContentServices.ServiceTables.Footer, 1).Contents.FirstOrDefault();

                List<string> attrName = _ContentServices.GetAttributes(ContentServices.ServiceTables.Contact);
                ViewBag.attrName = attrName;
                Contact contact = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
                ViewBag.contact = contact;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Footer_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Footer_Page", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(footerpage);
        }



        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult Footer(Footer toAdd)
        {
            string json = "";
            Footer footerpage = new Footer();
            try
            {

                toAdd.CategoryIds = String.Join(", ", toAdd.CategoryIdss.ToArray());
                toAdd.RecentArticleIds = String.Join(", ", toAdd.RecentArticleIdss.ToArray());
                toAdd.ContactIds = String.Join(", ", toAdd.ContactIdss.ToArray());

                DynamicResponse<List<Options>> articlestypes = new DynamicResponse<List<Options>>();
                articlestypes = _HomeServices.GetArticlesType();
                ViewBag.articlestypes = articlestypes.Data;
                DynamicResponse<List<SubmissionLO>> submission = _HomeServices.GetAllArticles();
                if (submission.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.articles = submission.Data;
               

                List<string> attrName = _ContentServices.GetAttributes(ContentServices.ServiceTables.Contact);
                ViewBag.attrName = attrName;
                Contact contact = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
                ViewBag.contact = contact;
                if (toAdd.Eissn == null) {
                    toAdd.Eissn = "";
                }
                if (toAdd.Issn == null)
                {
                    toAdd.Issn = "";
                }
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    RecentArticleIds = toAdd.RecentArticleIds,
                    CategoryIds = toAdd.CategoryIds,
                    ContactIds = toAdd.ContactIds,
                    Issn=toAdd.Issn,
                    Eissn = toAdd.Eissn,
                });


                _AdminServices.AddContent<Footer>(toAdd, AdminServices.ServiceAdminTables.Footer);


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Footer", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Footer", LoggerServices.ActionTypes.Add, "inputs" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(toAdd);
        }


        #endregion

        #region Contact
        [HttpGet]
        [CheckUserSession]
        public ActionResult Contact()
        {
            Contact contactpage = new Contact();
            try
            {
                contactpage = _ContentServices.GetContent<Contact>(ContentServices.ServiceTables.Contact, 1).Contents.FirstOrDefault();
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Contact_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Contact_Page", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(contactpage);
        }



        [CheckUserSession]
        [HttpPost]
        public ActionResult Contact(Contact toAdd)
        {
            string json = "";
            try
            {
                if (toAdd.Phone2 == null) {
                    toAdd.Phone2 = "";
                }
                if (toAdd.Mobile2 == null)
                {
                    toAdd.Mobile2 = "";
                }
                if (toAdd.AdditionalEmail == null)
                {
                    toAdd.AdditionalEmail = "";
                }
                if (toAdd.Facebook == null)
                {
                    toAdd.Facebook = "";
                }
                if (toAdd.Desc == null)
                {
                    toAdd.Desc = "";
                }
                if (toAdd.Twiter == null)
                {
                    toAdd.Twiter = "";
                }
                if (toAdd.Instagram == null)
                {
                    toAdd.Instagram = "";
                }
                if (toAdd.LinkedIn == null)
                {
                    toAdd.LinkedIn = "";
                }
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Phone1 = toAdd.Phone1,
                    Phone2 = toAdd.Phone2,
                    Mobile1 = toAdd.Mobile1,
                    Mobile2 = toAdd.Mobile2,
                    Address = toAdd.Address,
                    POBOX = toAdd.POBox,
                    Email = toAdd.Email,
                    AdditionalEmail = toAdd.AdditionalEmail,
                    Facebook = toAdd.Facebook,
                    Twiter = toAdd.Twiter,
                    Desc = toAdd.Desc,
                });

              
                _AdminServices.AddContent<Contact>(toAdd, AdminServices.ServiceAdminTables.Contact);


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Contact_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Contact_Page", LoggerServices.ActionTypes.Add, "inputs" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(toAdd);
        }
        #endregion

        #region AboutUs_Page

        [HttpGet]
        [CheckUserSession]
        public ActionResult AboutPage()
        {
            About_Page aboutuspage = new About_Page();
            try
            {
                About_Page n = new About_Page();
                aboutuspage = _ContentServices.GetContent<About_Page>(ContentServices.ServiceTables.About_Page, 1).Contents.FirstOrDefault();
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AboutUs_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AboutUs_Page", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(aboutuspage);
        }

        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult AboutPage(About_Page toAdd)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toAdd.Title,
                    Txt = toAdd.Txt,
                    Img = toAdd.Img,
                });

                string path = Server.MapPath("~/Images/AboutPage/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                if (toAdd.PostedFileImg != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileImg.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileImg.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileImg.SaveAs(path + newName);
                    toAdd.Img = newName;
                }



                _AdminServices.AddContent<About_Page>(toAdd, AdminServices.ServiceAdminTables.About_Page);


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AboutUs_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AboutUs_Page", LoggerServices.ActionTypes.Add, "inputs" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(toAdd);
        }

        #endregion

        #region review
        [HttpGet]
        [CheckUserSession]
        public ActionResult Review()
        {
            try
            {
                DynamicResponse<List<ReviewLO>> response = _AdminServices.GetReviews();
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                List<ReviewLO> reviews = response.Data;
                ViewBag.reviews = reviews;


            }
            catch (Exception ex)
            {
                #region Logger
                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Review", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Review", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewReview");

        }



        public ActionResult GetReview(long id)
        {
            try
            {
                DynamicResponse<ReviewLO> response = _AdminServices.GetReview(id);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ReviewLO review = response.Data;
                ViewBag.Review = review;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetReview", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetReview", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return View("GetReview");

        }


        [CheckUserSession]
        public ActionResult AdmitReview(bool IsAdmit, long Id)
        {
            DynamicResponse<long> response = new DynamicResponse<long>();
            try
            {
                response = _AdminServices.AdmitReview(IsAdmit,Id);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
               
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

            return RedirectToAction("Review");
        }

        #endregion


        #region Issues
        [HttpGet]
        [CheckUserSession]
        public ActionResult Issues()
        {
            try
            {

                DynamicResponse<List<IssueLO>> response = _AdminServices.GetAllIssues();
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.Issues = response.Data;


            }
            catch (Exception ex)
            {
                #region Logger
                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Issues", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Issues", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewIssue");

        }


        [CheckUserSession]
        public ActionResult GetIssue(long id)
        {
            try
            {
                 DynamicResponse<IssueLO> response = _AdminServices.GetIssueInfo(id);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.Issue= response.Data;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetIssue", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetIssue", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return View("GetIssue");

        }


        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditIssue(IssueLO issue)
        {
            try
            {
                DynamicResponse<long> response;
                string path = Server.MapPath("~/Images/Issues/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (issue.Photo != null)
                {
                    string extension = Path.GetExtension(issue.Photo.FileName);
                    string Name = Path.GetFileNameWithoutExtension(issue.Photo.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    issue.Photo.SaveAs(path + newName);
                    issue.CoverImage = newName;
                }

                response = _HomeServices.UpdateIssue(issue);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                return RedirectToAction("GetIssue", new { id = issue.Id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }


        #endregion

        #region article
        [HttpGet]
        [CheckUserSession]
        public ActionResult Articles()
        {
            try
            {

                List<SubmissionLO> editorspick = _AdminServices.GetAllArticles();
                ViewBag.editorspick = editorspick;


            }
            catch (Exception ex)
            {
                #region Logger
                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditorPick", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditorPick", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewArticles");

        }


        [CheckUserSession]
        public ActionResult GetArticle(long id)
        {
            try
            {
                DynamicResponse<SelectLO> options = new DynamicResponse<SelectLO>();
                options = _HomeServices.GetOption();
                ViewBag.articletype = options.Data.ArticleType;
                ViewBag.Category = options.Data.Category;
                SubmissionLO article = _AdminServices.GetArticle(id);
                ViewBag.article = article;
                List<UserLO> users = _AdminServices.GetAllUsers();
                ViewBag.users = users;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetArticle", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetArticle", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return View("GetArticle");

        }


        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditArticle(SubmissionLO submission)
        {
            try
            {
                DynamicResponse<long> response;
                string path = Server.MapPath("~/Images/Articles/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (submission.CoverPhoto != null)
                {
                    string extension = Path.GetExtension(submission.CoverPhoto.FileName);
                    string Name = Path.GetFileNameWithoutExtension(submission.CoverPhoto.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    submission.CoverPhoto.SaveAs(path + newName);
                    submission.Photo = newName;
                }
               
                if (submission.BannerImage != null)
                {
                    string extension = Path.GetExtension(submission.BannerImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(submission.BannerImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    submission.BannerImage.SaveAs(path + newName);
                    submission.Banner = newName;
                }

                path = Server.MapPath("~/files/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (submission.FilesToUpload[0]!=null)
                {
                    string extension = Path.GetExtension(submission.FilesToUpload[0].FileName);
                    string Name = Path.GetFileNameWithoutExtension(submission.FilesToUpload[0].FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    submission.FilesToUpload[0].SaveAs(path + newName);
                    submission.FileName = newName;
                }

                response = _HomeServices.UpdateSubmission(submission);
                if (response.HttpStatusCode != HttpStatusCode.OK) {
                    return RedirectToAction("Index", "Oops");
                }
                return RedirectToAction("GetArticle", new { id = submission.Id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Oops");
            }

        }


        #endregion

        #region Events 


        [CheckUserSession]
        public ActionResult AddEventsSectionForm()
        {
            try
            {
                List<Events> events = _ContentServices.GetContent<Events>(ContentServices.ServiceTables.Events, 99).Contents.ToList();
                ViewBag.Gallery = events;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddEventsSectionForm", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddEventsSectionForm", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return View("AddEvents");

        }

        [CheckUserSession]
        public ActionResult AddEvents(Events toAdd)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
                    MainImg = toAdd.MainImg,
                    SubImg = toAdd.SubImg,
                    Location =toAdd.Location,
                    MainTitle = toAdd.MainTitle,
                    MainDesc = toAdd.MainDesc, 
                    EventDate = toAdd.EventDate,
                });

                string path = Server.MapPath("~/Images/Events/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toAdd.PostedFileMainImg != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileMainImg.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileMainImg.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileMainImg.SaveAs(path + newName);
                    toAdd.MainImg = newName;
                }
                //add position
                List<Events> Events = _ContentServices.GetContent<Events>(ContentServices.ServiceTables.Events, 99).Contents.ToList();
                //edit positions for add
                bool result = _AdminServices.UpdatePosition("EVENTS_POSITION", 20, true, null);

                _AdminServices.AddContent<Events>(toAdd, AdminServices.ServiceAdminTables.Events);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddEventsSection", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion

            }
            return RedirectToAction("Events");

        }

        public ActionResult DeleteEventsSection(long id)
        {
            try
            {

               
                _AdminServices.DeleteContent(id);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteEventsSection", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return RedirectToAction("Events");

        }

        public ActionResult GetEvents(long id)
        {
            try
            {
            
                Events events = _ContentServices.GetContentOfItem<Events>(ContentServices.ServiceTables.Events, 1, id).Contents.FirstOrDefault();
                ViewBag.Events = events;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetEventsSection", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return View("GetEvents");

        }


        public ActionResult EditEventsSection(Events toedit)
        {
            string json = "";
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    MainImg = toedit.MainImg,
                    SubImg = toedit.SubImg,
                    Location = toedit.Location,
                    MainTitle = toedit.MainTitle,
                    MainDesc = toedit.MainDesc,
                    EventDate = toedit.EventDate,
                });

                string path = Server.MapPath("~/Images/Events/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                List<string> imgs = new List<string>();


                if (toedit.PostedFile != null)
                {
                    for (int j = 0; j < toedit.PostedFile.Count; j++)
                    {
                        if (toedit.PostedFile[j] != null)
                        {
                            string fileName = Path.GetFileName(toedit.PostedFile[j].FileName);

                            string extension = Path.GetExtension(toedit.PostedFile[j].FileName);
                            string Name = Path.GetFileNameWithoutExtension(toedit.PostedFile[j].FileName);
                            string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                            toedit.PostedFile[j].SaveAs(path + newName);
                            //toAdd.BackgroundImage[0] = newName;
                            imgs.Add(newName);
                        }
                    }
                }

                //PostedFileMainImg

                if (toedit.PostedFileMainImg != null)
                {

                    string fileName = Path.GetFileName(toedit.PostedFileMainImg.FileName);

                    string extension = Path.GetExtension(toedit.PostedFileMainImg.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toedit.PostedFileMainImg.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toedit.PostedFileMainImg.SaveAs(path + newName);
                    toedit.MainImg = newName;
                    imgs.Add(newName);

                }



                if (toedit.SubImg != null)
                {
                    for (int k = 0; k < toedit.SubImg.Count; k++)
                    {
                        if (toedit.SubImg[k] != "")
                        {
                            imgs.Add(toedit.SubImg[k]);
                        }
                    }
                }
                toedit.SubImg = imgs;

                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<Events>(toedit, AdminServices.ServiceAdminTables.Events);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditEventsSection", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return RedirectToAction("Events");

        }


        [CheckUserSession]
        public ActionResult Events()
        {
            try
            {
                List<Events> ourevents = _ContentServices.GetContent<Events>(ContentServices.ServiceTables.Events, 9999).Contents.ToList();
                ourevents = ourevents.OrderBy(e => DateTime.Parse(e.EventDate)).ToList();
                ViewBag.Events = ourevents;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditEventsSection", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewEvents");

        }



        #endregion

        #region EditorialBoard

        public ActionResult EditorialBoard()
        {
            EditorialBoard editorialboard = new EditorialBoard();
            try
            {
               editorialboard = _ContentServices.GetContent<EditorialBoard>(ContentServices.ServiceTables.EditorialBoard, 1).Contents.FirstOrDefault();
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditorialBoard", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditorialBoard", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(editorialboard);
        }

        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditorialBoard(EditorialBoard toAdd)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toAdd.Title,
                    Txt = toAdd.Txt,
                    Img = toAdd.Img,
                });

                string path = Server.MapPath("~/Images/EditorialBoard/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                if (toAdd.PostedFileImg != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileImg.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileImg.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileImg.SaveAs(path + newName);
                    toAdd.Img = newName;
                }



                _AdminServices.AddContent<EditorialBoard>(toAdd, AdminServices.ServiceAdminTables.EditorialBoard);


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditorialBoard", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditorialBoard", LoggerServices.ActionTypes.Add, "inputs" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(toAdd);
        }

        #endregion

        #region User

        public ActionResult GetUesr(long id)
        {
            try
            {
                List<DataType> degrees = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Degree, 9999).Contents.ToList();
                ViewBag.degrees = degrees;
                List<DataType> position = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Position, 9999).Contents.ToList();
                ViewBag.position = position;
                Team team = _ContentServices.GetContentOfItem<Team>(ContentServices.ServiceTables.Team, 1, id).Contents.FirstOrDefault();
                ViewBag.Team = team;
                DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
                response=_AdminServices.GetUser(id);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

                ViewBag.User = response.Data;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetTeam", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();

        }
        [CheckUserSession]
        public ActionResult EditUser(UserLO toedit)
        {
            try
            {
                string path = Server.MapPath("~/Images/Users/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toedit.PostedFileImage != null)
                {
                    string extension = Path.GetExtension(toedit.PostedFileImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toedit.PostedFileImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toedit.PostedFileImage.SaveAs(path + newName);
                    toedit.Image = newName;
                }


                DynamicResponse<long> response = _AdminServices.EditUser(toedit);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }


            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditUser", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditUser", LoggerServices.ActionTypes.Update, "inputs:" + toedit, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Users");

        }
        [CheckUserSession]
        [HttpGet]
        public ActionResult Users(List<string> Role)
        {
            try
            {
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
                List<long> roleids = response.Data.Select(s => s.Id).ToList();
                List<int> ids = new List<int>();
                foreach (int id in roleids) {
                    ids.Add((int)id);
                }
                List<UserLO> users = new List<UserLO>();
                if (Role != null)
                    {
                        if (Role[0] == "all")
                        {
                            users = _AdminServices.GetUsers();
                        }else
                        {
                            ids = _AdminServices.GetRoles(Role);
                            users = _AdminServices.GetUsers(ids);
                        }
                    }
                    else {
                    users = _AdminServices.GetUsers(ids);
                }
                
                ViewBag.users = users.OrderBy(e => e.Pos).ToList();
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Team", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Team", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewUser");
        }
        [CheckUserSession]
        public ActionResult AddUserForm()
        {
            try
            {
                List<DataType> degrees = _ContentServices.GetContent<BLO.ContentObjects.DataType>(ContentServices.ServiceTables.Degree, 9999).Contents.ToList();
                ViewBag.degrees = degrees;
                List<DataType> position = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Position, 9999).Contents.ToList();
                ViewBag.position = position;
                DynamicResponse<List<DataType>> response1 = _AdminServices.GetRoles();
                if (response1.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                ViewBag.roles = response1.Data;
                return View("AddUser");
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [CheckUserSession]
        public ActionResult AddUser(UserLO toAdd)
        {

            try
            {

                string path = Server.MapPath("~/Images/Users/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toAdd.PostedFileImage != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileImage.SaveAs(path + newName);
                    toAdd.Image = newName;
                }


                DynamicResponse<UserLO> usr = _AdminServices.AddUser(toAdd);
                if (usr.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddUsr", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddUsr", LoggerServices.ActionTypes.Add, "inputs:" + toAdd, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Users");

        }
        public ActionResult DeleteUser(long id)
        {
            try
            {

                _AdminServices.DeleteUser(id);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteTeam", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Users");

        }
        [CheckUserSession]
        public ActionResult GetUser(long id)
        {
            try
            {
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
                DynamicResponse<UserLO> response2 = new DynamicResponse<UserLO>();
                response2 = _AdminServices.GetUser(id);
                if (response2.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }

                ViewBag.User = response2.Data;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetTeam", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();

        }


        #endregion

        #region  DataType
        [CheckUserSession]
        [HttpGet]
        public ActionResult FormType(string FormName)
        {
            try
            {
                List<DataType> Datatype=new List<DataType>();
                if (FormName == "Degree")
                {
                    Datatype = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Degree, 9999).Contents.ToList(); 
                }
                if (FormName == "Position")
                {
                    Datatype = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.Position, 9999).Contents.ToList();
                }
                if (FormName == "IndexType")
                {
                    Datatype = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.IndexType, 9999).Contents.ToList();
                }
                if (FormName == "Role")
                {
                    DynamicResponse<List<DataType>> response= _AdminServices.GetRoles();
                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        return RedirectToAction("Index", "Oops");
                    }
                    Datatype = response.Data;
                }

                
                ViewBag.Title = FormName;
                ViewBag.Datatype = Datatype;



            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Type", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Type", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewTypeData");
        }

        [CheckUserSession]
        public ActionResult GetFormType(long id,string FormName)
        {
            try
            {
                DataType Datatype = new DataType();
                if (FormName == "Degree")
                {
                    Datatype = _ContentServices.GetContentOfItem<DataType>(ContentServices.ServiceTables.Degree, 1, id).Contents.FirstOrDefault();
                }
                if (FormName == "Position")
                {
                    Datatype = _ContentServices.GetContentOfItem<DataType>(ContentServices.ServiceTables.Position, 1, id).Contents.FirstOrDefault();
                }
                if (FormName == "IndexType")
                {
                    Datatype = _ContentServices.GetContentOfItem<DataType>(ContentServices.ServiceTables.IndexType, 1, id).Contents.FirstOrDefault();
                }
                ViewBag.Title = FormName;
                ViewBag.Datatype = Datatype;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetType", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();

        }

        public ActionResult DeleteFormTye(long id,string FormName)
        {
            try
            {
               _AdminServices.DeleteContent(id);
                ViewBag.Title = FormName;
               
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteTypeData", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("ViewTypeData", new { FormName = FormName });
        }

        [CheckUserSession]
        public ActionResult AddDataTypeForm(string FormName)
        {
            try
            {
                ViewBag.Title = FormName;
                return View("AddDataTypeForm");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        public ActionResult AddDataType(DataType toAdd,string FormName)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toAdd.Title,
                    Desc = toAdd.Desc
                });

                if (FormName == "Degree")
                {
                    _AdminServices.AddContent<DataType>(toAdd, AdminServices.ServiceAdminTables.Degree);
                }
                if (FormName == "Position")
                {
                    _AdminServices.AddContent<DataType>(toAdd, AdminServices.ServiceAdminTables.Position);
                }
                if (FormName == "IndexType")
                {
                    _AdminServices.AddContent<DataType>(toAdd, AdminServices.ServiceAdminTables.IndexType);
                }
                if (FormName == "Role")
                {
                    _AdminServices.AddRole(toAdd);
                }

                ViewBag.Title = FormName;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddDataType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddDataType", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("FormType", new { FormName = FormName });
        }


        [CheckUserSession]
        public ActionResult EditDataType(DataType toedit,string FormName)
        {

            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toedit.Title,
                    Desc = toedit.Desc
                });

                if (FormName == "Degree")
                {
                    _AdminServices.DeleteContent(toedit.Id);
                    _AdminServices.AddContent<DataType>(toedit, AdminServices.ServiceAdminTables.Degree);
                }
                if (FormName == "Position")
                {
                    _AdminServices.DeleteContent(toedit.Id);
                    _AdminServices.AddContent<DataType>(toedit, AdminServices.ServiceAdminTables.Position);
                }
                if (FormName == "IndexType")
                {
                    _AdminServices.DeleteContent(toedit.Id);
                    _AdminServices.AddContent<DataType>(toedit, AdminServices.ServiceAdminTables.IndexType);
                }

                ViewBag.Title = FormName;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditDataType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditDataType", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("FormType", new { FormName = FormName });
        }
        #endregion

        #region  Index
        [CheckUserSession]
        [HttpGet]
        public ActionResult Indexation()
        {
            try
            {
                List<DataType> indextype = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.IndexType, 9999).Contents.ToList();
                ViewBag.indextype = indextype;
                List<Index> data = new List<Index>();
                data = _ContentServices.GetContent<Index>(ContentServices.ServiceTables.Index, 9999).Contents.ToList();
                ViewBag.Index = data;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Index", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Index", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewIndex");
        }

        [CheckUserSession]
        public ActionResult GetIndex(long id)
        {
            try
            {
                List<DataType> indextype = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.IndexType, 9999).Contents.ToList();
                ViewBag.indextype = indextype;
                Index data = new Index();
                data = _ContentServices.GetContentOfItem<Index>(ContentServices.ServiceTables.Index, 1, id).Contents.FirstOrDefault();

                ViewBag.Index = data;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetIndex", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetIndex", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();
        }

        public ActionResult DeleteIndex(long id)
        {
            try
            {
                _AdminServices.DeleteContent(id);
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteIndex", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteIndex", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Indexation");
        }

        [CheckUserSession]
        public ActionResult AddIndexForm()
        {
            try
            {
                List<DataType> indextype = _ContentServices.GetContent<DataType>(ContentServices.ServiceTables.IndexType, 9999).Contents.ToList();
                ViewBag.indextype = indextype;
                return View("AddIndexForm");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult AddIndex(Index toAdd)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toAdd.Title,
                    Txt = toAdd.Txt,
                    IndexTypeId= toAdd.IndexTypeId,
                });

                _AdminServices.AddContent<Index>(toAdd, AdminServices.ServiceAdminTables.Index);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddIndex", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddIndex", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Indexation");
        }


        [CheckUserSession]
        public ActionResult EditIndex(Index toedit)
        {

            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toedit.Title,
                    Txt = toedit.Txt,
                    IndexTypeId = toedit.IndexTypeId,
                });
                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<Index>(toedit, AdminServices.ServiceAdminTables.Index);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditIndex", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditIndex", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Indexation");
        }
        #endregion

        #region  FooterMenu
        [CheckUserSession]
        [HttpGet]
        public ActionResult FooterMenu()
        {
            try
            {
                List<FooterMenu> data = new List<FooterMenu>();
                data = _ContentServices.GetContent<FooterMenu>(ContentServices.ServiceTables.FooterMenu, 9999).Contents.ToList();
                ViewBag.FooterMenu = data;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:FooterMenu", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("FooterMenu", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewFooterMenu");
        }

        [CheckUserSession]
        public ActionResult GetFooterMenu(long id)
        {
            try
            {
                FooterMenu data = new FooterMenu();
                data = _ContentServices.GetContentOfItem<FooterMenu>(ContentServices.ServiceTables.FooterMenu, 1, id).Contents.FirstOrDefault();

                ViewBag.FooterMenu = data;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetFooterMenu", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetFooterMenu", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();
        }

        public ActionResult DeleteFooterMenu(long id)
        {
            try
            {
                _AdminServices.DeleteContent(id);
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteFooterMenu", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteFooterMenu", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("FooterMenu");
        }

        [CheckUserSession]
        public ActionResult AddFooterMenuForm()
        {
            try
            {
                return View("AddFooterMenuForm");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult AddFooterMenu(FooterMenu toAdd)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toAdd.Title,
                    Text = toAdd.Text,
                    Img = toAdd.Img,
                    IsEnabled = toAdd.IsEnabled
                });

                string path = Server.MapPath("~/Images/FooterMenu/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toAdd.PostedFileImg != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileImg.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileImg.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileImg.SaveAs(path + newName);
                    toAdd.Img = newName;
                }

                _AdminServices.AddContent<FooterMenu>(toAdd, AdminServices.ServiceAdminTables.FooterMenu);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddFooterMenu", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddFooterMenu", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("FooterMenu");
        }


        [CheckUserSession]
        public ActionResult EditFooterMenu(FooterMenu toedit)
        {

            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toedit.Title,
                    Text = toedit.Text,
                    Img=toedit.Img,
                    IsEnabled=toedit.IsEnabled
                });
                string path = Server.MapPath("~/Images/FooterMenu/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toedit.PostedFileImg != null)
                {
                    string extension = Path.GetExtension(toedit.PostedFileImg.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toedit.PostedFileImg.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toedit.PostedFileImg.SaveAs(path + newName);
                    toedit.Img = newName;
                }

                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<FooterMenu>(toedit, AdminServices.ServiceAdminTables.FooterMenu);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditFooterMenu", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditFooterMenu", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("FooterMenu");
        }
        #endregion

        #region  CitationType
        [CheckUserSession]
        [HttpGet]
        public ActionResult CitationType()
        {
            try
            {
                List<CitationType> data = new List<CitationType>();
                data = _ContentServices.GetContent<CitationType>(ContentServices.ServiceTables.Citation, 9999).Contents.ToList();
                ViewBag.CitationType = data;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:CitationType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("CitationType", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewCitationType");
        }

        [CheckUserSession]
        public ActionResult GetCitationType(long id)
        {
            DynamicResponse<List<string>> response = new DynamicResponse<List<string>>();
            try
            {
                response = _AdminServices.GetSubmissionFieldName();
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                List<string> lst = new List<string>(){ "Type", "Specialit", "Title", "date","", "IssueNO", "Volume","AbstractText","Prefix", "AuthorInfo", "year" };
                ViewBag.CitationVariable = lst;
                CitationType data = new CitationType();
                data = _ContentServices.GetContentOfItem<CitationType>(ContentServices.ServiceTables.Citation, 1, id).Contents.FirstOrDefault();

                ViewBag.CitationType = data;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetCitationType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetCitationType", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();
        }

        public ActionResult DeleteCitationType(long id)
        {
            try
            {
                _AdminServices.DeleteContent(id);
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteCitationType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteCitationType", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("CitationType");
        }

        [CheckUserSession]
        public ActionResult AddCitationTypeForm()
        {
            DynamicResponse<List<string>> response = new DynamicResponse<List<string>>();
            try
            {
                response = _AdminServices.GetSubmissionFieldName();
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return RedirectToAction("Index", "Oops");
                }
                SubmissionLO s = new SubmissionLO();

                List<string> data = new List<string>();
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.IssueNO));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.PublishDate));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                data.Add(Helper.GetDisplayName<SubmissionLO>(t => t.Title));
                ViewBag.CitationVariable = data;
                return View("AddCitationTypeForm");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        [HttpPost, ValidateInput(false)]
        public ActionResult AddCitationType(CitationType toAdd)
        {
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toAdd.Title,
                    Text = toAdd.Text,
                    IsEnabled = toAdd.IsEnabled
                });


                _AdminServices.AddContent<CitationType>(toAdd, AdminServices.ServiceAdminTables.Citation);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddCitationType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddCitationType", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("CitationType");
        }


        [CheckUserSession]
        public ActionResult EditCitationType(CitationType toedit)
        {
            DynamicResponse<List<string>> response = new DynamicResponse<List<string>>();
            string json = "";
            try
            {

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Title = toedit.Title,
                    Text = toedit.Text,
                    IsEnabled = toedit.IsEnabled
                });



                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<CitationType>(toedit, AdminServices.ServiceAdminTables.Citation);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditCitationType", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditCitationType", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("CitationType");
        }
        #endregion

        #region Videos

        [CheckUserSession]
        public ActionResult AddVideosForm()
        {
            try
            {
                return View("AddVideos");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        public ActionResult AddVideos(Videos toAdd)
        {
            string json = "";
            try
            {


                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {

                    Name = toAdd.Name,
                    Image = toAdd.Image,
                    MainDesc = toAdd.MainDesc,
                    SubDesc = toAdd.SubDesc,
                    Pos = toAdd.Pos,
                    Src=toAdd.Src,
                    VideoDate = toAdd.VideoDate,


                });

                string path = Server.MapPath("~/Images/Videos/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toAdd.PostedFileImage != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileImage.SaveAs(path + newName);
                    toAdd.Image = newName;
                }


                if (toAdd.PostedFileVideo != null)
                {
                    string extension = Path.GetExtension(toAdd.PostedFileVideo.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toAdd.PostedFileVideo.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toAdd.PostedFileVideo.SaveAs(path + newName);
                    toAdd.Src = newName;
                }

                //add position
                //List<Videos> members = _ContentServices.GetContent<Videos>(ContentServices.ServiceTables.Videos, 99).Contents.ToList();
                //edit positions for add
                bool result = _AdminServices.UpdatePosition("MEMBERS_POS", 17, true, null);
                toAdd.Pos = "0";



                _AdminServices.AddContent<Videos>(toAdd, AdminServices.ServiceAdminTables.Videos);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:AddVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddVideos", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Videos");

        }

        public ActionResult DeleteVideos(long id)
        {
            try
            {
                // edit position
                //public bool UpdatePosition(string positionCode, long tableId, string currentPosition, bool isAdd, long? recordId)
                bool result = _AdminServices.UpdatePosition("MEMBERS_POS", 17, false, id);

                _AdminServices.DeleteContent(id);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:DeleteVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteVideos", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Videos");

        }

        [CheckUserSession]
        public ActionResult Videos()
        {
            try
            {
                List<Videos> ourVideos = _ContentServices.GetContent<Videos>(ContentServices.ServiceTables.Videos, 9999).Contents.ToList();
                ourVideos = ourVideos.OrderBy(e => long.Parse(e.Pos)).ToList();
                ViewBag.Videos = ourVideos;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:Videos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Videos", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewVideos");
        }


        [CheckUserSession]
        public ActionResult GetVideos(long id)
        {
            try
            {
                Videos members = _ContentServices.GetContentOfItem<Videos>(ContentServices.ServiceTables.Videos, 1, id).Contents.FirstOrDefault();
                ViewBag.Videos = members;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:GetVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetVideos", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();

        }

        [CheckUserSession]
        public ActionResult EditVideos(Videos toedit)
        {
            string json = "";
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {

                    Name = toedit.Name,
                    Image = toedit.Image,
                    Src = toedit.Src,
                    MainDesc = toedit.MainDesc,
                    SubDesc = toedit.SubDesc,
                    Pos = toedit.Pos,
                    VideoDate = toedit.VideoDate

                });
                string path = Server.MapPath("~/Images/Videos/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (toedit.PostedFileImage != null)
                {
                    string extension = Path.GetExtension(toedit.PostedFileImage.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toedit.PostedFileImage.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toedit.PostedFileImage.SaveAs(path + newName);
                    toedit.Image = newName;
                }


                if (toedit.PostedFileVideo != null)
                {
                    string extension = Path.GetExtension(toedit.PostedFileVideo.FileName);
                    string Name = Path.GetFileNameWithoutExtension(toedit.PostedFileVideo.FileName);
                    string newName = Name + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    toedit.PostedFileVideo.SaveAs(path + newName);
                    toedit.Src = newName;
                }

                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<Videos>(toedit, AdminServices.ServiceAdminTables.Videos);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("hudaabumayha.ham@gmail.com", "[LMJ] Error:EditVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditVideos", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Videos");

        }
        #endregion


        
        [CheckUserSession]
        [HttpPost]
        public JsonResult EditPos(List<PosLO> toedit)
        {
            try
            {
                DynamicResponse<List<long>> response = _AdminServices.EditPos(toedit);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    return Json("error");
                }

                return Json("re-orderd");
            }
            catch (Exception ex)
            {

                return Json("error");
            }
            return Json("okk");
        }
    }
}