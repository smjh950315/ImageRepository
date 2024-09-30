namespace ImgRepo.Model.ViewModel
{
    public class QueryModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateBegin { get; set; }
        public DateTime? CreateEnd { get; set; }
        public DateTime? UpdateBegin { get; set; }
        public DateTime? UpdateEnd { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }
        public string? Album { get; set; }
    }
}
