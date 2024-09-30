using Cyh.Net.Data;
using ImgRepo.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImgRepo.Model
{
    public partial class ImageRepositoryContext : DbContext, IDataSource
    {
        public ImageRepositoryContext(DbContextOptions<ImageRepositoryContext> options) : base(options) { }

        public DbSet<ImageInformation> ImageInformations { get; set; }
        public DbSet<AlbumInformation> AlbumInformations { get; set; }
        public DbSet<TagInformation> TagInformations { get; set; }
        public DbSet<CategoryInformation> CategoryInformations { get; set; }
        public DbSet<BindingRecord> BindingRecords { get; set; }
        public DbSet<ImageFileData> ImageFileDatas { get; set; }
    }
    public partial class ImageRepositoryContext
    {
        readonly Dictionary<Type, object> m_queryables = new Dictionary<Type, object>();

        public IQueryable<T> GetQueryable<T>() where T : class
        {
            Type entityType = typeof(T);
            if (this.m_queryables.TryGetValue(entityType, out var queryable))
            {
                return (IQueryable<T>)queryable;
            }
            else
            {
                var iqueryable = this.Set<T>();
                this.m_queryables.Add(entityType, iqueryable);
                return iqueryable;
            }
        }

        public IDataWriter<T> GetWriter<T>() where T : class
        {
            return new ImageRepositoryDataWriter<T>(this.Set<T>());
        }

        public bool Save()
        {
            return this.SaveChanges() != 0;
        }
    }
    public class ImageRepositoryDataWriter<T> : IDataWriter<T> where T : class
    {
        readonly DbSet<T> m_set;
        public ImageRepositoryDataWriter(DbSet<T> set)
        {
            this.m_set = set;
        }
        public void Add(T item)
        {
            this.m_set.Add(item);
        }

        public void Remove(T item)
        {
            this.m_set.Remove(item);
        }

        public void Update(T item)
        {
            this.m_set.Update(item);
        }
    }
}
