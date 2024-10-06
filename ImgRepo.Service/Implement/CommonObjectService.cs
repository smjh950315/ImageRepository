using Cyh.Net.Data;
using Cyh.Net.Data.Predicate;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.Query;

namespace ImgRepo.Service.Implement
{
    internal class CommonObjectService<TObject, TRecord> : CommonObjectServiceBase, ICommonObjectService
        where TObject : class, IBasicEntityInformation, new()
        where TRecord : class, IBasicEntityRecord, new()
    {
        protected override Type ObjectType => typeof(TObject);
        protected override Type RecordType => typeof(TRecord);

        public CommonObjectService(IDataSource dataSource) : base(dataSource)
        {
        }

        public virtual long RemoveObject(long objectId) => this.Remove(objectId);

        public long AddWebsite(long objectId, string webSite) => this.AddAttribute<WebsiteInformation>(objectId, webSite);

        public long RemoveWebsite(long objectId, string webSite) => this.RemoveAttribute<WebsiteInformation>(objectId, webSite);

        public long AddCategory(long objectId, string categoryName) => this.AddAttribute<CategoryInformation>(objectId, categoryName);

        public long RemoveCategory(long objectId, string categoryName) => this.RemoveAttribute<CategoryInformation>(objectId, categoryName);

        public long AddTag(long objectId, string tagName) => this.AddAttribute<TagInformation>(objectId, tagName);

        public long RemoveTag(long objectId, string tagName) => this.RemoveAttribute<TagInformation>(objectId, tagName);

        public BasicDetails? GetBasicDetails(long objectId)
        {
            if (objectId == 0) return null;
            TObject? @object = this.m_dataSource.GetQueryable<TObject>().FirstOrDefault(i => i.Id == objectId);
            if (@object == null) return null;
            return new BasicDetails
            {
                Id = @object.Id,
                Name = @object.Name,
                Description = @object.Description,
            };
        }

        public IEnumerable<BasicInfo> GetTags(long objectId) => this.GetAttributes<TagInformation>(objectId);

        public IEnumerable<BasicInfo> GetCategories(long objectId) => this.GetAttributes<CategoryInformation>(objectId);

        public IEnumerable<BasicInfo> GetWebsites(long objectId) => this.GetAttributes<WebsiteInformation>(objectId);

        public IEnumerable<long> GetIdsByTagName(IEnumerable<ExpressionData> exprDatas, DataRange? range = null)
            => this.GetIdsByAttributeName<TagInformation>(exprDatas, range);

        public IEnumerable<long> GetIdsByCategoryName(IEnumerable<ExpressionData> exprDatas, DataRange? range = null)
            => this.GetIdsByAttributeName<CategoryInformation>(exprDatas, range);

        public IEnumerable<long> GetIdsByWebsiteName(IEnumerable<ExpressionData> exprDatas, DataRange? range = null)
            => this.GetIdsByAttributeName<WebsiteInformation>(exprDatas, range);

        public IEnumerable<long> GetIdsByQueryModel(QueryModel? queryModel, DataRange? range = null)
        {
            if (queryModel == null) return Enumerable.Empty<long>();

            IQueryable<long> ids = this.queryAllAttributes<TObject, TRecord>(queryModel).Distinct();
            if (range != null)
            {
                return ids.Skip(range.Begin).Take(range.Count).ToList();
            }
            return ids.ToList();
        }
    }
}
