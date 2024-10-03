using Cyh.Net;
using ImgRepo.Model.ApiModel;
using ImgRepo.Model.Interface;
using ImgRepo.Service.Helpers;

namespace ImgRepo.Service.Dto
{
    public class NewImageDto
    {
        public string ImageName { get; set; }
        public string? Description { get; set; }
        public string FileName { get; set; }
        public string Format { get; set; }
        public byte[] Data { get; set; }
        public byte[] ThumbData { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }

        NewImageDto(IBasicUploadModel uploadModel, string filename, Stream stream)
        {
            this.FileName = filename;
            this.Data = new byte[stream.Length];
            stream.Read(this.Data, 0, this.Data.Length);
            this.ThumbData = ImageHelper.Resize(this.Data, 256, 256);
            this.ImageName = uploadModel.Name.IsNullOrEmpty() ? filename : uploadModel.Name;
            this.Format = ImageHelper.GetFormat(this.Data);
            this.Description = uploadModel.Description;
            this.Tags = uploadModel.Tags.IsNullOrEmpty() ? Array.Empty<string>() : uploadModel.Tags.Split(',');
            this.Categories = uploadModel.Categories.IsNullOrEmpty() ? Array.Empty<string>() : uploadModel.Categories.Split(',');
        }
        NewImageDto(IBasicUploadModel uploadModel, string filename, byte[] binaryData)
        {
            this.FileName = filename;
            this.Data = binaryData;
            this.ThumbData = ImageHelper.Resize(this.Data, 256, 256);
            this.ImageName = uploadModel.Name.IsNullOrEmpty() ? filename : uploadModel.Name;
            this.Format = ImageHelper.GetFormat(this.Data);
            this.Description = uploadModel.Description;
            this.Tags = uploadModel.Tags.IsNullOrEmpty() ? Array.Empty<string>() : uploadModel.Tags.Split(',');
            this.Categories = uploadModel.Categories.IsNullOrEmpty() ? Array.Empty<string>() : uploadModel.Categories.Split(',');
        }

        public static NewImageDto FromBasicUploadModel(IBasicUploadModel uploadModel, string filename, Stream stream)
        {
            return new NewImageDto(uploadModel, filename, stream);
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
