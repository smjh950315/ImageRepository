using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.Query;
using System.Reflection;

namespace ImgRepo.Service
{
    public static class Extensions
    {
        /// <summary>
        /// 儲存物件
        /// </summary>
        /// <param name="objectType">物件類型名稱</param>
        /// <param name="uriName">物件uri名稱</param>
        /// <param name="data">位元資料</param>
        /// <returns></returns>
        public static bool SaveObject(this IFileAccessService fileAccessService, string objectType, string uriName, byte[] data)
        {
            string uriPath = $"{objectType}/{uriName}";
            return fileAccessService.SaveFile(uriPath, data);
        }

        /// <summary>
        /// 從流讀出位元資料
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 不會發生例外的字串分割
        /// </summary>
        /// <param name="separator">分割字元</param>
        /// <returns>分割結果</returns>
        public static string[] SplitNoThrow(this string str, char separator)
            => str.IsNullOrEmpty() ? Array.Empty<string>() : str.Split(separator);

        /// <summary>
        /// 不會發生例外的字串分割
        /// </summary>
        /// <param name="separator">分割字串</param>
        /// <returns>分割結果</returns>
        public static string[] SplitNoThrow(this string str, string separator)
            => str.IsNullOrEmpty() ? Array.Empty<string>() : str.Split(separator);

        /// <summary>
        /// 從字典用名稱尋找方法並執行
        /// </summary>
        /// <param name="methodDictionary">字典</param>
        /// <param name="instance">目標方法需要的物件實體或是null表示靜態方法</param>
        /// <param name="methodName">方法名稱</param>
        /// <param name="genericParameters">泛型參數</param>
        /// <param name="arguments">方法參數</param>
        /// <returns>方法回傳值</returns>
        public static object? FindAndExecuteByName(this Dictionary<string, MethodInfo> methodDictionary, object? instance, string methodName, Type[] genericParameters, params object?[] arguments)
        {
            if (methodDictionary.TryGetValue(methodName, out MethodInfo? methodInfo))
            {
                MethodInfo? method
                    = methodInfo.IsGenericMethod && genericParameters.Length != 0
                    ? methodInfo.MakeGenericMethod(genericParameters)
                    : methodInfo;
                return method.Invoke(instance, arguments);
            }
            return null;
        }

        /// <summary>
        /// 為物件添加屬性
        /// </summary>
        /// <typeparam name="TAttribute">屬性DB類型</typeparam>
        /// <param name="objectId">物件ID</param>
        /// <param name="attrValue">屬性名稱</param>
        /// <returns>加入的屬性ID或是0代表失敗</returns>
        public static long AddAttribute<TAttribute>(this ICommonObjectService commonObjectService, long objectId, string attrValue) where TAttribute : class, IBasicEntityAttribute, new()
            => commonObjectService.SetAttribute<TAttribute>(objectId, attrValue, false);

        /// <summary>
        /// 移除物件屬性
        /// </summary>
        /// <typeparam name="TAttribute">屬性DB類型</typeparam>
        /// <param name="objectId">物件ID</param>
        /// <param name="attrValue">屬性名稱</param>
        /// <returns>移除的屬性ID或是0代表失敗</returns>
        public static long RemoveAttribute<TAttribute>(this ICommonObjectService commonObjectService, long objectId, string attrValue) where TAttribute : class, IBasicEntityAttribute, new()
            => commonObjectService.SetAttribute<TAttribute>(objectId, attrValue, true);

        /// <summary>
        /// 用查詢模型來取得物件ID
        /// </summary>
        /// <param name="queryModel">查詢模型</param>
        /// <param name="range">要查詢的範圍</param>
        /// <returns>物件ID</returns>
        public static IEnumerable<long> GetIdsByQueryModel(this ICommonObjectService commonObjectService, QueryModel? queryModel, DataRange? range = null)
        {
            var distinctResult = commonObjectService.GetQueryableIdsByQueryModel(queryModel).Distinct();
            return (range != null
                ? distinctResult.Skip(range.Begin).Take(range.Count)
                : distinctResult).ToList();
        }

