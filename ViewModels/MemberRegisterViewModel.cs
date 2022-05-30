using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HwApp1410931031.Models;

namespace HwApp1410931031.ViewModels
{
    public class MemberRegisterViewModel
    {
        public  Members NewMember { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Passowrd { get; set; }

        [DisplayName("確認密碼")]
        [Compare("Password", ErrorMessage = "兩次密碼不一致")]
        [Required(ErrorMessage = "請輸入確認密碼")]
        public string PasswordCheck { get; set; }
    }
}