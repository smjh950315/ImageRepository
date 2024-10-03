using ImgRepo.Model.Interface;
using ImgRepo.Service.Dto;

namespace ImgRepo.Web.Models
{
    public class WebUploadModel : IBasicUploadModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;

        public static implicit operator NewImageDto(WebUploadModel wfm)
        {
            using (Stream str = wfm.File.OpenReadStream())
            {
                return NewImageDto.FromBasicUploadModel(wfm, wfm.File.FileName, wfm.File.OpenReadStream());
            }
        }
    }
}
