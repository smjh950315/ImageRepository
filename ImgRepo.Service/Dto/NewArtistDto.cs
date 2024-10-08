﻿using ImgRepo.Data.Interface;

namespace ImgRepo.Service.Dto
{
    /// <summary>
    /// 建立藝術家資料檔的相關資料
    /// </summary>
    public class NewArtistDto : IBasicObjectDto
    {
        /// <summary>
        /// 藝術家名稱
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

        /// <summary>
        /// 綽號或是代稱
        /// </summary>
        public string[] NickNames { get; set; }

        /// <summary>
        /// 網站
        /// </summary>
        public string[] Websites { get; set; }

        NewArtistDto(IBasicUploadModel uploadModel, string[] nickNames, string[] websites)
        {
            this.Name = uploadModel.Name;
            this.Description = uploadModel.Description;
            this.Tags = uploadModel.Tags.SplitNoThrow(GlobalSettings.KeywordSplitter);
            this.Categories = uploadModel.Categories.SplitNoThrow(GlobalSettings.KeywordSplitter);
            this.NickNames = nickNames;
            this.Websites = websites;
        }

        public static NewArtistDto FromUploadModel(IBasicUploadModel uploadModel, string[] nickNames, string[] websites)
        {
            return new NewArtistDto(uploadModel, nickNames, websites);
        }
    }
}
