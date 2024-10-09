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
        private readonly ICommonAttributeService _commonAttributeService;
        public ApiSaveController(IImageService imageService, ICommonAttributeService commonAttributeService)
        {
            this._imageService = imageService;
            this._commonAttributeService = commonAttributeService;
        }

        [HttpPost]
        [Route("image/name")]
        public IActionResult RenameImage(ApiAttributeEdit apiAttributeEdit)
        {
            long newId = this._imageService.Rename(apiAttributeEdit.Id, apiAttributeEdit.Value ?? String.Empty);
            return newId > 0 ? this.Ok() : this.BadRequest();
        }

        [HttpPost]
        [Route("image/tag/add")]
        public BasicDetails? AddTag(ApiAttributeEdit apiAttributeEdit)
        {
            long tagId = this._imageService.AddTag(apiAttributeEdit.Id, apiAttributeEdit.Value ?? String.Empty);
            return tagId > 0 ? this._commonAttributeService.GetTagDetail(tagId) : null;
        }

        [HttpPost]
        [Route("image/tag/remove")]
        public BasicDetails? RemoveTag(ApiAttributeEdit apiAttributeEdit)
        {
            long tagId = this._imageService.RemoveTag(apiAttributeEdit.Id, apiAttributeEdit.Value ?? String.Empty);
            return tagId > 0 ? this._commonAttributeService.GetTagDetail(tagId) : null;
        }

        [HttpPost]
        [Route("image/category/add")]
        public BasicDetails? AddCategory(ApiAttributeEdit apiAttributeEdit)
        {
            long cateId = this._imageService.AddCategory(apiAttributeEdit.Id, apiAttributeEdit.Value ?? String.Empty);
            return cateId > 0 ? this._commonAttributeService.GetCategoryDetail(cateId) : null;
        }

        [HttpPost]
        [Route("image/category/remove")]
        public BasicDetails? RemoveCategory(ApiAttributeEdit apiAttributeEdit)
        {
            long cateId = this._imageService.RemoveCategory(apiAttributeEdit.Id, apiAttributeEdit.Value ?? String.Empty);
            return cateId > 0 ? this._commonAttributeService.GetCategoryDetail(cateId) : null;
        }

        [HttpPost]
        [Route("image/upload")]
        public async Task<IActionResult> Upload(ApiUploadModel uploadModel)
        {
            if (uploadModel.File == null)
            {
                this.ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.NoContent();
            }
            var newImageId = await this._imageService.CreateImageAsync(uploadModel);
            if (newImageId == 0) return this.NoContent();
            return this.Ok();
        }
    }
}
