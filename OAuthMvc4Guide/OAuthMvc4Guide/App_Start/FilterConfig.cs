using System.Web;
using System.Web.Mvc;

namespace OAuthMvc4Guide
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}