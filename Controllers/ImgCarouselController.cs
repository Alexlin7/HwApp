using System.Web.Mvc;
using HwApp1410931031.Services;
using HwApp1410931031.ViewModels;

namespace HwApp1410931031.Controllers
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