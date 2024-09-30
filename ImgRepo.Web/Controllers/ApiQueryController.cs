using Cyh.Net;
using Cyh.Net.Data.Predicate;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ImgRepo.Web.Controllers
{
    [ApiController]
    [Route("api/query")]
    public class ApiQueryController : Controller
    {
        private readonly IImageService _imageService;
        public ApiQueryController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        [Route("thumbnails")]
        public IEnumerable<ImageThumbView> GetThumbnails(QueryModel? queryModel)
        {
            return _imageService.GetImageThumbViews(queryModel);
        }

        [HttpGet]
        [Route("file/{id}")]
        public IActionResult GetImage(long id)
        {
            var fid = _imageService.GetImageFileId(id);
            if(fid == 0)
            {
                return this.NotFound();
            }
            var bytes = _imageService.GetFileBytes(fid);
            if (bytes.IsNullOrEmpty())
            {
                return this.NotFound();
            }
            return this.File(bytes, "image/jpeg");
        }
    }
}
