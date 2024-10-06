using Cyh.Net;
using ImageHelperSharp;
using ImageHelperSharp.Common;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
namespace ImgRepo.Service.Dto
{
    public class NewImageDto
    {
        public string ImageName { get; set; }
        public string? Description { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public byte[] ThumbData { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }
        public ImInfo ImInfo { get; set; }

        NewImageDto(IBasicUploadModel uploadModel, string filename, byte[] binaryData, string[] tags, string[] categories)
        {
            this.FileName = filename;
            this.Data = binaryData;
            this.ThumbData = StbService.Resize(this.Data, 256, 256);
            this.ImageName = uploadModel.Name.IsNullOrEmpty() ? filename : uploadModel.Name;
            this.ImInfo = StbService.GetImageFileInfo(this.Data);
            this.Description = uploadModel.Description;
            this.Tags = tags;
            this.Categories = categories;
        }
        NewImageDto(IBasicUploadModel uploadModel, string filename, Stream stream)
            : this(uploadModel, filename, stream.GetBytes())
        {
        }
        NewImageDto(IBasicUploadModel uploadModel, string filename, byte[] binaryData)
            : this(uploadModel, filename, binaryData, uploadModel.Tags.SplitNoThrow(GlobalSettings.KeywordSplitter), uploadModel.Categories.SplitNoThrow(GlobalSettings.KeywordSplitter))
        {
        }

        public static NewImageDto FromBasicUploadModel(IBasicUploadModel uploadModel, string filename, Stream stream)
        {
            return new NewImageDto(uploadModel, filename, stream);
        }

        public static NewImageDto FromBasicUploadModel(IBasicUploadModel uploadModel, string filename, Stream stream, string[] tags, string[] categories)
        {
            return new NewImageDto(uploadModel, filename, stream.GetBytes(), [], [])
            {
                Tags = tags,
                Categories = categories
            };
        }

        public static NewImageDto FromBasicUploadModel(IBasicUploadModel uploadModel, string filename, byte[] binaryData)
        {
            return new NewImageDto(uploadModel, filename, binaryData);
        }

        public static implicit operator NewImageDto(ApiUploadModel model)
        {
            byte[] data = Convert.FromBase64String(model.File.Base64);
            return new NewImageDto(model, model.File.FileName, data);
        }
    }
}
