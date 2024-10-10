using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using ImgRepo.Model.Query;

namespace ImgRepo.Service
{
    public interface ICommonObjectService
    {
        /// <summary>
        /// 設定物件屬性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="objectId">物件ID</param>
        /// <param name="attrValue">要設定的屬性名稱</param>
        /// <param name="_delete">是否刪除</param>
        /// <returns>設定的屬性ID 或是0表示失敗</returns>
        long SetAttribute<TAttribute>(long objectId, string attrValue, bool _delete) where TAttribute : class, IBasicEntityAttribute, new();

        /// <summary>
        /// 取得物件屬性的IQeuryable類型
        /// </summary>
        /// <typeparam name="TAttribute">物件屬性</typeparam>
        /// <param name="objectId">物件資訊的IQ</param>
        /// <returns>IQeuryable</returns>
        IQueryable<BasicInfo> GetAttributeQueryable<TAttribute>(long objectId) where TAttribute : class, IBasicEntityAttribute, new();

        /// <summary>
        /// 修改物件名稱
        /// </summary>
        /// <param name="artistId">物件ID</param>
        /// <param name="newName">新名稱</param>
        /// <returns>修改的物件ID，如果失敗回傳0，發生例外回傳-1</returns>
        long Rename(long objectId, string newName);

        /// <summary>
        /// 移除物件資料檔
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>移除的物件ID，如果失敗回傳0，發生例外回傳-1</returns>
        long Remove(long objectId);

        /// <summary>
        /// 取得物件基本資訊
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>對應ID的物件基本資訊，如果物件ID無效回傳null，發生例外回傳-1</returns>
        BasicDetails? GetBasicDetails(long objectId);

        /// <summary>
        /// 用查詢Model取得物件ID的IQeuryable類型
        /// </summary>
        /// <param name="queryModel">查詢Model</param>
        /// <returns>物件ID的IQeuryable類型</returns>
        IQueryable<long> GetQueryableIdsByQueryModel(QueryModel? queryModel);
    }
}
