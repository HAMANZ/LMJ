using Anz.LMJ.BLL.Content;
using Anz.LMJ.BLL.Logic;
using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.BLO.LookUpObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Anz.LMJ.BLL.Logic.Enums;

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
                __SharedConvetTable[ServiceAdminTables.EditorsPick] = LookUpLogic.AdminTables.EditorsPick;
                __SharedConvetTable[ServiceAdminTables.IssueFilter] = LookUpLogic.AdminTables.IssueFilter;
            }


            }



            #region My Variables

            public enum ServiceAdminTables
            {
                About_Page,
                Home_Banner,
                Events,
                Members,
                Videos,
                Team,
                News,
                EditorsPick,
            IssueFilter,
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


        

        public List<UserLO> GetUsers()
        {
            #region logics
            DynamicResponse<List<UserLO>> uesrs =new DynamicResponse<List<UserLO>>();
           UserLogic _UserLogic = new UserLogic();
            #endregion
            try
            {
                uesrs=_UserLogic.GetUsers();
                return uesrs.Data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

    }
    }


