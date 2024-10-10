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
        /// 比對兩張圖片差異度
        /// </summary>
        /// <param name="lhsId">圖片ID</param>
        /// <param name="rhsId">圖片ID</param>
        /// <returns>差異度，如果無法使用回傳小於0的值</returns>
        double GetImageDifferential(long lhsId, long rhsId);

        /// <summary>
        /// 建立新的圖片
        /// </summary>
        /// <returns>新圖片的ID，如果失敗傳回0，發生例外回傳-1</returns>
        Task<long> CreateImageAsync(NewImageDto? imageDto);

        /// <summary>
        /// 建立新的圖片
        /// </summary>
        /// <param name="is_same_batch">是否為同一批次</param>
        /// <returns>新圖片的ID，如果失敗傳回0，發生例外回傳-1</returns>
        Task<long> BatchCreateImageAsync(IEnumerable<NewImageDto> imageDtos, bool is_same_batch);

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
