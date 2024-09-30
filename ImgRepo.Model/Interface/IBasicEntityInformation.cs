namespace ImgRepo.Model.Interface
{
    public interface IBasicEntityInformation
    {
        long Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}
