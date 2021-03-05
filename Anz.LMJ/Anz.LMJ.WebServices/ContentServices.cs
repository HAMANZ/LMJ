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
            Members,
            Videos,
            Team,
            News,
            EditorsPick,
            IssueFilter,
            Contact,
            Position,
            Role,
            Policy,
            Terms,
            CopyRight,
            Citation

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
                _SharedTables[ServiceTables.Members] = LookUpLogic.AdminTables.Members;
                _SharedTables[ServiceTables.Videos] = LookUpLogic.AdminTables.Videos;
                _SharedTables[ServiceTables.Team] = LookUpLogic.AdminTables.Team;
                _SharedTables[ServiceTables.News] = LookUpLogic.AdminTables.News;
                _SharedTables[ServiceTables.IssueFilter] = LookUpLogic.AdminTables.IssueFilter;
                _SharedTables[ServiceTables.Contact] = LookUpLogic.AdminTables.Contact;
                _SharedTables[ServiceTables.Role] = LookUpLogic.AdminTables.Role;
                _SharedTables[ServiceTables.Position] = LookUpLogic.AdminTables.Position;
                _SharedTables[ServiceTables.Policy] = LookUpLogic.AdminTables.Policy;
                _SharedTables[ServiceTables.Terms] = LookUpLogic.AdminTables.Terms;
                _SharedTables[ServiceTables.CopyRight] = LookUpLogic.AdminTables.CopyRight;
                _SharedTables[ServiceTables.Citation] = LookUpLogic.AdminTables.Citation;
                
            }
        }
        #endregion


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
