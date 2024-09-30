using ImgRepo.Service;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.Text;
using ImgRepo.Model.ViewModel;

namespace ImgRepo.Web.Controllers
{
    [ApiController]
    [Route("api/save")]
    public class ApiSaveController : Controller
    {
        private readonly IImageService _imageService;
        public ApiSaveController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpPost]
        [Route("upload")]
        public IActionResult Upload(UploadModel uploadModel)
        {
            if (uploadModel.File == null)
            {
                ModelState.AddModelError("UploadBytes", "Please upload an image");
                return this.NoContent();
            }
            var str = uploadModel.File.Base64;
            var buffer = Convert.FromBase64String(str);
            _imageService.SaveImageFile(new BasicDetails
            {
                Name = uploadModel.Name,
                Description = uploadModel.Description
            }, buffer);
            return this.Ok();
        }
    }
}
