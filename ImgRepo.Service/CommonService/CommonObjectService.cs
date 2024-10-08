using Cyh.Net.Data;
using ImgRepo.Data.Interface;

namespace ImgRepo.Service.CommonService
{
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
