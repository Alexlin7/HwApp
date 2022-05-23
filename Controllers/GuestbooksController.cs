using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebApplication3.Models;
using WebApplication3.Services;
using WebApplication3.ViewModels;


namespace WebApplication3.Controllers
{
    public class GuestbooksController : Controller
    {
        //宣告Guestbooks資料表的Service物件
        private readonly GuestbooksDBService GuestbookService = new GuestbooksDBService();


        // GET: Guestbooks
        public ActionResult Index()
        {
            // 宣告一個新頁面模型
            GuestbooksViewModel Data = new GuestbooksViewModel();
            //從Service 中取得頁面所需陣列資料
            Data.DataList = GuestbookService.GetDataList();
            //將頁面資料傳入View 中
            return View(Data);

        }

        #region 新增留言
        // 新增留言一開始載入頁面
        public ActionResult Create()
        {
            return PartialView(); // 因為此頁面用於載入至開始頁面中，故使用部分檢視回傳
        }
        // 新增留言傳入資料時的Action
        [HttpPost] // 設定此Action 只接受頁面POST 資料傳入
                   // 使用Bind 的 Include 來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Create([Bind(Include = "Name, Content")] Guestbooks Data)
        {
            GuestbookService.InsertGuestbooks(Data); // 使用Service 來新增一筆資料
            return RedirectToAction("Index"); // 重新導向頁面至開始頁面
        }
        #endregion

        #region 修改留言
        // 修改留言頁面要根據傳入編號來決定要修改的資料
        public ActionResult Edit(int Id)
        {
            // 取得頁面所需資料，藉由Service 取得
            Guestbooks Data = GuestbookService.GetDataById(Id);
            // 將資料傳入View 中
            return View(Data);
        }

        // 修改留言傳入資料時的Action
        [HttpPost] // 設定此Action 只接受頁面POST 資料傳入
                   // 使用Bind 的Inculde 來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Edit(int Id, [Bind(Include = "Name, Content")] Guestbooks UpdateData)
        {
            // 修改資料的是否可修改判斷
            if (GuestbookService.CheckUpdate(Id))
            {
                // 將編號設定至修改資料中
                UpdateData.Id = Id;
                // 使用Service 來修改資料
                GuestbookService.UpdateGuestbooks(UpdateData);
                // 重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
            else
            {
                // 重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 回覆留言
        // 回覆留言頁面要根據傳入編號來決定要回覆的資料
        public ActionResult Reply(int Id)
        {
            // 取得頁面所需資料，藉由Service 取得
            Guestbooks Data = GuestbookService.GetDataById(Id);
            // 將資料傳入View 中
            return View(Data);
        }
        // 修改留言傳入資料時的Action
        [HttpPost] // 設定此Action 只接受頁面POST 資料傳入
                   // 使用Bind 的Inculde 來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Reply(int Id, [Bind(Include = "Reply,ReplyTime")] Guestbooks ReplyData)
        {
            // 修改資料的是否可修改判斷
            if (GuestbookService.CheckUpdate(Id))
            {
                // 將編號設定至回覆留言資料中
                ReplyData.Id = Id;
                // 使用Service 來回覆留言資料
                GuestbookService.ReplyGuestbooks(ReplyData);
                // 重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
            else
            {
                // 重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
        }
        #endregion

    }
}