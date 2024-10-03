using ImgRepo.Model.Common;
using ImgRepo.Model.Image;
using ImgRepo.Model.Query;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IImageService
    {
        /// <summary>
        /// 建立新的圖片
        /// </summary>
        /// <returns>新圖片的ID，如果失敗傳回0，發生例外回傳-1</returns>
        long CreateImage(NewImageDto? imageDto);

        /// <summary>
        /// 移除圖片
        /// </summary>
        /// <param name="imageId">圖片ID</param>
        /// <returns>移除的圖片ID，0代表失敗、-1代表發生例外</returns>
        long RemoveImage(long imageId);

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
        /// 為圖片新增分類
        /// </summary>
        /// <param name="imageId">分類ID</param>
        /// <param name="categoryName">分類文字</param>
        /// <returns>新增的分類ID，如果圖片ID無效回傳0，發生例外回傳-1</returns>
        long AddCategory(long imageId, string categoryName);

        /// <summary>
        /// 移除圖片的分類
        /// </summary>
        /// <param name="imageId">圖片ID</param>
        /// <param name="categoryName">分類文字</param>
        /// <returns>移除的分類ID，如果圖片ID無效回傳0，發生例外回傳-1</returns>
        long RemoveCategory(long imageId, string categoryName);

        /// <summary>
        /// 為圖片新增標籤
        /// </summary>
        /// <param name="imageId">圖片ID</param>
        /// <param name="tagName">標籤文字</param>
        /// <returns>新增的標籤ID，如果圖片ID無效回傳0，發生例外回傳-1</returns>
        long AddTag(long imageId, string tagName);

        /// <summary>
        /// 移除圖片的標籤
        /// </summary>
        /// <param name="imageId">圖片ID</param>
        /// <param name="tagName">標籤文字</param>
        /// <returns>移除的標籤ID，如果圖片ID無效回傳0，發生例外回傳-1</returns>
        long RemoveTag(long imageId, string tagName);



        /// <summary>
        /// 取得圖片的基本資訊
        /// </summary>
        /// <param name="id">圖片ID</param>
        /// <returns>如果沒有對應ID的圖片就傳回null</returns>
        BasicDetails? GetImageDetail(long id);

        /// <summary>
        /// 取得標籤的基本資訊
        /// </summary>
        /// <param name="id">標籤ID</param>
        /// <returns>如果沒有對應ID的標籤就傳回null</returns>
        BasicDetails? GetTagDetail(long id);

        /// <summary>
        /// 取得分類的基本資訊
        /// </summary>
        /// <param name="id">分類ID</param>
        /// <returns>如果沒有對應ID的分類就傳回null</returns>
        BasicDetails? GetCategoryDetail(long id);

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
        IEnumerable<ApiThumbFileModel> GetThumbnails(QueryModel? queryModel);

        /// <summary>
        /// 取得圖片標籤清單
        /// </summary>
        /// <param name="imgId">圖片ID</param>
        /// <returns>標籤清單</returns>
        IEnumerable<BasicInfo> GetImageTags(long imgId);

        /// <summary>
        /// 取得圖片分類清單
        /// </summary>
        /// <param name="imgId">圖片ID</param>
        /// <returns>分類清單</returns>
        IEnumerable<BasicInfo> GetImageCategories(long imgId);
    }
}
