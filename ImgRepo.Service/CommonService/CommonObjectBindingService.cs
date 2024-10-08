using Cyh.Net.Data;
using ImgRepo.Data.Interface;

namespace ImgRepo.Service.CommonService
{
    internal class CommonObjectBindingService<TMainObject, TSubObject, TObjectBinding> : CommonObjectBindingServiceBase
        where TMainObject : class, IBasicEntityInformation, new()
        where TSubObject : class, IBasicEntityInformation, new()
        where TObjectBinding : class, IObjectBindingRecord, new()
    {
        public override Type MainObjectType => typeof(TMainObject);
        public override Type SubObjectType => typeof(TSubObject);
        public override Type BindingType => typeof(TObjectBinding);

        public CommonObjectBindingService(IDataSource dataSource) : base(dataSource)
        {
        }
    }
}
