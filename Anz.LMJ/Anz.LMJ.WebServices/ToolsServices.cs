
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anz.LMJ.BLL;

namespace Anz.LMJ.WebServices
{
    public static class ToolsServices
    {


        public static void sendEmail(string emailTo, string subject, string body)
        {
            Tools.sendEmail(emailTo, subject, body);
        }
    }
}
