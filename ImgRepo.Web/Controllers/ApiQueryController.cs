using ImgRepo.Model.Common;
using ImgRepo.Model.Image;
using ImgRepo.Model.Query;
using ImgRepo.Service;
using Microsoft.AspNetCore.Mvc;

namespace ImgRepo.Web.Controllers
{
    [ApiController]
    [Route("api/query")]
    public class ApiQueryController : Controller
    {
        private readonly IImageService _imageService;
        private readonly ICommonAttributeService _commonAttributeService;
        public ApiQueryController(IImageService imageService, ICommonAttributeService commonAttributeService)
        {
            this._imageService = imageService;
            this._commonAttributeService = commonAttributeService;
        }

        [HttpPost]
        [Route("image/thumbnails")]
        public IEnumerable<ApiThumbFileModel> GetThumbnails(QueryModel? queryModel)
        {
            GC.Collect();
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
        [Route("image/tags/{imgId}")]
        public IEnumerable<BasicInfo> GetTags(long imgId)
        {
            return this._imageService.GetTags(imgId);
        }

        [HttpGet]
        [Route("image/categories/{imgId}")]
        public IEnumerable<BasicInfo> GetCategories(long imgId)
        {
            return this._imageService.GetCategories(imgId);
        }

        [HttpGet]
        [Route("image/author/{imgId}")]
        public BasicDetails? GetAuthor(long imgId)
        {
            return this._imageService.GetBasicDetails(imgId);
        }
    }
}
