using Cyh.Net.Data;
using ImgRepo.Model.Entities;

namespace ImgRepo.Service.Implement
{
    public class ArtistService
    {
        IQueryable<ArtistInformation> m_artistInformations;
        IQueryable<ArtistAdditionalRecord> m_artistAdditionalRecords;
        public ArtistService(IDataSource dataSource)
        {
            this.m_artistAdditionalRecords = dataSource.GetQueryable<ArtistAdditionalRecord>();
            this.m_artistInformations = dataSource.GetQueryable<ArtistInformation>();
        }
    }
}
