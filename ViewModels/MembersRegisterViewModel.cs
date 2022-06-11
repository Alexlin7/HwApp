using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HwApp1410931031.Models;

namespace HwApp1410931031.ViewModels
{
    public class MembersRegisterViewModel
    {
        [DisplayName("大頭照")]
        public HttpPostedFileBase MembersImage { get; set; }

        public Members NewMember { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }

        [DisplayName("確認密碼")]
        [Compare("Password", ErrorMessage = "兩次密碼不一致")]
        [Required(ErrorMessage = "請輸入確認密碼")]
        public string PasswordCheck { get; set; }
    }
}