using ImgRepo.Model.Interface;

namespace ImgRepo.Model.Entities
{
    public class AlbumInformation : IBasicEntityInformation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
