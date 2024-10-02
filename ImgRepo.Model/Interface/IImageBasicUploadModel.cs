namespace ImgRepo.Model.Interface
{
    public interface IImageBasicUploadModel
    {
        string Name { get; set; }
        string Description { get; set; }
        string Tags { get; set; }
        string Categories { get; set; }
    }
}
