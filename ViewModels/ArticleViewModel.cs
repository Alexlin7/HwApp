using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HwApp1410931031.Models;


namespace HwApp1410931031.ViewModels
{
    public class ArticleViewModel
    {
        public Article article { get; set; }

        public List<Message> DataList { get; set; }
    }
}