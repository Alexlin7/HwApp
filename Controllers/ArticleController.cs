using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HwApp1410931031.Models;
using HwApp1410931031.Services;
using HwApp1410931031.ViewModels;

namespace HwApp1410931031.Controllers
{
    public class ArticleController : Controller
    {
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();
        //宣告Article資料表的Service物件
        private readonly ArticleDBService articleService = new ArticleDBService();
        //宣告Message資料表的Service物件
        private readonly MessageDBService messageService = new MessageDBService();

        #region 起始頁
        // GET: Article
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 文章列表
        //將Page(頁數)預設為1
        public ActionResult List(string Search, string Account, int Page = 1)
        {
            //宣告一個新頁面模型
            ArticleIndexViewModel Data = new ArticleIndexViewModel();
            //將傳入值Search(搜尋)放入頁面模型中
            Data.Search = Search;
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(Page);
            //將此文章擁有者的Account(帳號)放入頁面模型中
            Data.Account = Account;
            //從Service中取得頁面所需陣列資料
            Data.DataList = articleService.GetDataList(Data.Paging, Data.Search, Data.Account);
            return PartialView(Data); //將頁面資料傳入View中
        }
        #endregion

        #region 文章頁面
        //文章根據傳入的編號來決定要顯示的資料
        public ActionResult Article(int A_Id)
        {
            //新開首頁則取最後一筆
            ArticleViewModel Data = new ArticleViewModel();
            //增加觀看數
            articleService.AddWatch(A_Id);
            Data.article = articleService.GetArticleDataById(A_Id);
            ForPaging paging = new ForPaging(0); //確定是否有留言資料預設0
            Data.DataList = messageService.GetDataList(paging, A_Id);
            return View(Data);
        }
        #endregion

        #region 新增文章
        //新增文章一開始載入頁面
        [Authorize] // 設定此Action必須登入
        public ActionResult Create()
        {
            return PartialView();
        }
        //新增文章傳入資料時的Action
        [Authorize] // 設定此Action必須登入
        [HttpPost] // 設定此Action只接受頁面Post資料傳入
        public ActionResult Create([Bind(Include = "Title,Content")] Article Data)
        {
            Data.Account = User.Identity.Name;
            articleService.InsertArticle(Data);
            return RedirectToAction("Index", "Blog", new { Account = User.Identity.Name });
        }
        #endregion

        #region 修改文章
        //修改文章要根據傳入的文章編號決定要修改的資料
        [Authorize] // 設定此Action必須登入
        public ActionResult EditPage(int A_Id)
        {
            Article Data = new Article();
            Data = articleService.GetArticleDataById(A_Id);
            return PartialView(Data);
        }
        //修改文章傳入資料的Action
        [Authorize] // 設定此Action必須登入
        [HttpPost] // 設定此Action只接受頁面Post資料傳入
        public ActionResult EditPage(int A_Id, Article Data)
        {
            //判斷是否可以修改此文章，有回文則不行
            if (articleService.CheckUpdate(A_Id))
            {
                articleService.UpdateArticle(Data);
            }
            return RedirectToAction("Article", new { A_Id = A_Id });
        }
        #endregion

        #region 刪除文章
        //刪除文章要根據傳入的文章編號來刪除資料
        [Authorize] // 設定此Action只有登入才能使用
        public ActionResult Delete(int A_Id)
        {
            articleService.DeleteArticle(A_Id);
            return RedirectToAction("Index", "Blog", new { Account = User.Identity.Name });
        }
        #endregion

        #region 顯示人氣
        public ActionResult ShowPopularity(string Account)
        {
            //宣告一個新頁面模型
            ArticleIndexViewModel Data = new ArticleIndexViewModel();
            //取得頁面所需的人氣資料陣列
            Data.DataList = articleService.GetPopularList(Account);
            //將資料傳入View中
            return View(Data);
        }
        #endregion


    }
}