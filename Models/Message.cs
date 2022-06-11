using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HwApp1410931031.Models
{
    public class Message
    {
        public int A_Id { get; set; }

        public int M_Id { get; set; }

        [DisplayName("留言帳號：")]
        public string Account { get; set; }

        [DisplayName("留言內容：")]
        public string Content { get; set; }

        [DisplayName("留言時間：")]
        public DateTime CreateTime { get; set; }

        public Members Member { get; set; } = new Members();
    }
}