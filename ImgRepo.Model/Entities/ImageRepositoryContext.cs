using Cyh.Net.Data;
using ImgRepo.Model.Entities.Album;
using ImgRepo.Model.Entities.Artist;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.Entities.Bindings;
using ImgRepo.Model.Entities.Image;
using Microsoft.EntityFrameworkCore;

namespace ImgRepo.Model
{
    public partial class ImageRepositoryContext : DbContext, IDataSource
    {
        public ImageRepositoryContext(DbContextOptions<ImageRepositoryContext> options) : base(options) { }


        public DbSet<AlbumInformation> AlbumInformations { get; set; }
        public DbSet<AlbumRecord> AlbumRecords { get; set; }


        public DbSet<ArtistInformation> ArtistInformations { get; set; }
        public DbSet<ArtistRecord> ArtistRecords { get; set; }


        public DbSet<ImageInformation> ImageInformations { get; set; }
        public DbSet<ImageRecord> ImageRecords { get; set; }
        public DbSet<ImageFileData> ImageFileDatas { get; set; }


        public DbSet<TagInformation> TagInformations { get; set; }
        public DbSet<CategoryInformation> CategoryInformations { get; set; }
        public DbSet<WebsiteInformation> WebsiteInformations { get; set; }


        public DbSet<AlbumImageBindingRecord> AlbumImageBindings { get; set; }
    }

    public partial class ImageRepositoryContext
    {
        readonly Dictionary<Type, object> m_queryables = new Dictionary<Type, object>();
        readonly Dictionary<Type, object> m_writers = new Dictionary<Type, object>();

        public IDisposable? BeginTransaction()
        {
            return this.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.Database.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            this.Database.RollbackTransaction();
        }

        public IQueryable<T> GetQueryable<T>() where T : class
        {
            Type entityType = typeof(T);
            if (this.m_queryables.TryGetValue(entityType, out object? queryable))
            {
                return (IQueryable<T>)queryable;
            }
            else
            {
                DbSet<T> iqueryable = this.Set<T>();
                this.m_queryables.Add(entityType, iqueryable);
                return iqueryable;
            }
        }

        public IDataWriter<T> GetWriter<T>() where T : class
        {
            Type entityType = typeof(T);
            if (this.m_writers.TryGetValue(entityType, out object? queryable))
            {
                return (IDataWriter<T>)queryable;
            }
            else
            {
                DbSet<T>? dbSet = this.GetQueryable<T>() as DbSet<T>;
                ImageRepositoryDataWriter<T> idataWriter = new ImageRepositoryDataWriter<T>(dbSet!);
                this.m_writers.Add(entityType, idataWriter);
                return idataWriter;
            }
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

        public void AddRange(IEnumerable<T> items)
        {
            this.m_set.AddRange(items);
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
