using Anz.LMJ.DAL.Accessor;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLL
{
    public class LoggerLogic
    {
        #region Accessor
        AllLoggerAccessor _AllLoggerAccessor = new AllLoggerAccessor();
        #endregion


        public void Error(string Method, string Action,string Parameters,string Result)
        {
            try
            {
                Logger_Error toAdd = new Logger_Error();
                toAdd.MethodName = Method;
                toAdd.ActionType = Action;
                toAdd.Parameters = Parameters;
                toAdd.Result = Result;
                toAdd.SysDate = DateTime.Now;


                _AllLoggerAccessor.Error(toAdd);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void Admin(string Method, string Action,string Parameters,string Result)
        {
            try
            {

                Logger_Admin toAdd = new Logger_Admin();
                toAdd.MethodName = Method;
                toAdd.ActionType = Action;
                toAdd.Parameters = Parameters;
                toAdd.Result = Result;

                _AllLoggerAccessor.Admin(toAdd);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void User(string Method, string Action,string Parameters,string Result)
        {
            try
            {

                Logger_User toAdd = new Logger_User();
                toAdd.MethodName = Method;
                toAdd.ActionType = Action;
                toAdd.Parameters = Parameters;
                toAdd.Result = Result;

                _AllLoggerAccessor.User(toAdd);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void CyberSource(string Method, string Action,string Parameters,string Result)
        {
            try
            {

                Logger_CyberSource toAdd = new Logger_CyberSource();
                toAdd.MethodName = Method;
                toAdd.ActionType = Action;
                toAdd.Parameters = Parameters;
                toAdd.Result = Result;

                _AllLoggerAccessor.CyberSource(toAdd);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
