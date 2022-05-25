using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HwApp1410931031.Models
{
    public class Guestbooks
    {

        [DisplayName("編號：")]
        public int Id { get; set; }
        [DisplayName("名字：")]
        [Required(ErrorMessage = "請輸入名字")]
        [StringLength(20, ErrorMessage = "名字不可超過20 字元")]
        public string Name { get; set; }
        [DisplayName("留言內容：")]
        [Required(ErrorMessage = "請輸入留言內容")]
        [StringLength(100, ErrorMessage = "留言內容不可超過100 字元")]
        public string Content { get; set; }
        [DisplayName("新增時間：")]
        public DateTime CreateTime { get; set; }
        [DisplayName("回覆內容：")]
        [StringLength(100, ErrorMessage = "回覆內容不可超過100 字元")]
        public string Reply { get; set; }
        [DisplayName("回覆時間：")]
        public DateTime? ReplyTime { get; set; }


    }
}