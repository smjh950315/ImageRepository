using ImgRepo.Model.Common;

namespace ImgRepo.Service
{
    public interface ICommonAttributeService
    {
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
    }
}
