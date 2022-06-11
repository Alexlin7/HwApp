using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HwApp1410931031.Models
{
    public class Article
    {
        [DisplayName("文章編號")]
        //文章編號
        public int A_Id { get; set; }
        [DisplayName("標題")]
        [Required(ErrorMessage = "請輸入標題")]
        [StringLength(50, ErrorMessage = "標題長度最多50字元")]
        //標題
        public string Title { get; set; }
        [DisplayName("文章內容")]
        [Required(ErrorMessage = "請輸入文章內容")]
        //內容
        public string Content { get; set; }
        [DisplayName("發表者")]
        //會員帳號
        public string Account { get; set; }
        [DisplayName("新增時間")]
        //新增時間
        public DateTime CreateTime { get; set; }
        [DisplayName("觀看人數")]
        //瀏覽人數
        public int Watch { get; set; }
        //Members資料表（外來鍵）
        //預設時就將Members物件建立完畢
        public Members Member { get; set; } = new Members();
    }
}