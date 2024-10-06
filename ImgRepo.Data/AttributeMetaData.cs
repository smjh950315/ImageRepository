#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace ImgRepo.Data
{
    public class AttributeMetaData
    {
        public long TypeId { get; set; }
        public Type ModelType { get; set; }
    }
}
