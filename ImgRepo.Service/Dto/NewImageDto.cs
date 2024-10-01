using ImgRepo.Model.ApiModel;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Helpers;

namespace ImgRepo.Service.Dto
{
    public class NewImageDto
    {
        public string ImageName { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Format { get; set; }
        public byte[] Data { get; set; }
        public byte[] ThumbData { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }

        public static implicit operator NewImageDto(ApiUploadModel model)
        {
            byte[] data = Convert.FromBase64String(model.File.Base64);
            return new NewImageDto
            {
                ImageName = model.Name,
                Description = model.Description,
                FileName = model.File.FileName,
                Format = model.File.Format,
                Data = data,
                ThumbData = ImageHelper.Resize(data, 256, 256),
                Tags = model.Tags,
                Categories = model.Categories
            };
        }
        public static implicit operator NewImageDto(WebUploadModel uploadModel)
        {
            return new NewImageDto
            {
                ImageName = uploadModel.ImageName,
                Description = uploadModel.Description,
                FileName = uploadModel.FileName,
                Format = ImageHelper.GetFormat(uploadModel.FileData),
                Data = uploadModel.FileData,
                ThumbData = ImageHelper.Resize(uploadModel.FileData, 256, 256),
                Tags = uploadModel.Tags,
                Categories = uploadModel.Categories
            };
        }
    }
}
