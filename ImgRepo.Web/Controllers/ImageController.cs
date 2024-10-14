using ImgRepo.Model.Common;
using ImgRepo.Service;
using ImgRepo.Service.Dto;
using ImgRepo.Web.StreamFileHelper;
using Microsoft.AspNetCore.Mvc;

namespace ImgRepo.Web.Controllers
{
    public class ImageController : Controller
    {
        readonly IImageService _imageService;
        readonly IFileAccessService _fileAccessService;
        public ImageController(IImageService imageService, IFileAccessService fileAccessService)
        {
            this._imageService = imageService;
            this._fileAccessService = fileAccessService;
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

        // 測試中
        [HttpPost]
        [DisableFormValueModelBindingFilter]
        public async Task<IActionResult> StreamUpload(/*WebUploadModel uploadModel*/)
        {
            BatchNewImageDto batchNewImageDto = new BatchNewImageDto(this._fileAccessService);

            Microsoft.AspNetCore.Mvc.ModelBinding.FormValueProvider formProvider = await this.Request.StreamFile((f) =>
            {
                Guid guid = Guid.NewGuid();
                string uri = guid.ToString();
                return batchNewImageDto.WriteAndRecordFile(uri, f.FileName);
            });

            string tagStr = formProvider.GetValue("Tags").ToString();
            string cateStr = formProvider.GetValue("Categories").ToString();
            batchNewImageDto.SetTagsByUnsplitedString(tagStr);
            batchNewImageDto.SetCategoriesByUnsplitedString(cateStr);

            if (batchNewImageDto.AddedFileCount == 0)
            {
                return this.RedirectToAction("Upload", "Image");
            }
            if (batchNewImageDto.AddedFileCount == 1)
            {
                batchNewImageDto.Name = formProvider.GetValue("Name").ToString();
            }

            this._imageService.BatchCreateImage(batchNewImageDto);

            return this.RedirectToAction("Upload", "Image");
        }
    }
}
