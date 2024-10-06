using Cyh.Net.Data;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Attributes;

namespace ImgRepo.Service.Implement
{
    internal class CommonAttributeService : CommonService, ICommonAttributeService
    {
        IQueryable<TagInformation> m_tags;
        IDataWriter<TagInformation> m_tagWriter;
        IQueryable<CategoryInformation> m_categories;
        IDataWriter<CategoryInformation> m_categoryWriter;

        public CommonAttributeService(IDataSource dataSource) : base(dataSource)
        {
            this.m_tags = dataSource.GetQueryable<TagInformation>();
            this.m_tagWriter = dataSource.GetWriter<TagInformation>();
            this.m_categories = dataSource.GetQueryable<CategoryInformation>();
            this.m_categoryWriter = dataSource.GetWriter<CategoryInformation>();
        }

        public BasicDetails? GetTagDetail(long id)
        {
            return this.m_tags.Select(x => new BasicDetails
            {
                Id = x.Id,
                Name = x.Value,
                Description = x.Description
            }).FirstOrDefault(t => t.Id == id);
        }

        public BasicDetails? GetCategoryDetail(long id)
        {
            return this.m_categories.Select(x => new BasicDetails
            {
                Id = x.Id,
                Name = x.Value,
                Description = x.Description
            }).FirstOrDefault(t => t.Id == id);
        }
    }
}
