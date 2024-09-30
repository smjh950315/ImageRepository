using ImgRepo.Model.ViewModel;
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
            _imageService = imageService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new List<ImageAdvanceInformation>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetImage(long imgFileId)
        {
            var bytes = _imageService.GetFileBytes(imgFileId);
            return File(bytes, "image/jpeg");
        }

        public IActionResult Upload()
        {
            return View(new ImageUploadModel());
        }

        [HttpPost]
        public IActionResult Upload(ImageUploadModel imageBaseInfo)
        {
            if (imageBaseInfo.ImageFile == null)
            {
                ModelState.AddModelError("UploadBytes", "Please upload an image");
                return View(imageBaseInfo);
            }
            var str = imageBaseInfo.ImageFile.OpenReadStream();
            var buffer = new byte[str.Length];
            str.Read(buffer, 0, buffer.Length);
            _imageService.SaveImage(imageBaseInfo, buffer);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
