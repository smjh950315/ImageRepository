namespace ImgRepo.Model.Interface
{
    public interface IBasicEntityRecord
    {
        /// <summary>
        /// relation id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// image, author, album
        /// </summary>
        public long ObjectId { get; set; }


        /// <summary>
        /// 0=none, 1=tag, 2=category
        /// </summary>
        public long AttrType { get; set; }

        /// <summary>
        /// tag_id, category_id
        /// </summary>
        public long AttrId { get; set; }
    }
}
