using Cyh.Net.Data;
using Cyh.Net.Data.Predicate;
using ImgRepo.Model.Common;
using ImgRepo.Model.Query;

namespace ImgRepo.Service
{
    public interface ICommonObjectService
    {
        /// <summary>
        /// 移除物件資料檔
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>移除的物件ID，如果失敗回傳0，發生例外回傳-1</returns>
        long RemoveObject(long objectId);

        /// <summary>
        /// 加入物件網站
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="webSite">網站網址</param>
        /// <returns>新增的網站ID，如果失敗回傳0，發生例外回傳-1</returns>
        long AddWebsite(long objectId, string webSite);

        /// <summary>
        /// 移除物件網站
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="webSite">網站網址</param>
        /// <returns>移除的網站ID，如果失敗回傳0，發生例外回傳-1</returns>
        long RemoveWebsite(long objectId, string webSite);

        /// <summary>
        /// 為物件新增分類
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="categoryName">分類文字</param>
        /// <returns>新增的分類ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        long AddCategory(long objectId, string categoryName);

        /// <summary>
        /// 移除物件的分類
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="categoryName">分類文字</param>
        /// <returns>移除的分類ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        long RemoveCategory(long objectId, string categoryName);

        /// <summary>
        /// 為物件新增標籤
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="tagName">標籤文字</param>
        /// <returns>新增的標籤ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        long AddTag(long objectId, string tagName);

        /// <summary>
        /// 移除物件的標籤
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="tagName">標籤文字</param>
        /// <returns>移除的標籤ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        long RemoveTag(long objectId, string tagName);

        /// <summary>
        /// 取得物件基本資訊
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>對應ID的物件基本資訊，如果物件ID無效回傳null，發生例外回傳-1</returns>
        BasicDetails? GetBasicDetails(long objectId);

        /// <summary>
        /// 取得物件標籤清單
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>標籤清單</returns>
        IEnumerable<BasicInfo> GetTags(long objectId);

        /// <summary>
        /// 取得物件分類清單
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>分類清單</returns>
        IEnumerable<BasicInfo> GetCategories(long objectId);

        /// <summary>
        /// 取得物件網站清單
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>網站清單</returns>
        IEnumerable<BasicInfo> GetWebsites(long objectId);

        /// <summary>
        /// 用標籤取物件ID
        /// </summary>
        /// <param name="exprDatas"></param>
        /// <returns></returns>
        public IEnumerable<long> GetIdsByTagName(IEnumerable<ExpressionData> exprDatas, DataRange? range = null);

        /// <summary>
        /// 用分類取物件ID
        /// </summary>
        /// <param name="exprDatas"></param>
        /// <returns></returns>
        public IEnumerable<long> GetIdsByCategoryName(IEnumerable<ExpressionData> exprDatas, DataRange? range = null);

        /// <summary>
        /// 用網站取物件ID
        /// </summary>
        /// <param name="exprDatas"></param>
        /// <returns></returns>
        public IEnumerable<long> GetIdsByWebsiteName(IEnumerable<ExpressionData> exprDatas, DataRange? range = null);

        IEnumerable<long> GetIdsByQueryModel(QueryModel? queryModel, DataRange? range = null);
    }
}
