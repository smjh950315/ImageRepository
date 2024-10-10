namespace ImgRepo.Data.Interface
{
    /// <summary>
    /// 連接兩個物件的紀錄
    /// </summary>
    public interface IObjectBindingRecord
    {
        long Id { get; set; }
        long MainObjectId { get; set; }
        long SubObjectId { get; set; }
    }
}
