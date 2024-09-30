using ImgRepo.Model.ViewModel;
namespace ImgRepo.Service
{
    public interface IImageService
    {
        void UpdateImageTag(long imageId, string tagName, bool _delete);
        byte[] GetFileBytes(long fileId);
        IEnumerable<ImageThumbView> GetImageThumbViews(QueryModel? queryModel);

        ArtworkDetails? GetImageDetails(long id);
        ArtworkDetails? GetAlbumDetails(long id);

        BasicDetails? GetTagDetails(long id);
        BasicDetails? GetCategoryDetails(long id);

        long GetImageFileId(long imageId);

        void SaveImageFile(BasicDetails? imgDetails, byte[]? data);
    }
}
