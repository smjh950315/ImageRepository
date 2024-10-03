using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IArtistService
    {
        long CreateArtist(NewArtistDto? artistDto); 
        long RenameArtist(long artistId, string newName);
        long RemoveArtist(long artistId);
        long AddCategory(long artistId, string categoryName);
        long RemoveCategory(long artistId, string categoryName);
        long AddTag(long artistId, string tagName);
        long RemoveTag(long artistId, string tagName);

        BasicDetails? GetArtistDetails(long artistId);

        /// <summary>
        /// 用名稱查詢作者清單
        /// </summary>
        /// <param name="artistName">名稱</param>
        /// <returns>作者清單</returns>
        IEnumerable<BasicDetails>? GetArtistDetails(string artistName);

        /// <summary>
        /// 取得作者標籤清單
        /// </summary>
        /// <param name="artistId">作者ID</param>
        /// <returns>標籤清單</returns>
        IEnumerable<BasicInfo> GetArtistTags(long artistId);

        /// <summary>
        /// 取得作者分類清單
        /// </summary>
        /// <param name="artistId">作者ID</param>
        /// <returns>分類清單</returns>
        IEnumerable<BasicInfo> GetArtistCategories(long artistId);
    }
}
