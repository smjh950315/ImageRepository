using ImgRepo.Model.ViewModel;

namespace ImgRepo.Service
{
    public interface IAlbumService
    {
        BasicDetails? GetAlbumDetails(long albumId);
        long CreateAlbum(string albumName);
        long AddImage(long albumId, long imageId);
        long RemoveImage(long albumId, long imageId);
        long AddTag(long albumId, string tagName);
        long RemoveTag(long albumId, string tagName);
        long AddCategory(long albumId, string categoryName);
        long RemoveCategory(long albumId, string categoryName);
        long SetAlbumName(long albumId, string albumName);
        long DeleteAlbum(long albumId);
    }
}
