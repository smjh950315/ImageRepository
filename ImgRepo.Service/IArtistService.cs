using ImgRepo.Model.Common;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IArtistService : ICommonObjectService
    {
        /// <summary>
        /// 新增藝術家資料檔
        /// </summary>
        /// <param name="artistDto">藝術家新增模型</param>
        /// <returns>新的藝術家ID，如果失敗回傳0，發生例外回傳-1</returns>
        long CreateArtist(NewArtistDto? artistDto);

        /// <summary>
        /// 修改藝術家名稱
        /// </summary>
        /// <param name="artistId">藝術家ID</param>
        /// <param name="newName">新名稱</param>
        /// <returns>修改的藝術家ID，如果失敗回傳0，發生例外回傳-1</returns>
        long RenameArtist(long artistId, string newName);

        /// <summary>
        /// 用名稱查詢作者清單
        /// </summary>
        /// <param name="artistName">名稱</param>
        /// <returns>作者清單</returns>
        IEnumerable<BasicDetails>? GetArtistDetails(string artistName);
    }
}
