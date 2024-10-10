using Cyh.Net.Data;
using ImgRepo.Data.Interface;

namespace ImgRepo.Service.CommonService
{
    /// <summary>
    /// 連接主要物件與次要物件的服務
    /// </summary>
    /// <typeparam name="TMainObject">主要物件</typeparam>
    /// <typeparam name="TSubObject">次要物件</typeparam>
    /// <typeparam name="TObjectBinding">關聯紀錄</typeparam>
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
