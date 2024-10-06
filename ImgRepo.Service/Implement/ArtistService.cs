using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Artist;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class ArtistService : CommonObjectService<ArtistInformation, ArtistRecord>, IArtistService
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
            ArtistInformation artist = new ArtistInformation
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
            long artistId = artist.Id;
            if (artistDto.Tags != null)
            {
                foreach (string tag in artistDto.Tags)
                {
                    this.AddTag(artistId, tag);
                }
            }
            if (artistDto.Categories != null)
            {
                foreach (string category in artistDto.Categories)
                {
                    this.AddCategory(artistId, category);
                }
            }
            return artistId;
        }

        public long RenameArtist(long artistId, string newName) => this.Rename(artistId, newName);

        public IEnumerable<BasicDetails>? GetArtistDetails(string artistName)
        {
            throw new NotImplementedException();
        }
    }
}
