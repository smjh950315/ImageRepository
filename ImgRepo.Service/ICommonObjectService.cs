using Cyh.Net.Data.Predicate;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using ImgRepo.Model.Query;

namespace ImgRepo.Service
{
    public interface ICommonObjectService
    {
        long SetAttribute<TAttribute>(long objectId, string attrValue, bool _delete) where TAttribute : class, IBasicEntityAttribute, new();

        IEnumerable<BasicInfo> GetAttributes<TAttribute>(long objectId) where TAttribute : class, IBasicEntityAttribute, new();

        IQueryable<long> GetIdsByAttributeName<TAttribute>(IEnumerable<ExpressionData> exprDatas) where TAttribute : class, IBasicEntityAttribute, new();

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

        IQueryable<long> GetQueryableIdsByQueryModel(QueryModel? queryModel);
    }
}
