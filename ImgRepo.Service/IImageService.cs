using Cyh.Net.Data;
using ImgRepo.Model.Common;
using ImgRepo.Model.Image;
using ImgRepo.Model.Query;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IImageService : ICommonObjectService
    {
        /// <summary>
        /// 建立新的圖片
        /// </summary>
        /// <returns>新圖片的ID，如果失敗傳回0，發生例外回傳-1</returns>
        long CreateImage(NewImageDto? imageDto);

        /// <summary>
        /// 重新命名圖片
        /// </summary>
        /// <param name="imageId">圖片ID</param>
        /// <param name="newName">新名稱</param>
        /// <returns>圖片ID</returns>
        long RenameImage(long imageId, string newName);

        /// <summary>
        /// 用藝術家資料檔ID設定作者
        /// <para>已經有作者則會覆蓋</para>
        /// <para>authorDataId為0則會新增作者資料檔</para>
        /// <para>authorDataId為-1則會清除作者</para>
        /// </summary>
        /// <returns>如果新增或是覆蓋，傳回藝術家資料檔ID; 如果清除，傳回-1</returns>
        long SetAuthor(long imageId, long authorDataId, bool _delete);

        /// <summary>
        /// 取得圖片原始檔案
        /// </summary>
        /// <param name="imgId">圖片ID</param>
        /// <returns>Api用的檔案模型，如果沒有對應圖片就傳回null</returns>
        ApiFileModel? GetFullImage(long imgId);

        /// <summary>
        /// 取得圖片縮圖檔案
        /// </summary>
        /// <param name="imgId">圖片ID</param>
        /// <returns>Api用的檔案模型，如果沒有對應圖片就傳回null</returns>
        ApiFileModel? GetThumbnail(long imgId);

        /// <summary>
        /// 取得圖片縮圖檔案
        /// </summary>
        /// <returns>Api用的檔案模型</returns>
        IEnumerable<ApiThumbFileModel> GetThumbnails(QueryModel? queryModel, DataRange? dataRange = null);
    }
}
