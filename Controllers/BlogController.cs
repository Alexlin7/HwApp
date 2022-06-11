using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HwApp1410931031.Services;
using HwApp1410931031.ViewModels;

namespace HwApp1410931031.Controllers
{
    public class BlogController : Controller
    {
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();

        #region 部落格頁面
        // GET: Blog
        public ActionResult Index(string Account)
        {
            BlogViewModel Data = new BlogViewModel();
            Data.Member = membersService.GetDatabyAccount(Account);
            return View(Data);
        }
        #endregion
    }
}