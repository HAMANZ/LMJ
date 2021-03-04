using Anz.LMJ.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.WebServices
{
    public class LoggerServices
    {
        #region Logic
        LoggerLogic _LoggerLogic = new LoggerLogic();
        #endregion

        public enum ActionTypes { Add, Update, Read, Delete }

        public void Error(string Method, ActionTypes Action, string Parameters, string Result)
        {
            try
            {
                _LoggerLogic.Error(Method, Action.ToString(), Parameters, Result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void Admin(string Method, ActionTypes Action, string Parameters, string Result)
        {
            try
            {
                _LoggerLogic.Admin(Method, Action.ToString(), Parameters, Result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void User(string Method, ActionTypes Action, string Parameters, string Result)
        {
            try
            {
                _LoggerLogic.User(Method, Action.ToString(), Parameters, Result);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void CyberSource(string Method, ActionTypes Action, string Parameters, string Result)
        {
            try
            {
                _LoggerLogic.CyberSource(Method, Action.ToString(), Parameters, Result);

            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
