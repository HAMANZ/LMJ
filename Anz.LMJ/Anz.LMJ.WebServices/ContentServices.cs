using Anz.LMJ.BLL.Content;
using Anz.LMJ.BLO.LookUpObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.WebServices
{
    public class ContentServices
    {

        #region Logics
        LookUpLogic _LookUpLogic = new LookUpLogic();
        //ContentAdminLogic _ContentAdminLogic = new ContentAdminLogic();


        #endregion


        #region Variables
        public enum ServiceTables
        {
            About_Page,
            Hero_Banner,
            Events,
            EditorialBoard,
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

        Dictionary<ServiceTables, LookUpLogic.AdminTables> _SharedTables = new Dictionary<ServiceTables, LookUpLogic.AdminTables>();

        #endregion


        #region Constructor
        public ContentServices()
        {
            if (_SharedTables.Count() == 0)
            {
                _SharedTables[ServiceTables.Hero_Banner] = LookUpLogic.AdminTables.Hero_Banner;
                _SharedTables[ServiceTables.About_Page] = LookUpLogic.AdminTables.About_Page;
                _SharedTables[ServiceTables.Events] = LookUpLogic.AdminTables.Events;
                _SharedTables[ServiceTables.EditorialBoard] = LookUpLogic.AdminTables.EditorialBoard;
                _SharedTables[ServiceTables.Videos] = LookUpLogic.AdminTables.Videos;
                _SharedTables[ServiceTables.Team] = LookUpLogic.AdminTables.Team;
                _SharedTables[ServiceTables.News] = LookUpLogic.AdminTables.News;
                _SharedTables[ServiceTables.IssueFilter] = LookUpLogic.AdminTables.IssueFilter;
                _SharedTables[ServiceTables.Contact] = LookUpLogic.AdminTables.Contact;
                _SharedTables[ServiceTables.Degree] = LookUpLogic.AdminTables.Degree;
                _SharedTables[ServiceTables.Position] = LookUpLogic.AdminTables.Position;
                _SharedTables[ServiceTables.FooterMenu] = LookUpLogic.AdminTables.FooterMenu;
                _SharedTables[ServiceTables.Citation] = LookUpLogic.AdminTables.Citation;
                _SharedTables[ServiceTables.Index] = LookUpLogic.AdminTables.Index;
                _SharedTables[ServiceTables.IndexType] = LookUpLogic.AdminTables.IndexType;
                _SharedTables[ServiceTables.Footer] = LookUpLogic.AdminTables.Footer;
                

            }
        }
        #endregion


      

             public List<string> GetAttributes(ServiceTables table)
        {
            try
            {
                List<LookUpAttributes> result = new List<LookUpAttributes>();
                List<string> attrName=new List<string>();
                result = _LookUpLogic.GetAttributes(_SharedTables[table], "en");
                foreach (LookUpAttributes item in (List<LookUpAttributes>)result) {
                    attrName.Add(item.Name);
                }
                return attrName;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public GeneralContents<T> GetContent<T>(ServiceTables table, int limit)
        {
            try
            {
                GeneralContents<T> result = new GeneralContents<T>();

                result = _LookUpLogic.GetContent<T>(_SharedTables[table], "en", 0, limit, null, true, null);

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        List<LookUpAttributes> attributes = new List<LookUpAttributes>();

        public object ConfigurationManager { get; private set; }

        public GeneralContents<T> GetContentOfItem<T>(ServiceTables table, int limit, long mainId)
        {
            try
            {
                GeneralContents<T> result = new GeneralContents<T>();

                result = _LookUpLogic.GetContent<T>(_SharedTables[table], "en", 0, limit, null, true, mainId);

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
