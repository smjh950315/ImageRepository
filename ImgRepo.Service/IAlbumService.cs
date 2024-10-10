using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IAlbumService
    {
        /// <summary>
        /// 新增圖片到相簿
        /// </summary>
        /// <param name="albumId">相簿ID</param>
        /// <param name="imageId">圖片ID</param>
        /// <returns>圖片ID，或是0代表失敗</returns>
        long AddImage(long albumId, long imageId);

        /// <summary>
        /// 移除相簿中的圖片
        /// </summary>
        /// <param name="albumId">相簿ID</param>
        /// <param name="imageId">圖片ID</param>
        /// <returns>圖片ID，或是0代表失敗</returns>
        long RemoveImage(long albumId, long imageId);

        /// <summary>
        /// 建立相簿
        /// </summary>
        /// <param name="albumDto">相簿資料傳輸物件</param>
        /// <returns>新增的相簿ID或是0表示失敗</returns>
        long CreateAlbum(NewAlbumDto? albumDto);
    }
}
