using Cyh.Net.Data;
using ImgRepo.Data.Interface;

namespace ImgRepo.Service.CommonService
{
    /// <summary>
    /// 物件與屬性相關服務
    /// </summary>
    /// <typeparam name="TObject">物件類型</typeparam>
    /// <typeparam name="TRecord">紀錄類型</typeparam>
    internal class CommonObjectService<TObject, TRecord> : CommonObjectServiceBase
        where TObject : class, IBasicEntityInformation, new()
        where TRecord : class, IBasicEntityRecord, new()
    {
        public override Type ObjectType => typeof(TObject);
        public override Type RecordType => typeof(TRecord);

        public CommonObjectService(IDataSource dataSource) : base(dataSource)
        {
        }
    }
}
