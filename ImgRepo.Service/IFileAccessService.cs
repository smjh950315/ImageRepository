namespace ImgRepo.Service
{
    public interface IFileAccessService
    {
        byte[] GetFile(string uri);
        bool SaveFile(string uri, byte[] data);
        bool RemoveFile(string uri);
    }
}
