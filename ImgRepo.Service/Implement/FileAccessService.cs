namespace ImgRepo.Service.Implement
{
    internal class FileAccessService : IFileAccessService
    {
        string m_basePath;

        public FileAccessService(string basePath)
        {
            if (basePath.EndsWith('/') || basePath.EndsWith('\\'))
            {
                this.m_basePath = basePath;
            }
            else
            {
                this.m_basePath = basePath + "/";
            }
        }

        public byte[] GetFile(string uri)
        {
            try
            {
                return File.ReadAllBytes(this.m_basePath + uri);
            }
            catch
            {
                return [];
            }
        }

        public bool RemoveFile(string uri)
        {
            try
            {
                File.Delete(this.m_basePath + uri);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveFile(string uri, byte[] data)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(this.m_basePath + uri)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(this.m_basePath + uri));
                }
                File.WriteAllBytes(this.m_basePath + uri, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
