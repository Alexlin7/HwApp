using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using HwApp1410931031.Security;
using HwApp1410931031.Services;
using HwApp1410931031.ViewModels;

namespace HwApp1410931031.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersDBService membersService = new MembersDBService();

        private readonly MailService mailService = new MailService();
        // GET: Members
        public ActionResult Index()
        {
            return View();
        }
        #region 註冊

        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Guestbooks");
            return View();
        }

        [HttpPost]
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            if (ModelState.IsValid)
            {
                RegisterMember.NewMember.Password = RegisterMember.Password;
                string AuthCode = mailService.GetValidateCode();
                RegisterMember.NewMember.AuthCode = AuthCode;
                membersService.Register(RegisterMember.NewMember);

                string TempMail = System.IO.File.ReadAllText(
                    Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html")
                );

                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "Members", new
                    {
                        Account = RegisterMember.NewMember.Account,
                        AuthCode = AuthCode
                    })
                };

                string MailBody = mailService.GetRegisterMailBody(
                    TempMail, RegisterMember.NewMember.Name,
                    ValidateUrl.ToString().Replace("%3F", "?"));

                mailService.SendRegisterMail(MailBody, RegisterMember.NewMember.Email);

                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                return RedirectToAction("RegisterResult");
            }

            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            return View(RegisterMember);
        }

        public ActionResult RegisterResult()
        {
            return View();
        }

        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            return Json(membersService.AccountCheck(RegisterMember.NewMember.Account),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }
        #endregion

        #region 登入
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Guestbooks");
            return View();
        }

        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);

            if (String.IsNullOrEmpty(ValidateStr))
            {
                string RoleData = membersService.GetRole(LoginMember.Account);

                JwtService jwtService = new JwtService();

                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);

                HttpCookie cookie = new HttpCookie(cookieName)
                {
                    Value = Server.UrlEncode(Token)
                };

                Response.Cookies.Add(cookie);

                Response.Cookies[cookieName].Expires =
                    DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                return RedirectToAction("Index", "Guestbooks");
            }
            else
            {
                ModelState.AddModelError("", ValidateStr);
                return View(LoginMember);
            }
        }
        #endregion

        #region 登出

        [Authorize]
        public ActionResult Logout()
        {
            string cookieName = WebConfigurationManager.AppSettings["cookieName"].ToString();
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            cookie.Values.Clear();

            Response.Cookies.Set(cookie);

            return RedirectToAction("Login");
        }
        #endregion

        #region 修改留言

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel ChangeData)
        {
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }

            return View();
        }

        #endregion
    }

}