using System.Web;
using System.Web.Mvc;

namespace Anz.LMJ.StartUp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
