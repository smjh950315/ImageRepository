namespace ImgRepo.Service.Implement
{
    internal class FileAccessService : IFileAccessService
    {
        readonly string m_basePath;

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

        public byte[] GetFile(string uri, long size)
        {
            try
            {
                if (size == 0)
                {
                    return File.ReadAllBytes(this.m_basePath + uri);
                }
                else
                {
                    using (FileStream fs = File.OpenRead(this.m_basePath + uri))
                    {
                        byte[] buffer = new byte[size];
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
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

        public Stream GetWriteStream(string uri)
        {
            if (!Directory.Exists(Path.GetDirectoryName(this.m_basePath + uri)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.m_basePath + uri));
            }
            return File.Create(this.m_basePath + uri);
        }

        public long GetFileSize(string uri)
        {
            try
            {
                return new FileInfo(this.m_basePath + uri).Length;
            }
            catch
            {
                return 0;
            }
        }
    }
}
