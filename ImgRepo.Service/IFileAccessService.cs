namespace ImgRepo.Service
{
    public interface IFileAccessService
    {
        /// <summary>
        /// 用uri取得檔案
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <returns>是否成功</returns>
        byte[] GetFile(string uri, long size = 0);

        /// <summary>
        /// 儲存檔案到指定uri
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <param name="data">位元檔案</param>
        /// <returns>是否成功</returns>
        bool SaveFile(string uri, byte[] data);

        /// <summary>
        /// 儲存檔案到指定uri
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <returns>位元檔案流</returns>
        Stream GetWriteStream(string uri);

        /// <summary>
        /// 移除指定uri的檔案
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <returns>是否成功</returns>
        bool RemoveFile(string uri);

        /// <summary>
        /// 取得指定uri的檔案大小
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <returns>位元大小</returns>
        long GetFileSize(string uri);
    }
}
