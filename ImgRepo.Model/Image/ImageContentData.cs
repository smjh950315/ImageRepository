namespace ImgRepo.Model.Image
{
    public class ImageContentData
    {
        public Guid Guid { get; set; }
        public byte[] Data { get; set; }
        public string ExtName { get; set; }
    }
}
