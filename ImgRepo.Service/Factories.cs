using Cyh.Net.Data;
using ImgRepo.Service.Implement;

namespace ImgRepo.Service
{
    public static class Factories
    {
        static Factories()
        {
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
        //public static IAlbumService? GetAlbumService(IServiceProvider sp)
        //{
        //    object? dataSource = sp.GetService(typeof(IDataSource));
        //    if (dataSource != null)
        //    {
        //        return (AlbumService)Activator.CreateInstance(typeof(AlbumService), dataSource);
        //    }
        //    return null;
        //}
    }
}
