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
    }
}
