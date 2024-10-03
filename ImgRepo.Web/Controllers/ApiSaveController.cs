using ImgRepo.Model.Common;
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
        [Route("rename/image/{imgId}/{newName}")]
        public IActionResult RenameImage(long imgId, string newName)
        {
            long newId = this._imageService.RenameImage(imgId, newName);
            return newId > 0 ? this.Ok() : this.NoContent();
        }

        [HttpGet]
        [Route("tag/add/{imgId}/{tagName}")]
        public BasicDetails? AddTag(long imgId, string tagName)
        {
            long tagId = this._imageService.AddTag(imgId, tagName);
            return tagId > 0 ? this._imageService.GetTagDetail(tagId) : null;
        }

        [HttpGet]
        [Route("tag/remove/{imgId}/{tagName}")]
        public BasicDetails? RemoveTag(long imgId, string tagName)
        {
            long tagId = this._imageService.RemoveTag(imgId, tagName);
            return tagId > 0 ? this._imageService.GetTagDetail(tagId) : null;
        }

        [HttpGet]
        [Route("category/add/{imgId}/{tagName}")]
        public BasicDetails? AddCategory(long imgId, string tagName)
        {
            long cateId = this._imageService.AddCategory(imgId, tagName);
            return cateId > 0 ? this._imageService.GetCategoryDetail(cateId) : null;
        }

        [HttpGet]
        [Route("category/remove/{imgId}/{catName}")]
        public BasicDetails? RemoveCategory(long imgId, string catName)
        {
            long cateId = this._imageService.RemoveCategory(imgId, catName);
            return cateId > 0 ? this._imageService.GetCategoryDetail(cateId) : null;
        }

        [HttpPost]
        [Route("image/upload")]
        public IActionResult Upload(ApiUploadModel uploadModel)
        {
            if (uploadModel.File == null)
            {
                this.ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.NoContent();
            }
            this._imageService.CreateImage(uploadModel);
            return this.Ok();
        }
    }
}
