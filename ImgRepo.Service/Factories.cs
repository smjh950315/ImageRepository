using Cyh.Net.Data;

namespace ImgRepo.Service
{
    public static class Factories
    {
        public static IImageService? GetImageService(IServiceProvider sp)
        {
            object? dataSource = sp.GetService(typeof(IDataSource));
            if (dataSource != null)
            {
                //return (ImageService)Activator.CreateInstance(typeof(ImageService), dataSource);
            }
            return null;
        }
    }
}
