using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebApplication3.Models;
using WebApplication3.Services;
using WebApplication3.ViewModels;

namespace WebApplication3.Controllers
{
    public class ImgCarouselController : Controller
    {
        private readonly ImgCarouselDBService ImgCarouselService= new ImgCarouselDBService();


        // GET: ImgCarousel
        public ActionResult Index()
        {
            ImgCarouselViewModel Data = new ImgCarouselViewModel();
            Data.DataList = ImgCarouselService.GetDataList();
            return View(Data);
        }
    }
}