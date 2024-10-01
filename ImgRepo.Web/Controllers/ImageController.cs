using ImgRepo.Model.ViewModel;
using ImgRepo.Service;
using ImgRepo.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImgRepo.Web.Controllers
{
    public class ImageController : Controller
    {
        IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            this._imageService = imageService;
        }
        public IActionResult Index(long id)
        {
            var imgDetail = _imageService.GetImageDetails(id);
            if (imgDetail == null)
            {
                return this.View(new BasicDetails());
            }
            return this.View(imgDetail);
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return this.View();
        }
        [HttpPost]
        public IActionResult Upload(WebFileModel uploadModel)
        {
            if (uploadModel == null)
            {
                this.ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.Upload();
            }
            var newImage = this._imageService.UploadImage(uploadModel);
            return this.RedirectToAction("Index", "Image", new { id = newImage });
        }
    }
}
