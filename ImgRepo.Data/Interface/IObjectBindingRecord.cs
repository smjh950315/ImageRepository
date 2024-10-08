namespace ImgRepo.Data.Interface
{
    public interface IObjectBindingRecord
    {
        long Id { get; set; }
        long MainObjectId { get; set; }
        long SubObjectId { get; set; }
    }
}
