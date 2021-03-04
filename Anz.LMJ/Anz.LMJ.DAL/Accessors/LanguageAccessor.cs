using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class LanguageAccessor
    {

        #region Get

        public Language Get(string code)
        {
            Language lang = new Language();


            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    lang = db.Languages
                        .Where(e => e.Code == code).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
            }

            return lang;
        }
        #endregion


        #region Add

        #endregion

        #region Update

        #endregion


        #region Delete

        #endregion

    }
}
