using Cyh.Net;
using ImageHelperSharp;
using ImageHelperSharp.Common;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
namespace ImgRepo.Service.Dto
{
    /// <summary>
    /// 建立圖片的資料檔
    /// </summary>
    public class NewImageDto : IBasicObjectDto
    {
        /// <summary>
        /// 圖片名稱，未取名則使用檔名做為其名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 說明或描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 檔案資料
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 縮圖資料
        /// </summary>
        public byte[] ThumbData { get; set; }

        /// <summary>
        /// 標籤
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        public string[] Categories { get; set; }

        /// <summary>
        /// 圖像資訊
        /// </summary>
        public ImInfo ImInfo { get; set; }

        NewImageDto(IBasicUploadModel uploadModel, string filename, byte[] binaryData, string[] tags, string[] categories)
        {
            this.FileName = filename;
            this.Data = binaryData;
            this.ThumbData = StbService.Resize(this.Data, 256, 256);
            this.Name = uploadModel.Name.IsNullOrEmpty() ? filename : uploadModel.Name;
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
