namespace ImgRepo.Service
{
    /// <summary>
    /// 全域設定
    /// </summary>
    public static class GlobalSettings
    {
        /// <summary>
        /// 屬性分隔用的符號
        /// </summary>
        public static string KeywordSplitter { get; set; }

        static GlobalSettings()
        {
            KeywordSplitter = ",";
        }
    }
}
