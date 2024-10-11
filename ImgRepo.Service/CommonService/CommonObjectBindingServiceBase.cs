using Cyh.Net.Data;
using ImgRepo.Data.Interface;
using System.Reflection;

namespace ImgRepo.Service.CommonService
{
    /// <summary>
    /// 連接主要物件與次要物件的服務
    /// </summary>
    internal abstract class CommonObjectBindingServiceBase
    {
        static readonly Dictionary<string, MethodInfo> CachedMethods;

        static CommonObjectBindingServiceBase()
        {
            CachedMethods = new();
            string implPrefix = "Impl_";
            MethodInfo[] methodImpls = typeof(CommonObjectBindingServiceBase).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (MethodInfo method in methodImpls)
            {
                if (!method.Name.Contains(implPrefix)) continue;
                CachedMethods[method.Name] = method;
            }
        }

        long Impl_SetObjectBinding<TMainObject, TSubObject, TObjectBinding>(long _mainObjId, long _subObjId, bool _delete)
            where TMainObject : class, IBasicEntityInformation, new()
            where TSubObject : class, IBasicEntityInformation, new()
            where TObjectBinding : class, IObjectBindingRecord, new()
        {
            IQueryable<TObjectBinding> bindings = this.m_dataSource.GetQueryable<TObjectBinding>();
            TObjectBinding? binding = bindings.FirstOrDefault(x => x.MainObjectId == _mainObjId && x.SubObjectId == _subObjId);
            if (binding == null)
            {
                if (_delete) return 0;
                binding = new TObjectBinding
                {
                    MainObjectId = _mainObjId,
                    SubObjectId = _subObjId,
                };
                this.m_dataSource.GetWriter<TObjectBinding>().Add(binding);
            }
            else
            {
                if (!_delete) return 0;
                this.m_dataSource.GetWriter<TObjectBinding>().Remove(binding);
            }
            this.m_dataSource.Save();
            return _subObjId;
        }

        protected IDataSource m_dataSource;

        public abstract Type MainObjectType { get; }
        public abstract Type SubObjectType { get; }
        public abstract Type BindingType { get; }

        protected CommonObjectBindingServiceBase(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
        }

        /// <summary>
        /// 被連結或解除連結的物件ID
        /// </summary>
        /// <param name="_mainObjId">主要物件ID</param>
        /// <param name="_subObjId">次要物件ID</param>
        /// <param name="_delete">是否刪除</param>
        /// <returns>次要物件ID，或是0代表失敗</returns>
        public virtual long SetBinding(long _mainObjId, long _subObjId, bool _delete)
        {
            if (_mainObjId <= 0 || _subObjId <= 0) return 0;
            return (long)CachedMethods.FindAndExecuteByName(
                this, "Impl_SetObjectBinding",
                [this.MainObjectType, this.SubObjectType, this.BindingType],
                _mainObjId, _subObjId, _delete)!;
        }
    }
}
