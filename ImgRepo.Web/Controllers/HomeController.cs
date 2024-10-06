using ImgRepo.Service;
using ImgRepo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ImgRepo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IImageService _imageService;
        public HomeController(IImageService imageService, ILogger<HomeController> logger)
        {
            this._imageService = imageService;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult ImageBrowse()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
