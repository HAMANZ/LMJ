using Anz.LMJ.BLL.Content;
using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.BLO.ContentObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Anz.LMJ.BLL.Logic.Enums;
using Anz.LMJ.BLO.LogicObjects.Review;

namespace Anz.LMJ.WebServices
{
    public class AdminServices
    {

            LookUpLogic _ContentAdminLogic = new LookUpLogic();

            #region Shared Variables

            private static Dictionary<ServiceAdminTables, LookUpLogic.AdminTables> __SharedConvetTable = new Dictionary<ServiceAdminTables, LookUpLogic.AdminTables>();


            #endregion

            public AdminServices()
            {
                if (__SharedConvetTable.Count == 0)
                {

                __SharedConvetTable[ServiceAdminTables.Home_Banner] = LookUpLogic.AdminTables.Hero_Banner;
                __SharedConvetTable[ServiceAdminTables.About_Page] = LookUpLogic.AdminTables.About_Page;
                __SharedConvetTable[ServiceAdminTables.Events] = LookUpLogic.AdminTables.Events;
                __SharedConvetTable[ServiceAdminTables.Members] = LookUpLogic.AdminTables.Members;
                __SharedConvetTable[ServiceAdminTables.Videos] = LookUpLogic.AdminTables.Videos;
                __SharedConvetTable[ServiceAdminTables.Team] = LookUpLogic.AdminTables.Team;
                __SharedConvetTable[ServiceAdminTables.News] = LookUpLogic.AdminTables.News;
                __SharedConvetTable[ServiceAdminTables.IssueFilter] = LookUpLogic.AdminTables.IssueFilter;
                __SharedConvetTable[ServiceAdminTables.Contact] = LookUpLogic.AdminTables.Contact;
                __SharedConvetTable[ServiceAdminTables.Degree] = LookUpLogic.AdminTables.Degree;
                __SharedConvetTable[ServiceAdminTables.Position] = LookUpLogic.AdminTables.Position;
                __SharedConvetTable[ServiceAdminTables.FooterMenu] = LookUpLogic.AdminTables.FooterMenu;
                __SharedConvetTable[ServiceAdminTables.Citation] = LookUpLogic.AdminTables.Citation;
                __SharedConvetTable[ServiceAdminTables.Index] = LookUpLogic.AdminTables.Index;
                __SharedConvetTable[ServiceAdminTables.IndexType] = LookUpLogic.AdminTables.IndexType;
                __SharedConvetTable[ServiceAdminTables.Footer] = LookUpLogic.AdminTables.Footer;
                
            }


            }



            #region My Variables

            public enum ServiceAdminTables
            {
            Home_Banner,
            About_Page,
            Hero_Banner,
            Events,
            Members,
            Videos,
            Team,
            News,
            EditorsPick,
            IssueFilter,
            Contact,
            Position,
            Degree,
            Policy,
            Terms,
            CopyRight,
            Citation,
            Index,
            IndexType,
            FooterMenu,
            Footer
        }

        #endregion



