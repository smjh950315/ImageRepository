using Cyh.Net;
using ImgRepo.Model.Common;
using ImgRepo.Service;
using ImgRepo.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImgRepo.Web.Controllers
{
    public class ImageController : Controller
    {
        readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            this._imageService = imageService;
        }

        public IActionResult Index(long id)
        {
            BasicDetails? imgDetail = this._imageService.GetBasicDetails(id);
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
        public async Task<IActionResult> Upload(WebUploadModel uploadModel)
        {
            if (uploadModel == null || uploadModel.Files.IsNullOrEmpty())
            {
                this.ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.Upload();
            }
            long? newImage = null;
            if (uploadModel.HasMultipleFiles())
            {
                var dtos = uploadModel.GetNewImageDtos();
                newImage = await this._imageService.BatchCreateImageAsync(dtos, true);
                return this.RedirectToAction("Index", "Image", new { id = newImage });
            }
            else
            {
                newImage = await this._imageService.CreateImageAsync(uploadModel);
            }
            return this.RedirectToAction("Index", "Image", new { id = newImage });
        }
    }
}
