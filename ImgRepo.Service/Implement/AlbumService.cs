using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.Entities.Album;
using ImgRepo.Model.Entities.Bindings;
using ImgRepo.Model.Entities.Image;
using ImgRepo.Service.CommonService;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class AlbumService : CommonObjectService<AlbumInformation, AlbumRecord>, IAlbumService
    {
        class MetaAlbumImageRecord
        {
            public long AlbumId { get; set; }
        }
        class AlbumImageBindingService : CommonObjectBindingService<AlbumInformation, ImageInformation, AlbumImageBindingRecord>
        {
            public AlbumImageBindingService(IDataSource dataSource) : base(dataSource)
            {
            }
        }
        readonly IQueryable<AlbumInformation> m_albums;
        readonly IDataWriter<AlbumInformation> m_albumWriter;
        readonly IQueryable<ImageInformation> m_images;
        readonly IDataWriter<ImageInformation> m_imageWriter;
        readonly IQueryable<AlbumImageBindingRecord> m_albumImageBindings;
        readonly CommonObjectBindingServiceBase m_albumImageBindingService;
        public AlbumService(IDataSource dataSource) : base(dataSource)
        {
            this.m_albums = this.m_dataSource.GetQueryable<AlbumInformation>();
            this.m_albumWriter = this.m_dataSource.GetWriter<AlbumInformation>();
            this.m_images = this.m_dataSource.GetQueryable<ImageInformation>();
            this.m_imageWriter = this.m_dataSource.GetWriter<ImageInformation>();
            this.m_albumImageBindings = this.m_dataSource.GetQueryable<AlbumImageBindingRecord>();
            this.m_albumImageBindingService = new AlbumImageBindingService(this.m_dataSource);
        }

        public long CreateAlbum(NewAlbumDto? albumDto)
        {
            if (albumDto == null) return 0;
            using (this.m_dataSource.BeginTransaction())
            {
                AlbumInformation album = new AlbumInformation
                {
                    Name = albumDto.Name,
                    Description = albumDto.Description,
                    Created = DateTime.Now,
                };
                this.m_albumWriter.Add(album);
                if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
                if (albumDto.Tags != null)
                {
                    foreach (string tag in albumDto.Tags)
                    {
                        this.AddTag(album.Id, tag);
                    }
                }
                if (albumDto.Categories != null)
                {
                    foreach (string category in albumDto.Categories)
                    {
                        this.AddCategory(album.Id, category);
                    }
                }
                this.m_dataSource.CommitTransaction();
                return album != null ? album.Id : 0;
            }
        }

        public long AddImage(long albumId, long imageId) => this.m_albumImageBindingService.SetBinding(imageId, albumId, false);

        public long RemoveImage(long albumId, long imageId) => this.m_albumImageBindingService.SetBinding(imageId, albumId, true);
    }
}
