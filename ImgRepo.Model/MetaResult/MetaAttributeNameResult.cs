namespace ImgRepo.Model.MetaResult
{
    public class MetaAttributeNameResult
    {
        string? _value;
        public long ObjectId { get; set; }
        public long AttrType { get; set; }
        public string Value
        {
            get => _value ?? string.Empty;
            set => _value = value;
        }
    }
}