        /// <summary>
        /// 加入物件網站
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="webSite">網站網址</param>
        /// <returns>新增的網站ID，如果失敗回傳0，發生例外回傳-1</returns>
        public static long AddWebsite(this ICommonObjectService commonObjectService, long objectId, string webSite)
            => commonObjectService.AddAttribute<WebsiteInformation>(objectId, webSite);

        /// <summary>
        /// 移除物件網站
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="webSite">網站網址</param>
        /// <returns>移除的網站ID，如果失敗回傳0，發生例外回傳-1</returns>
        public static long RemoveWebsite(this ICommonObjectService commonObjectService, long objectId, string webSite)
            => commonObjectService.RemoveAttribute<WebsiteInformation>(objectId, webSite);

        /// <summary>
        /// 為物件新增分類
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="categoryName">分類文字</param>
        /// <returns>新增的分類ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        public static long AddCategory(this ICommonObjectService commonObjectService, long objectId, string categoryName)
            => commonObjectService.AddAttribute<CategoryInformation>(objectId, categoryName);

        /// <summary>
        /// 移除物件的分類
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="categoryName">分類文字</param>
        /// <returns>移除的分類ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        public static long RemoveCategory(this ICommonObjectService commonObjectService, long objectId, string categoryName)
            => commonObjectService.RemoveAttribute<CategoryInformation>(objectId, categoryName);

        /// <summary>
        /// 為物件新增標籤
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="tagName">標籤文字</param>
        /// <returns>新增的標籤ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        public static long AddTag(this ICommonObjectService commonObjectService, long objectId, string tagName)
            => commonObjectService.AddAttribute<TagInformation>(objectId, tagName);

        /// <summary>
        /// 移除物件的標籤
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <param name="tagName">標籤文字</param>
        /// <returns>移除的標籤ID，如果物件ID無效回傳0，發生例外回傳-1</returns>
        public static long RemoveTag(this ICommonObjectService commonObjectService, long objectId, string tagName)
            => commonObjectService.RemoveAttribute<TagInformation>(objectId, tagName);

        /// <summary>
        /// 取得物件標籤清單
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>標籤清單</returns>
        public static IEnumerable<BasicInfo> GetTags(this ICommonObjectService commonObjectService, long objectId)
            => commonObjectService.GetAttributeQueryable<TagInformation>(objectId);

        /// <summary>
        /// 取得物件分類清單
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>分類清單</returns>
        public static IEnumerable<BasicInfo> GetCategories(this ICommonObjectService commonObjectService, long objectId)
            => commonObjectService.GetAttributeQueryable<CategoryInformation>(objectId);

        /// <summary>
        /// 取得物件網站清單
        /// </summary>
        /// <param name="objectId">物件ID</param>
        /// <returns>網站清單</returns>
        public static IEnumerable<BasicInfo> GetWebsites(this ICommonObjectService commonObjectService, long objectId)
            => commonObjectService.GetAttributeQueryable<WebsiteInformation>(objectId);


        /// <summary>
        /// 取得標籤的基本資訊
        /// </summary>
        /// <param name="id">標籤ID</param>
        /// <returns>如果沒有對應ID的標籤就傳回null</returns>
        public static BasicDetails? GetTagDetail(this ICommonAttributeService commonAttributeService, long id)
            => commonAttributeService.GetDetailById<TagInformation>(id);

        /// <summary>
        /// 取得分類的基本資訊
        /// </summary>
        /// <param name="id">分類ID</param>
        /// <returns>如果沒有對應ID的分類就傳回null</returns>
        public static BasicDetails? GetCategoryDetail(this ICommonAttributeService commonAttributeService, long id)
            => commonAttributeService.GetDetailById<CategoryInformation>(id);

    }
}
