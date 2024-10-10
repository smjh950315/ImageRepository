using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;

namespace ImgRepo.Service
{
    public interface ICommonAttributeService
    {
        /// <summary>
        /// 取得屬性資訊的IQeuryable類型
        /// </summary>
        /// <typeparam name="TAttribute">屬性類型</typeparam>
        /// <returns>屬性資訊的IQeuryable類型</returns>
        IQueryable<BasicDetails> GetDetailsQueryable<TAttribute>() where TAttribute : class, IBasicEntityAttribute, new();

        /// <summary>
        /// 用屬性ID取得屬性資訊
        /// </summary>
        /// <typeparam name="TAttribute">屬性類型</typeparam>
        /// <param name="id">屬性ID</param>
        /// <returns>屬性資訊，或是null代表屬性不存在</returns>
        BasicDetails? GetDetailById<TAttribute>(long id) where TAttribute : class, IBasicEntityAttribute, new();
    }
}
