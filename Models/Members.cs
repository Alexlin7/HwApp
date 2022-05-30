using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HwApp1410931031.Models
{
    public class Members
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "帳號長度需介於6-30字元")]
        [Remote("AccountCheck", "Members", ErrorMessage = "此帳號已被註冊過")]
        public string Account { get; set; }

        public string Password { get; set; }
        [DisplayName("名稱")]
        [StringLength(20, ErrorMessage = "名稱最多20字")]
        [Required(ErrorMessage = "請輸入名稱")]
        public string Name { get; set; }

        [DisplayName("電子信箱")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "長度最多200字元")]
        [EmailAddress(ErrorMessage = "這不是Emial格式")]
        public string Email { get; set; }

        public string AuthCode { get; set; }

        public bool IsAdmin { get; set; }
    }
}