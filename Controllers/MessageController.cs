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
    public class MessageController : Controller
    {
        //宣告Message資料表的Service物件
        private readonly MessageDBService messageService = new MessageDBService();

        #region 留言頁面
        // GET: Message
        public ActionResult Index(int A_Id = 1)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        #endregion

        #region 留言陣列
        //將Page(頁數)預設為1
        public ActionResult MessageList(int A_Id, int Page = 1)
        {
            MessageViewModel Data = new MessageViewModel();//宣告一個新頁面模型
            Data.Paging = new ForPaging(Page); //新增頁面模型中的分頁
            Data.A_Id = A_Id; //將傳入值文章編號入頁面模型中
                              //從Service中取得頁面所需陣列資料
            Data.DataList = messageService.GetDataList(Data.Paging, Data.A_Id);
            return PartialView(Data); //將頁面資料傳入View中
        }
        #endregion

        #region 新增留言
        //新增留言起始
        //[Authorize] // 設定此Action必須登入
        public ActionResult Create(int A_Id)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        [Authorize] // 設定此Action必須登入
        [HttpPost]
        //使用Bind的Include來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Add(int A_Id, [Bind(Include = "Content")] Message Data)
        {
            // 把A_Id放進去
            Data.A_Id = A_Id;
            //設定新增留言的新增者為登入者
            Data.Account = User.Identity.Name;
            // 使用Service來新增一筆資料
            messageService.InsertMessage(Data);
            return RedirectToAction("MessageList", new { A_Id = A_Id });
        }
        #endregion

        #region 修改留言
        //修改文章要根據傳入的文章編號來刪除資料
        [Authorize] // 登入才能改
        public ActionResult UpdateMessage(int A_Id, int M_Id, string Content)
        {
            Message message = new Message();
            message.A_Id = A_Id;
            message.M_Id = M_Id;
            message.Content = Content;
            messageService.UpdateMessage(message);
            //重新導向頁面至文章頁面
            return RedirectToAction("Article", "Article", new { A_Id = A_Id });
        }
        #endregion

        #region 刪除留言
        //刪除文章要根據傳入的文章編號來修改資料
        [Authorize] // 登入才能改
        public ActionResult DeleteMessage(int A_Id, int M_Id)
        {
            messageService.DeleteMessage(A_Id, M_Id);
            //重新導向頁面至文章頁面
            return RedirectToAction("Article", "Article", new { A_Id = A_Id });
        }
        #endregion

    }
}