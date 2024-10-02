using ImgRepo.Model.ApiModel;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service;
using Microsoft.AspNetCore.Mvc;

namespace ImgRepo.Web.Controllers
{
    [ApiController]
    [Route("api/query")]
    public class ApiQueryController : Controller
    {
        private readonly IImageService _imageService;
        public ApiQueryController(IImageService imageService)
        {
            this._imageService = imageService;
        }

        [HttpPost]
        [Route("thumbnails")]
        public IEnumerable<ApiFileModel> GetThumbnails(QueryModel? queryModel)
        {
            return this._imageService.GetThumbnails(queryModel);
        }

        [HttpGet]
        [Route("image/thumb/{id}")]
        public ApiFileModel? GetThumbnail(long id)
        {
            return this._imageService.GetThumbnail(id);
        }

        [HttpGet]
        [Route("image/file/{id}")]
        public ApiFileModel? GetImage(long id)
        {
            return this._imageService.GetFullImage(id);
        }

        [HttpGet]
        [Route("tags/{imgId}")]
        public IEnumerable<BasicInfo> GetTags(long imgId)
        {
            return this._imageService.GetImageTags(imgId);
        }

        [HttpGet]
        [Route("categories/{imgId}")]
        public IEnumerable<BasicInfo> GetCategories(long imgId)
        {
            return this._imageService.GetImageCategories(imgId);
        }

        [HttpGet]
        [Route("author/{imgId}")]
        public BasicDetails? GetAuthor(long imgId)
        {
            return this._imageService.GetImageDetail(imgId);
        }
    }
}
