using Cyh.Net.Data;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using Microsoft.EntityFrameworkCore;

namespace ImgRepo.Service.Implement
{
    internal class CommonAttributeService : ICommonAttributeService
    {
        readonly IDataSource m_dataSource;

        public CommonAttributeService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
        }

        public virtual IQueryable<BasicDetails> GetDetailsQueryable<TAttribute>() where TAttribute : class, IBasicEntityAttribute, new()
        {
            return this.m_dataSource
                .GetQueryable<TAttribute>()
                .Select(x => new BasicDetails
                {
                    Id = x.Id,
                    Name = x.Value,
                    Description = x.Description
                });
        }

        public BasicDetails? GetDetailById<TAttribute>(long id) where TAttribute : class, IBasicEntityAttribute, new()
        {
            var queryable = this.m_dataSource.GetQueryable<TAttribute>();
            if (queryable is DbSet<TAttribute> set)
            {
                var entity = set.Find(id);
                if (entity is null) return null;
                return new BasicDetails
                {
                    Id = entity.Id,
                    Name = entity.Value,
                    Description = entity.Description
                };
            }
            else
            {
                return queryable.Select(x => new BasicDetails
                {
                    Id = x.Id,
                    Name = x.Value,
                    Description = x.Description
                }).FirstOrDefault();
            }
        }
    }
}