        public bool UpdatePosition(string positionCode, long tableId, bool isAdd, long? recordId)
        {
            try
            {
                _ContentAdminLogic.UpdatePosition(positionCode, tableId, isAdd, recordId);

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }




        #region Contant

        public long AddContent<T>(T obj, AdminServices.ServiceAdminTables table)
            {
                try
                {
                    return _ContentAdminLogic.AddContent<T>(obj, __SharedConvetTable[table]);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }


            public void DeleteContent(long contenId)
            {
                try
                {
                    _ContentAdminLogic.DeleteContent(contenId);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }


        public Dictionary<string, string> isAdmin(string email, string pass)
        {
            #region logics
            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                return _UserLogic.isAdmin(email, pass);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public Dictionary<string, string> AuthIsAdmin(string email, string pass)
        {
            #region logics
            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                return _UserLogic.AuthIsAdmin(email, pass);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public bool CheckRole(long userId, Roles role, long journalId)
        {
            #region logics
            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                DynamicResponse<bool> response = new DynamicResponse<bool>();
                response = _UserLogic.CheckRole(userId, role, journalId);

                return response.Data;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

            public List<Options> getListArticleOption() {
            HomeServices _HomeServices = new HomeServices();
            DynamicResponse<List<SubmissionLO>> submissions = _HomeServices.getListArticleOption();
            List<Options>  options = new List<Options>();
            foreach (SubmissionLO item in submissions.Data)
            {
                options.Add(new Options
                {
                    Id = item.Id,
                    Value = item.Title
                }); ;

            }

            return options;
        }


        public List<SubmissionLO> GetArticles(List<long> articleids)
        {
            HomeServices _HomeServices = new HomeServices();
            DynamicResponse<List<SubmissionLO>> submissions = _HomeServices.GetArticle(articleids);
            return submissions.Data;
        }

        public List<SubmissionLO> GetAllArticles()
        {
            HomeServices _HomeServices = new HomeServices();
            DynamicResponse<List<SubmissionLO>> submissions = _HomeServices.GetAllArticles();
            return submissions.Data;
        }

        public SubmissionLO GetArticle(long id)
        {
            HomeServices _HomeServices = new HomeServices();
            DynamicResponse<SubmissionLO> submissions = _HomeServices.GetArticle(id);
            return submissions.Data;
        }

        public DynamicResponse<UserLO> AddUser(UserLO user)
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<UserLO> response = new DynamicResponse<UserLO>();
            response = _UserLogic.AddUser(user);
            return response;
        }

        public DynamicResponse<DataType> AddRole(DataType rol)
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<DataType> response = new DynamicResponse<DataType>();
            response = _UserLogic.AddRole(rol);
            return response;
        }

        
        public DynamicResponse<long> DeleteUser(long userid)
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<long> response = new DynamicResponse<long>();
            response = _UserLogic.DeleteUser(userid);
            return response;
        }

        public DynamicResponse<List<DataType>> GetRoles()
        {
            UserLogic _UserLogic = new UserLogic();
            DynamicResponse<List<DataType>> response = new DynamicResponse<List<DataType>>();
            response = _UserLogic.GetRoles();
            return response;
        }

        public UserLO GetBasic(long usrid)
        {
            #region logics
            DynamicResponse<UserLO> uesrs = new DynamicResponse<UserLO>();
            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                uesrs = _UserLogic.GetBasic(usrid);
                return uesrs.Data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        
       public List<int> GetRoles(List<string> roles)
        {
            #region logics
            DynamicResponse<List<int>> response = new DynamicResponse<List<int>>();
            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                response = _UserLogic.GetRoles(roles);
                return response.Data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public List<UserLO> GetUsers(List<int> rolesid)
        {
            #region logics
            DynamicResponse<List<UserLO>> uesrs =new DynamicResponse<List<UserLO>>();
           UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                uesrs=_UserLogic.GetUsers(rolesid);
                return uesrs.Data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public DynamicResponse<UserLO> GetUser(long userid)
            {
                #region logics
                
                UserLogic _UserLogic = new UserLogic();
                #endregion
                try
                {
               return _UserLogic.GetBasic(userid);
               
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

        
         public DynamicResponse<long> AdmitReview(bool isAdmit,long reivewid)
        {
            #region logics

            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                return _UserLogic.AdmitReview(isAdmit,reivewid);

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public DynamicResponse<ReviewLO> GetReview(long id)
        {
            #region logics

            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                return _UserLogic.GetReview(id);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public DynamicResponse<List<ReviewLO>> GetReviews()
        {
            #region logics

            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                return _UserLogic.GetReviews();

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public DynamicResponse<long> EditUser(UserLO toEdit)
        {
            #region logics

            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                return _UserLogic.EditUser(toEdit);

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        
        public List<UserLO> GetAllUsers()
        {
            #region logics
            DynamicResponse<List<UserLO>> uesrs = new DynamicResponse<List<UserLO>>();
            UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                uesrs = _UserLogic.GetUsers();
                return uesrs.Data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public DynamicResponse<List<string>> GetSubmissionFieldName()
        {
            SubmissionLogic _SubmissionLogic = new SubmissionLogic();
            DynamicResponse<List<string>> response = new DynamicResponse<List<string>>();
            response = _SubmissionLogic.GetSubmissionFieldName();
            return response;
        }
        
        #endregion

    }
    }


