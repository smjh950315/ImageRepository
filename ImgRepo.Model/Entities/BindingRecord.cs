namespace ImgRepo.Model.Entities
{
    public class BindingRecord
    {
        public long Id { get; set; }


        /// <summary>
        /// 0=none, 1=image, 2=album
        /// </summary>
        public long ObjType { get; set; }
        public long ObjId { get; set; }
        

        /// <summary>
        /// 0=none, 1=tag, 2=category, 3=author
        /// </summary>
        public long AttType { get; set; }
        public long AttId { get; set; }
    }
}
