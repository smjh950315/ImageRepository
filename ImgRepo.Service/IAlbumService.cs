using ImgRepo.Service.Dto;

namespace ImgRepo.Service
{
    public interface IAlbumService
    {
        long AddImage(long albumId, long imageId);
        long RemoveImage(long albumId, long imageId);
        long CreateAlbum(NewAlbumDto? albumDto);
    }
}
