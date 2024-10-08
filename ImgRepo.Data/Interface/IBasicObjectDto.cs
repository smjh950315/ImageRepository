namespace ImgRepo.Data.Interface
{
    public interface IBasicObjectDto
    {
        /// <summary>
        /// 物件名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 物件描述
        /// </summary>
        string? Description { get; set; }
    }
}
