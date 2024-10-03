using ImgRepo.Model.Interface;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities
{
    /// <summary>
    /// 相簿資訊
    /// </summary>
    public class AlbumInformation : IBasicEntityInformation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
