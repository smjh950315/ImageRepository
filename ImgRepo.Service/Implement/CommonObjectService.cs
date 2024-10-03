using Cyh.Net.Data;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.Enums;
using ImgRepo.Model.Interface;

namespace ImgRepo.Service.Implement
{
    internal class CommonObjectService<TObject, TRecord> : CommonService, ICommonObjectService
        where TObject : class, IBasicEntityInformation, new()
        where TRecord : class, IBasicEntityRecord, new()
    {
        public CommonObjectService(IDataSource dataSource) : base(dataSource) { }

        public virtual long RemoveObject(long objectId)
        {
            return this.removeObject<TObject>(objectId);
        }

        public long AddWebsite(long objectId, string webSite)
        {
            return this.setObjectAttr<TObject, TRecord, CategoryInformation>(objectId, AttributeType.Website, webSite, false);
        }

        public long RemoveWebsite(long objectId, string webSite)
        {
            return this.setObjectAttr<TObject, TRecord, CategoryInformation>(objectId, AttributeType.Website, webSite, true);
        }

        public long AddCategory(long objectId, string categoryName)
        {
            return this.setObjectAttr<TObject, TRecord, CategoryInformation>(objectId, AttributeType.Category, categoryName, false);
        }

        public long RemoveCategory(long objectId, string categoryName)
        {
            return this.setObjectAttr<TObject, TRecord, CategoryInformation>(objectId, AttributeType.Category, categoryName, true);
        }

        public long AddTag(long objectId, string tagName)
        {
            return this.setObjectAttr<TObject, TRecord, TagInformation>(objectId, AttributeType.Tag, tagName, false);
        }

        public long RemoveTag(long objectId, string tagName)
        {
            return this.setObjectAttr<TObject, TRecord, TagInformation>(objectId, AttributeType.Tag, tagName, true);
        }

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

        public IEnumerable<BasicInfo> GetTags(long objectId)
        {
            if (objectId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<TRecord, TagInformation>(objectId, AttributeType.Tag);
        }

        public IEnumerable<BasicInfo> GetCategories(long objectId)
        {
            if (objectId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<TRecord, CategoryInformation>(objectId, AttributeType.Category);
        }

        public IEnumerable<BasicInfo> GetWebsites(long objectId)
        {
            if (objectId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<TRecord, WebsiteInformation>(objectId, AttributeType.Website);
        }
    }
}
