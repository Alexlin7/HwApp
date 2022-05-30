using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HwApp1410931031.Security;
using Jose;

namespace HwApp1410931031
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            string CookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            if (httpRequest.Cookies[CookieName] != null)
            {
                JwtObject jwtObject = JWT.Decode<JwtObject>(Convert.ToString(httpRequest.Cookies[CookieName].Value),
                    Encoding.UTF8.GetBytes(SecretKey),
                    JwsAlgorithm.HS512);

                string[] roles = jwtObject.Role.Split(new char[] { ',' });

                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier, jwtObject.Account)
                };

                var claimIdentity = new ClaimsIdentity(claims, CookieName);

                claimIdentity.AddClaim(
                    new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "My Identity", @"http://www.w3.org/2001/XMLSchema#string"));

                HttpContext.Current.User = new GenericPrincipal(claimIdentity, roles);
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }
        }
    }
}
