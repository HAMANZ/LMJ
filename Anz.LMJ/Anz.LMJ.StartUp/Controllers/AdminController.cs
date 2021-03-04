using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.BLO.LookUpObjects;
using Anz.LMJ.FrontEnd;
using Anz.LMJ.WebServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Anz.LMJ.BLL.Logic.Enums;

namespace Anz.LMJ.StartUp.Controllers
{
    public class AdminController : Controller
    {
        ContentServices _ContentServices = new ContentServices();
        HomeServices _HomeServices = new HomeServices();
        LoggerServices _LoggerServices = new LoggerServices();
        AdminServices _AdminServices = new AdminServices();
        ManagementService _ManagementService = new ManagementService();
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

        public ActionResult SignIn(string email, string password,string view)
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:SignIn", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:IssueFilter", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:HomeBanner", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:HomeBanner", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:HomeBanner", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
               // _LoggerServices.Error("HomeBanner", LoggerServices.ActionTypes.Add, "home banner inputs:" + json, "<p>Error: " + ex.Message + "</p>");

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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AboutUs_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AboutUs_Page", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(aboutuspage);
        }

        [HttpGet]
        [CheckUserSession]
        public ActionResult Articles()
        {
            try
            {
              
                List<SubmissionLO> editorspick= _AdminServices.GetAllArticles();
                ViewBag.editorspick = editorspick;


            }
            catch (Exception ex)
            {
                #region Logger
                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:EditorPick", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditorPick", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewArticles");

        }


        
        public ActionResult GetArticle(long id)
        {
            try
            {
                SubmissionLO article = _AdminServices.GetArticle(id);
                ViewBag.article = article;
                List<UserLO> users = _AdminServices.GetUsers();
                ViewBag.users = users;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:GetArticle", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetArticle", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }

            return View("GetArticle");

        }


        [CheckUserSession]
        public JsonResult EditArticle(long submissionid, long userId, bool isEditorsPick, bool isTopReader,List<long> tagsid)
        {
            try
            {
                DynamicResponse<long> response;
                response = _HomeServices.UpdateSubmission(submissionid, userId, isEditorsPick, isTopReader, tagsid);
                if (response.HttpStatusCode != HttpStatusCode.OK) {
                   return Json("error");
                }  
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error");
            }

        }



        [HttpPost]
        [CheckUserSession]
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

                string path = Server.MapPath("~/Images/");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AboutUs_Page", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AboutUs_Page", LoggerServices.ActionTypes.Add, "inputs" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View(toAdd);
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AddEventsSectionForm", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { img = toAdd.MainImg, title = toAdd.MainTitle, desc = toAdd.MainDesc, date = toAdd.EventDate,images=toAdd.SubImg });

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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AddEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddEventsSection", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion

            }
            return RedirectToAction("Events");

        }

        public ActionResult DeleteEventsSection(long id)
        {
            try
            {

                // edit position
                //public bool UpdatePosition(string positionCode, long tableId, string currentPosition, bool isAdd, long? recordId)
                bool result = _AdminServices.UpdatePosition("EVENTS_POSITION", 20, false, id);

                _AdminServices.DeleteContent(id);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:DeleteEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:GetEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new { img = toedit.MainImg, title = toedit.MainTitle, desc = toedit.MainDesc, date = toedit.EventDate,images=toedit.SubImg });

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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:EditEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:EditEventsSection", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditEventsSection", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion
            }
            return View("ViewEvents");

        }



        #endregion




        #region Members

        [CheckUserSession]
        public ActionResult AddMembersForm()
        {
            try
            {
                return View("AddMembers");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        public ActionResult AddMembers(Members toAdd)
        {
            string json = "";
            try
            {


                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {

                    Name = toAdd.Name,
                    Image = toAdd.Image,
                    Position = toAdd.Position,
                    MainDesc = toAdd.MainDesc,
                    SubDesc = toAdd.SubDesc,
                    Pos = toAdd.Pos


                });

                string path = Server.MapPath("~/Images/Members/");
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

                //add position
                //List<Members> members = _ContentServices.GetContent<Members>(ContentServices.ServiceTables.Members, 99).Contents.ToList();
                //edit positions for add
                bool result = _AdminServices.UpdatePosition("MEMBERS_POS", 17, true, null);
                toAdd.Pos = "0";



                _AdminServices.AddContent<Members>(toAdd, AdminServices.ServiceAdminTables.Members);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AddMembers", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddMembers", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Members");

        }

        public ActionResult DeleteMembers(long id)
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:DeleteMembers", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteMembers", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Members");

        }

        [CheckUserSession]
        public ActionResult Members()
        {
            try
            {
                List<Members> ourMembers = _ContentServices.GetContent<Members>(ContentServices.ServiceTables.Members, 9999).Contents.ToList();
                ourMembers = ourMembers.OrderBy(e => long.Parse(e.Pos)).ToList();
                ViewBag.Members = ourMembers;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:Members", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Members", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewMembers");
        }


        [CheckUserSession]
        public ActionResult GetMembers(long id)
        {
            try
            {
                Members members = _ContentServices.GetContentOfItem<Members>(ContentServices.ServiceTables.Members, 1, id).Contents.FirstOrDefault();
                ViewBag.Members = members;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:GetMembers", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetMembers", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();

        }

        [CheckUserSession]
        public ActionResult EditMembers(Members toedit)
        {
            string json = "";
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {

                    Name = toedit.Name,
                    Image = toedit.Image,
                    Position = toedit.Position,
                    MainDesc = toedit.MainDesc,
                    SubDesc = toedit.SubDesc,
                    Pos = toedit.Pos


                });
                string path = Server.MapPath("~/Images/Members/");
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
                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<Members>(toedit, AdminServices.ServiceAdminTables.Members);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:EditMembers", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditMembers", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Members");

        }
        #endregion




        #region Team

        [CheckUserSession]
        public ActionResult AddTeamForm()
        {
            try
            {
                return View("AddTeam");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [CheckUserSession]
        public ActionResult AddTeam(Team toAdd)
        {
            string json = "";
            try
            {


                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {

                    Name = toAdd.Name,
                    Image = toAdd.Image,
                    Position = toAdd.Position,
                    MainDesc = toAdd.MainDesc,
                    Pos = toAdd.Pos


                });

                string path = Server.MapPath("~/Images/Team/");
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

                //add position
                //List<Team> members = _ContentServices.GetContent<Team>(ContentServices.ServiceTables.Team, 99).Contents.ToList();
                //edit positions for add
                bool result = _AdminServices.UpdatePosition("MEMBERS_POS", 17, true, null);
                toAdd.Pos = "0";



                _AdminServices.AddContent<Team>(toAdd, AdminServices.ServiceAdminTables.Team);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AddTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("AddTeam", LoggerServices.ActionTypes.Add, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Team");

        }

        public ActionResult DeleteTeam(long id)
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:DeleteTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("DeleteTeam", LoggerServices.ActionTypes.Delete, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Team");

        }

        [CheckUserSession]
        public ActionResult Team()
        {
            try
            {
                List<Team> ourTeam = _ContentServices.GetContent<Team>(ContentServices.ServiceTables.Team, 9999).Contents.ToList();
                ourTeam = ourTeam.OrderBy(e => long.Parse(e.Pos)).ToList();
                ViewBag.Team = ourTeam;

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:Team", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("Team", LoggerServices.ActionTypes.Read, "", "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View("ViewTeam");
        }


        [CheckUserSession]
        public ActionResult GetTeam(long id)
        {
            try
            {
                Team members = _ContentServices.GetContentOfItem<Team>(ContentServices.ServiceTables.Team, 1, id).Contents.FirstOrDefault();
                ViewBag.Team = members;
            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:GetTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("GetTeam", LoggerServices.ActionTypes.Read, "id:" + id, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return View();

        }

        [CheckUserSession]
        public ActionResult EditTeamSection(Team toedit)
        {
            string json = "";
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {

                    Name = toedit.Name,
                    Image = toedit.Image,
                    Position = toedit.Position,
                    MainDesc = toedit.MainDesc,
                    Pos = toedit.Pos


                });
                string path = Server.MapPath("~/Images/Team/");
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
                _AdminServices.DeleteContent(toedit.Id);
                _AdminServices.AddContent<Team>(toedit, AdminServices.ServiceAdminTables.Team);

            }
            catch (Exception ex)
            {
                #region Logger

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:EditTeam", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditTeam", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Team");

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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:AddVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:DeleteVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:Videos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:GetVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
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

                ToolsServices.sendEmail("f.refaai@anzimaty.com", "[LMJ] Error:EditVideos", "<p>" + ex.Message + "</p><p>" + ex.InnerException + "</p>");
                _LoggerServices.Error("EditVideos", LoggerServices.ActionTypes.Update, "inputs:" + json, "<p>Error: " + ex.Message + "</p>");

                #endregion;
            }
            return RedirectToAction("Videos");

        }
        #endregion



    }
}