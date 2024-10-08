using ImgRepo.Data.Interface;

namespace ImgRepo.Service.Dto
{
    /// <summary>
    /// 建立相簿的相關資料
    /// </summary>
    public class NewAlbumDto : IBasicObjectDto
    {
        /// <summary>
        /// 相簿名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 說明或描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 標籤
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        public string[] Categories { get; set; }

        NewAlbumDto(IBasicUploadModel uploadModel)
        {
            this.Name = uploadModel.Name;
            this.Description = uploadModel.Description;
            this.Tags = uploadModel.Tags.SplitNoThrow(GlobalSettings.KeywordSplitter);
            this.Categories = uploadModel.Categories.SplitNoThrow(GlobalSettings.KeywordSplitter);
        }

        public static NewAlbumDto FromUploadModel(IBasicUploadModel uploadModel)
        {
            return new NewAlbumDto(uploadModel);
        }
    }
}
