using Cyh.Net.Data;
using ImgRepo.Data.Enums;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Service.Implement;

namespace ImgRepo.Service
{
    /// <summary>
    /// 注入服務用的委派產生工廠
    /// </summary>
    public static class Factories
    {
        static Factories()
        {
            AttributeType.RegisterAttributeMetaData<NameInformation>("Name");
            AttributeType.RegisterAttributeMetaData<TagInformation>("Tag");
            AttributeType.RegisterAttributeMetaData<CategoryInformation>("Category");
        }

        public static IImageService GetImageService(IServiceProvider sp)
        {
            object? dataSource = sp.GetService(typeof(IDataSource));
            object? fileAccessService = sp.GetService(typeof(IFileAccessService));
            if (dataSource != null)
            {
                return (ImageService)Activator.CreateInstance(typeof(ImageService), dataSource, fileAccessService)!;
            }
            throw new Exception("IDataSource not found");
        }

        public static IArtistService GetArtistService(IServiceProvider sp)
        {
            object? dataSource = sp.GetService(typeof(IDataSource));
            object? fileAccessService = sp.GetService(typeof(IFileAccessService));
            if (dataSource != null)
            {
                return (ArtistService)Activator.CreateInstance(typeof(ArtistService), dataSource)!;
            }
            throw new Exception("IDataSource not found");
        }

        public static ICommonAttributeService GetCommonAttributeService(IServiceProvider sp)
        {
            object? dataSource = sp.GetService(typeof(IDataSource));
            if (dataSource != null)
            {
                return (CommonAttributeService)Activator.CreateInstance(typeof(CommonAttributeService), dataSource)!;
            }
            throw new Exception("IDataSource not found");
        }

        public static IFileAccessService GetFileAccessService(IServiceProvider sp, string baseUri)
        {
            return new FileAccessService(baseUri);
        }
    }
}
