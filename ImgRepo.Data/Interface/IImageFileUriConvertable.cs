namespace ImgRepo.Data.Interface
{
    public interface IImageFileUriConvertable
    {
        long ImageId { get; set; }
        string Format { get; set; }
        string Uri { get; set; }
    }
}
