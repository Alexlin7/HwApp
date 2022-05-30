using System.Collections.Generic;
using System.ComponentModel;
using HwApp1410931031.Models;
using HwApp1410931031.Services;

namespace HwApp1410931031.ViewModels
{
    public class GuestbooksViewModel
    {
        // 搜尋欄位
        [DisplayName(" 搜尋：")]
        public string Search { get; set; }

        //顯示資料陣列
        public List<Guestbooks> DataList { get; set; }

        // 分頁內容
        public ForPaging Paging { get; set; }

    }
}