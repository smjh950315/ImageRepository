using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Dto;

namespace ImgRepo.Web.Models
{
    public class WebFileModel
    {
        public string ImageName { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Categories { get; set; }
        public IFormFile File { get; set; }

        public static implicit operator NewImageDto(WebFileModel wfm)
        {
            byte[] bytes;
            using (var str = wfm.File.OpenReadStream())
            {
                bytes = new byte[str.Length];
                str.Read(bytes, 0, bytes.Length);
            }
            string[] tags = wfm.Tags.Split(',');
            string[] categories = wfm.Categories.Split(',');
            return new WebUploadModel
            {
                ImageName= wfm.ImageName,
                Description = wfm.Description,
                Tags = tags,
                Categories = categories,
                FileName = wfm.File.FileName,
                FileData = bytes,
            };
        }
    }
}
