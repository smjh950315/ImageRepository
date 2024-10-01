using ImgRepo.Model.ApiModel;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Dto;
namespace ImgRepo.Service
{
    public interface IImageService
    {
        BasicDetails? GetAlbumDetails(long id);
        BasicDetails? GetImageDetails(long id);

        BasicDetails? GetTagDetails(long id);
        BasicDetails? GetCategoryDetails(long id);

        IEnumerable<ImageThumbView> GetImageThumbViews(QueryModel? queryModel);
        ApiFileModel? GetFileBytes(long fileId);
        ApiFileModel? GetFullImage(long imgId);
        ApiFileModel? GetThumbnail(long imgId);

        IEnumerable<BasicInfo> GetTags(long imgId);
        IEnumerable<BasicInfo> GetCategories(long imgId);

        long UpdateImageTag(long imageId, string tagName, bool _delete);
        long GetImageFileId(long imageId);
        long UploadImage(NewImageDto? imageDto);
    }
}
