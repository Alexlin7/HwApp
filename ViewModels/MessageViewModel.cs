using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HwApp1410931031.Models;
using HwApp1410931031.Services;

namespace HwApp1410931031.ViewModels
{
    public class MessageViewModel
    {
        //顯示資料陣列
        public List<Message> DataList { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
        //文章編號
        public int A_Id { get; set; }
    }
}