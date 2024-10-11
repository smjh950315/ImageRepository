using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.Entities.Artist;
using ImgRepo.Service.CommonService;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class ArtistService : CommonObjectService<ArtistInformation, ArtistRecord>, IArtistService
    {

        readonly IDataWriter<ArtistInformation> m_artistWriter;

        public ArtistService(IDataSource dataSource) : base(dataSource)
        {
            this.m_artistWriter = dataSource.GetWriter<ArtistInformation>();
        }

        public long CreateArtist(NewArtistDto? artistDto)
        {
            if (artistDto == null) return 0;

            using (this.m_dataSource.BeginTransaction())
            {
                ArtistInformation artist = new ArtistInformation
                {
                    Name = artistDto.Name,
                    Description = artistDto.Description,
                    Created = DateTime.Now,
                };
                this.m_artistWriter.Add(artist);
                if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
                if (artistDto.Tags != null)
                {
                    foreach (string tag in artistDto.Tags)
                    {
                        this.AddTag(artist.Id, tag);
                    }
                }
                if (artistDto.Categories != null)
                {
                    foreach (string category in artistDto.Categories)
                    {
                        this.AddCategory(artist.Id, category);
                    }
                }
                this.m_dataSource.CommitTransaction();
                return artist.Id;
            }
        }
    }
}
