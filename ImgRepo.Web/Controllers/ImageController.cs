using ImgRepo.Model.Common;
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
            BasicDetails? imgDetail = this._imageService.GetImageDetail(id);
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
        public IActionResult Upload(WebUploadModel uploadModel)
        {
            if (uploadModel == null || uploadModel.File == null)
            {
                this.ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.Upload();
            }
            long newImage = this._imageService.CreateImage(uploadModel);
            return this.RedirectToAction("Index", "Image", new { id = newImage });
        }
    }
}
