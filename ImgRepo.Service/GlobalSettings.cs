namespace ImgRepo.Service
{
    public static class GlobalSettings
    {
        public static string KeywordSplitter { get; set; }
        static GlobalSettings()
        {
            KeywordSplitter = ",";
        }
    }
}
