using ImgRepo.Model.ApiModel;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service;
using Microsoft.AspNetCore.Mvc;

namespace ImgRepo.Web.Controllers
{
    [ApiController]
    [Route("api/save")]
    public class ApiSaveController : Controller
    {
        private readonly IImageService _imageService;

        public ApiSaveController(IImageService imageService)
        {
            this._imageService = imageService;
        }

        [HttpGet]
        [Route("tag/add/{imgId}/{tagName}")]
        public BasicDetails? AddTag(long imgId, string tagName)
        {
            var tagId = _imageService.UpdateImageTag(imgId, tagName, false);
            return tagId > 0 ? _imageService.GetTagDetails(tagId) : null;
        }
        [HttpGet]
        [Route("tag/remove/{imgId}/{tagName}")]
        public BasicDetails? RemoveTag(long imgId, string tagName)
        {
            var tagId = _imageService.UpdateImageTag(imgId, tagName, true);
            return tagId > 0 ? _imageService.GetTagDetails(tagId) : null;
        }

        [HttpPost]
        [Route("upload")]
        public IActionResult Upload(ApiUploadModel uploadModel)
        {
            if (uploadModel.File == null)
            {
                this.ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.NoContent();
            }
            this._imageService.UploadImage(uploadModel);
            return this.Ok();
        }
    }
}
