namespace ImgRepo.Service
{
    public interface IFileAccessService
    {
        /// <summary>
        /// 用uri取得檔案
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <returns>是否成功</returns>
        byte[] GetFile(string uri);

        /// <summary>
        /// 儲存檔案到指定uri
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <param name="data">位元檔案</param>
        /// <returns>是否成功</returns>
        bool SaveFile(string uri, byte[] data);

        /// <summary>
        /// 移除指定uri的檔案
        /// </summary>
        /// <param name="uri">指定uri</param>
        /// <returns>是否成功</returns>
        bool RemoveFile(string uri);
    }
}
