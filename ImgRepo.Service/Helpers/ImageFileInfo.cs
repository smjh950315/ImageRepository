namespace ImgRepo.Service.Helpers
{
    public class ImageFileInfo
    {
        string? _format;
        public int Width { get; set; }
        public int Height { get; set; }
        public int Comp { get; set; }
        public string Format
        {
            get
            {
                if (_format == null)
                {
                    return "Unknown";
                }
                return _format;
            }
            set { _format = value; }
        }
    }
}
