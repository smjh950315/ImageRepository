using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using Microsoft.EntityFrameworkCore;

namespace ImgRepo.Service.Implement
{
    internal class CommonAttributeService : ICommonAttributeService
    {
        readonly IDataSource m_dataSource;

        static CommonAttributeService()
        {
        }

        public CommonAttributeService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
        }

        public long CreateNewUnchecked<TAttribute>(string value) where TAttribute : class, IBasicEntityAttribute, new()
        {
            TAttribute newAttr = new TAttribute
            {
                Value = value,
                Created = DateTime.Now,
            };
            this.m_dataSource.GetWriter<TAttribute>().Add(newAttr);
            this.m_dataSource.Save();
            return newAttr.Id;
        }

        public long GetIdByName<TAttribute>(string value) where TAttribute : class, IBasicEntityAttribute, new()
        {
            return value.IsNullOrEmpty() ? 0 : this.CreateNewUnchecked<TAttribute>(value);
        }

        public IEnumerable<long> GetIdsByNames<TAttribute>(IEnumerable<string> values) where TAttribute : class, IBasicEntityAttribute, new()
        {
            // no values
            if (values.IsNullOrEmpty()) return Enumerable.Empty<long>();

            List<long>? idResults = null;

            IEnumerable<string> requiredToAddValues;
            {
                IQueryable<BasicDetails> existingAttrs = this
                    .GetDetailsQueryable<TAttribute>()
                    .Where(x => values.Contains(x.Name));

                if (existingAttrs.Any())
                {
                    IQueryable<string> existingValues = existingAttrs.Select(x => x.Name!);
                    requiredToAddValues = values.Except(existingValues);
                    idResults = existingAttrs.Select(x => x.Id).ToList();
                }
                else
                {
                    requiredToAddValues = values;
                }
            }

            List<TAttribute> newAttrs = new List<TAttribute>();
            {
                foreach (string requiredToAddValue in requiredToAddValues)
                {
                    newAttrs.Add(new TAttribute
                    {
                        Value = requiredToAddValue,
                        Created = DateTime.Now,
                    });
                }
                this.m_dataSource.GetWriter<TAttribute>().AddRange(newAttrs);
                if (!Lib.TryExecute(() => this.m_dataSource.Save())) return Enumerable.Empty<long>();
            }

            IEnumerable<long> newIds = newAttrs.Select(x => x.Id);

            return idResults != null ? idResults.Concat(newIds) : newIds;
        }

        public IQueryable<BasicDetails> GetDetailsQueryable<TAttribute>() where TAttribute : class, IBasicEntityAttribute, new()
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
            IQueryable<TAttribute> queryable = this.m_dataSource.GetQueryable<TAttribute>();
            if (queryable is DbSet<TAttribute> set)
            {
                TAttribute? entity = set.Find(id);
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
