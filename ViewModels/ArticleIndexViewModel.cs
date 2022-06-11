using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using HwApp1410931031.Models;
using HwApp1410931031.Services;

namespace HwApp1410931031.ViewModels
{
    public class ArticleIndexViewModel
    {
        [DisplayName("搜尋：")]
        public string Search { get; set; }

        public List<Article> DataList { get; set; }

        public ForPaging Paging { get; set; }

        public string Account { get; set; }
    }
}