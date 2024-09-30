using ImgRepo.Model.ViewModel;

namespace ImgRepo.Web.Models
{
    public class ImageUploadModel : ImageBasicInformation
    {
        public IFormFile ImageFile { get; set; }
    }
}
