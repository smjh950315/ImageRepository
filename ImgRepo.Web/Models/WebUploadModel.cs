using Cyh.Net;
using ImgRepo.Data.Interface;
using ImgRepo.Service;
using ImgRepo.Service.Dto;

namespace ImgRepo.Web.Models
{
    public class WebUploadModel : IBasicUploadModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public IEnumerable<IFormFile> Files { get; set; } = null!;

        public bool HasMultipleFiles() => this.Files.Count() > 1;

        public IEnumerable<NewImageDto> GetNewImageDtos()
        {
            if (this.Files.IsNullOrEmpty()) return Enumerable.Empty<NewImageDto>();
            List<NewImageDto> result = new List<NewImageDto>();
            var tags = this.Tags.SplitNoThrow(',');
            var categories = this.Categories.SplitNoThrow(',');
            foreach (var file in this.Files)
            {
                using (Stream str = file.OpenReadStream())
                {
                    result.Add(NewImageDto.FromBasicUploadModel(this, file.FileName, str, tags, categories));
                }
            }
            return result;
        }

        public static implicit operator NewImageDto?(WebUploadModel wfm)
        {
            if (wfm.Files.IsNullOrEmpty())
            {
                return null;
            }
            using (Stream str = wfm.Files.First().OpenReadStream())
            {
                return NewImageDto.FromBasicUploadModel(wfm, wfm.Files.First().FileName, str);
            }
        }
    }
}
