using Cyh.Net;
using ImgRepo.Data.Interface;

namespace ImgRepo.Service.Dto
{
    public class NewArtistDto
    {
        public string ArtistName { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }
        public string[] NickNames { get; set; }
        public string[] Websites { get; set; }

        NewArtistDto(IBasicUploadModel uploadModel, string[] nickNames, string[] websites)
        {
            this.ArtistName = uploadModel.Name;
            this.Description = uploadModel.Description;
            this.Tags = uploadModel.Tags.IsNullOrEmpty() ? Array.Empty<string>() : uploadModel.Tags.Split(',');
            this.Categories = uploadModel.Categories.IsNullOrEmpty() ? Array.Empty<string>() : uploadModel.Categories.Split(',');
            this.NickNames = nickNames;
            this.Websites = websites;
        }
    }
}