using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.Entities;
using ImgRepo.Model.Enums;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class ArtistService : CommonService, IArtistService
    {
        IQueryable<ArtistInformation> m_artists;
        IDataWriter<ArtistInformation> m_artistWriter;

        IQueryable<ArtistRecord> m_artistRecords;
        IDataWriter<ArtistRecord> m_artistRecordWriter;

        public ArtistService(IDataSource dataSource) : base(dataSource)
        {
            this.m_artists = dataSource.GetQueryable<ArtistInformation>();
            this.m_artistWriter = dataSource.GetWriter<ArtistInformation>();
            this.m_artistRecords = dataSource.GetQueryable<ArtistRecord>();
            this.m_artistRecordWriter = dataSource.GetWriter<ArtistRecord>();
        }

        public long CreateArtist(NewArtistDto? artistDto)
        {
            if (artistDto == null) return 0;
            var artist = new ArtistInformation
            {
                Name = artistDto.ArtistName,
                Description = artistDto.Description,
                Created = DateTime.Now,
            };
            this.m_dataSource.BeginTransaction();
            this.m_artistWriter.Add(artist);
            if (!Lib.TryExecute(() => this.m_dataSource.Save()))
            {
                this.m_dataSource.RollbackTransaction();
                return -1;
            }
            this.m_dataSource.CommitTransaction();
            var artistId = artist.Id;
            if (artistDto.Tags != null)
            {
                foreach (var tag in artistDto.Tags)
                {
                    this.AddTag(artistId, tag);
                }
            }
            if (artistDto.Categories != null)
            {
                foreach (var category in artistDto.Categories)
                {
                    this.AddCategory(artistId, category);
                }
            }
            return artistId;
        }

        public long RenameArtist(long artistId, string newName)
        {
            return this.renameObject<ArtistInformation>(artistId, newName);
        }

        public long RemoveArtist(long artistId)
        {
            return this.removeObject<ArtistInformation>(artistId);
        }

        public long AddCategory(long imageId, string categoryName)
        {
            return this.setObjectAttr<ArtistInformation, ArtistRecord, CategoryInformation>(imageId, AttributeType.Category, categoryName, false);
        }
        public long RemoveCategory(long imageId, string categoryName)
        {
            return this.setObjectAttr<ArtistInformation, ArtistRecord, CategoryInformation>(imageId, AttributeType.Category, categoryName, true);
        }

        public long AddTag(long artistId, string tagName)
        {
            return this.setObjectAttr<ArtistInformation, ArtistRecord, TagInformation>(artistId, AttributeType.Tag, tagName, false);
        }
        public long RemoveTag(long artistId, string tagName)
        {
            return this.setObjectAttr<ArtistInformation, ArtistRecord, TagInformation>(artistId, AttributeType.Tag, tagName, true);
        }


        public BasicDetails? GetArtistDetails(long artistId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BasicDetails>? GetArtistDetails(string artistName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BasicInfo> GetArtistTags(long artistId)
        {
            if (artistId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<ArtistRecord, ArtistInformation>(artistId, AttributeType.Tag);
        }

        public IEnumerable<BasicInfo> GetArtistCategories(long artistId)
        {
            if (artistId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<ArtistRecord, ArtistInformation>(artistId, AttributeType.Category);
        }
    }
}
