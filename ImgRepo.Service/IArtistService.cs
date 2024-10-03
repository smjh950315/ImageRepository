using ImgRepo.Model.Common;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IArtistService
    {
        /// <summary>
        /// 新增藝術家資料檔
        /// </summary>
        /// <param name="artistDto">藝術家新增模型</param>
        /// <returns>新的藝術家ID，如果失敗回傳0，發生例外回傳-1</returns>
        long CreateArtist(NewArtistDto? artistDto);

        /// <summary>
        /// 修改藝術家資料檔
        /// </summary>
        /// <param name="artistId">藝術家ID</param>
        /// <param name="newName">新名稱</param>
        /// <returns>修改的藝術家ID，如果失敗回傳0，發生例外回傳-1</returns>
        long RenameArtist(long artistId, string newName);

        /// <summary>
        /// 移除藝術家資料檔
        /// </summary>
        /// <param name="artistId">藝術家ID</param>
        /// <returns>移除的藝術家ID，如果失敗回傳0，發生例外回傳-1</returns>
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
