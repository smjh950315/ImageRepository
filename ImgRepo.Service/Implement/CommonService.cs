using Cyh.Net;
using Cyh.Net.Data;
using Cyh.Net.Data.Predicate;
using ImgRepo.Data.Enums;
using ImgRepo.Data.Interface;
using ImgRepo.Model;
using ImgRepo.Model.Common;
using ImgRepo.Model.Query;
using System.Diagnostics;
using System.Reflection;

namespace ImgRepo.Service.Implement
{
    internal abstract class CommonService
    {
        static Dictionary<string, MethodInfo> m_methodCache;
        static Dictionary<string, MethodInfo> MethodImpls;

        static CommonService()
        {
            m_methodCache = new();

            MethodImpls = new();
            string implPrefix = "Impl_";
            var methodImpls = typeof(CommonService).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (MethodInfo method in methodImpls)
            {
                if (!method.Name.Contains(implPrefix)) continue;
                MethodImpls[method.Name] = method;
            }

            foreach (MethodInfo method in typeof(CommonService).GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                m_methodCache[method.Name] = method;
            }
        }
        class GroupCountComparerMetaData<TKey>
        {
            public TKey Key { get; set; }
            public long LeftCount { get; set; }
            public long RightCount { get; set; }

            public static IQueryable<GroupCountComparerMetaData<TKey>> JoinGroup<TObj>(IQueryable<IGrouping<TKey, TObj>> left, IQueryable<IGrouping<TKey, TObj>> right)
            {
                var lmeta = left.Select(x => new
                {
                    Key = x.Key,
                    Count = x.Count()
                });
                var rmeta = right.Select(x => new
                {
                    Key = x.Key,
                    Count = x.Count()
                });
                return lmeta.Join(rmeta, l => l.Key, r => r.Key, (l, r) => new GroupCountComparerMetaData<TKey>
                {
                    Key = l.Key,
                    LeftCount = l.Count,
                    RightCount = r.Count
                });
            }

        }
        class MetaAttributeNameResult
        {
            string? _value;
            public long ObjectId { get; set; }
            public long AttrType { get; set; }
            public string Value
            {
                get => this._value ?? string.Empty;
                set => this._value = value;
            }
        }

        static object? InvokeStaticWithCommonSerivce(MethodInfo method, CommonService commonService, params object[] parameters)
        {
            var _parameters = new object[parameters.Length + 1];
            _parameters[0] = commonService;
            parameters.CopyTo(_parameters, 1);
            return method.Invoke(null, _parameters);
        }

        static object? CallStaticMethod(CommonService instance, string methodName, object[] parameters)
        {
            if (MethodImpls.TryGetValue(methodName, out MethodInfo? method))
            {
                return InvokeStaticWithCommonSerivce(method, instance, parameters);
            }
            return null;
        }

        static object? CallStaticGenericMethod(Type[] genericParameters, CommonService instance, string methodName, object[] parameters)
        {
            if (MethodImpls.TryGetValue(methodName, out MethodInfo? method))
            {
                if (method.IsGenericMethod)
                {
                    MethodInfo genericMethod = method.MakeGenericMethod(genericParameters);
                    return InvokeStaticWithCommonSerivce(genericMethod, instance, parameters);
                }
            }
            return null;
        }

        static long Impl_SetObjectAttributeData<TObject, TRecord, TAttribute>(CommonService commonService, long objectId, string attrValue, bool _delete)
            where TObject : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            // no object
            if (objectId == 0) return 0;
            IDataSource dataSource = commonService.m_dataSource;
            IQueryable<TObject> objs = dataSource.GetQueryable<TObject>();
            IQueryable<TAttribute> attrs = dataSource.GetQueryable<TAttribute>();
            IQueryable<TRecord> records = dataSource.GetQueryable<TRecord>();
            IDataWriter<TAttribute> attrWriter = dataSource.GetWriter<TAttribute>();
            IDataWriter<TRecord> recordWriter = dataSource.GetWriter<TRecord>();

            TAttribute? attr = attrs.FirstOrDefault(a => a.Value == attrValue);

            long attributeTypeId = AttributeType.GetAttributeTypeId<TAttribute>();
            if (attributeTypeId == 0) return 0;

            if (_delete)
            {
                if (attr == null)
                {
                    // no attr, nothing to do
                    return 0;
                }
                else
                {
                    // has attr, remove record
                    TRecord? record = records.FirstOrDefault(r => r.ObjectId == objectId && r.AttrId == attr.Id);
                    if (record != null)
                    {
                        recordWriter.Remove(record);
                        if (!Lib.TryExecute(() => dataSource.Save()))
                        {
                            return -1;
                        }
                    }
                    return attr.Id;
                }
            }
            else
            {
                if (attr == null)
                {
                    // no attr, create attr and record
                    attr = new TAttribute
                    {
                        Value = attrValue,
                        Created = DateTime.Now,
                    };
                    attrWriter.Add(attr);
                    if (!Lib.TryExecute(() => dataSource.Save()))
                    {
                        return -1;
                    }
                    TRecord record = new TRecord
                    {
                        ObjectId = objectId,
                        AttrId = attr.Id,
                        AttrType = attributeTypeId,
                    };
                    recordWriter.Add(record);
                    if (!Lib.TryExecute(() => dataSource.Save()))
                    {
                        return -1;
                    }
                }
                else
                {
                    // has attr, create record
                    TRecord record = new TRecord
                    {
                        ObjectId = objectId,
                        AttrId = attr.Id,
                        AttrType = attributeTypeId,
                    };
                    recordWriter.Add(record);
                    if (!Lib.TryExecute(() => dataSource.Save()))
                    {
                        return -1;
                    }
                }
                return attr.Id;
            }
        }

        static IEnumerable<BasicInfo> Impl_GetObjectAttributeInformations<TRecord, TAttribute>(CommonService commonService, long objectId)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            long attrType = AttributeType.GetAttributeTypeId<TAttribute>();
            IQueryable<TRecord> records = commonService.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttribute> attrs = commonService.m_dataSource.GetQueryable<TAttribute>();
            return from r in records
                   join a in attrs on r.AttrId equals a.Id
                   where r.ObjectId == objectId && r.AttrType == attrType
                   select new BasicInfo
                   {
                       Id = a.Id,
                       Name = a.Value,
                   };
        }

        protected abstract Type ObjectType { get; }
        protected abstract Type RecordType { get; }

        protected IDataSource m_dataSource;

        protected CommonService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
        }

        public long SetObjectAttributeData<TAttribute>(long objectId, string attrValue, bool _delete)
        {
            if (objectId == 0) return 0;
            return (long)CallStaticGenericMethod(
                [this.ObjectType, this.RecordType, typeof(TAttribute)],
                this, "Impl_SetObjectAttributeData",
                [objectId, attrValue, _delete])!;
        }

        public IEnumerable<BasicInfo> GetObjectAttributeInformations<TAttribute>(long objectId)
        {
            if (objectId == 0) return Enumerable.Empty<BasicInfo>();
            return (IEnumerable<BasicInfo>)CallStaticGenericMethod(
                [this.RecordType, typeof(TAttribute)],
                this, "Impl_GetObjectAttributeInformations",
                [objectId])!;
        }

        protected long setObjectAttrData<TRecord, TAttr>(long objectId, long attrType, string attrValue, bool _delete)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityAttribute, new()
        {
            // no image
            if (objectId == 0) return 0;

            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IDataWriter<TAttr> attrWriter = this.m_dataSource.GetWriter<TAttr>();
            IDataWriter<TRecord> recordWriter = this.m_dataSource.GetWriter<TRecord>();

            TAttr? attr = attrs.FirstOrDefault(a => a.Value == attrValue);

            if (_delete)
            {
                if (attr == null)
                {
                    // no attr, nothing to do
                    return 0;
                }
                else
                {
                    // has attr, remove record
                    TRecord? record = records.FirstOrDefault(r => r.ObjectId == objectId && r.AttrId == attr.Id);
                    if (record != null)
                    {
                        recordWriter.Remove(record);
                        if (!Lib.TryExecute(() => this.m_dataSource.Save()))
                        {
                            return -1;
                        }
                    }
                    return attr.Id;
                }
            }
            else
            {
                if (attr == null)
                {
                    // no attr, create attr and record
                    attr = new TAttr
                    {
                        Value = attrValue,
                        Created = DateTime.Now,
                    };
                    attrWriter.Add(attr);
                    if (!Lib.TryExecute(() => this.m_dataSource.Save()))
                    {
                        return -1;
                    }
                    TRecord record = new TRecord
                    {
                        ObjectId = objectId,
                        AttrId = attr.Id,
                        AttrType = attrType,
                    };
                    recordWriter.Add(record);
                    if (!Lib.TryExecute(() => this.m_dataSource.Save()))
                    {
                        return -1;
                    }
                }
                else
                {
                    // has attr, create record
                    TRecord record = new TRecord
                    {
                        ObjectId = objectId,
                        AttrId = attr.Id,
                        AttrType = attrType,
                    };
                    recordWriter.Add(record);
                    if (!Lib.TryExecute(() => this.m_dataSource.Save()))
                    {
                        return -1;
                    }
                }
                return attr.Id;
            }
        }

        protected long setObjectAttr<TObject, TRecord, TAttr>(long objectId, long attrType, string attrValue, bool _delete)
            where TObject : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityAttribute, new()
        {
            if (objectId == 0) return 0;
            IQueryable<TObject> objs = this.m_dataSource.GetQueryable<TObject>();
            if (!objs.Any(o => o.Id == objectId)) return 0;
            return this.setObjectAttrData<TRecord, TAttr>(objectId, attrType, attrValue, _delete);
        }


        protected IEnumerable<BasicInfo> getObjectAttrInfos<TRecord, TAttr>(long objectId, long attrType)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityAttribute, new()
        {
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            return from r in records
                   join a in attrs on r.AttrId equals a.Id
                   where r.ObjectId == objectId && r.AttrType == attrType
                   select new BasicInfo
                   {
                       Id = a.Id,
                       Name = a.Value,
                   };
        }

        protected long renameObject<TObj>(long objectId, string newName) where TObj : class, IBasicEntityInformation, new()
        {
            if (objectId == 0) return 0;
            TObj? obj = this.m_dataSource.GetQueryable<TObj>().FirstOrDefault(i => i.Id == objectId);
            if (obj == null || obj.Name == newName) return 0;
            obj.Name = newName;
            obj.Updated = DateTime.Now;
            this.m_dataSource.GetWriter<TObj>().Update(obj);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return obj.Id;
        }

        protected long removeObject<TObj>(long objectId) where TObj : class, IBasicEntityInformation, new()
        {
            if (objectId == 0) return 0;
            TObj? obj = this.m_dataSource.GetQueryable<TObj>().FirstOrDefault(i => i.Id == objectId);
            if (obj == null) return 0;
            this.m_dataSource.GetWriter<TObj>().Remove(obj);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return obj.Id;
        }

        protected IQueryable<long> getObjectIdsByAttrName<TObj, TRecord, TAttr>(long attrType, IEnumerable<ExpressionData> exprDatas)
            where TObj : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityAttribute, new()
        {
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            IQueryable<MetaAttributeNameResult> metaQueryable = records
                .Join(attrs,
                    r => r.AttrId,
                    a => a.Id,
                    (r, a) => new MetaAttributeNameResult
                    {
                        ObjectId = r.ObjectId,
                        AttrType = r.AttrType,
                        Value = a.Value
                    }).Where(r => r.AttrType == attrType);

            if (exprDatas.IsNullOrEmpty())
                return metaQueryable.Select(r => r.ObjectId);

            foreach (ExpressionData exprData in exprDatas)
            {
                exprData.MemberName = "Value";
            }

            System.Linq.Expressions.Expression<Func<MetaAttributeNameResult, bool>>? predicate = Predicate.GetExpression<MetaAttributeNameResult>(exprDatas);

            if (predicate == null)
                return metaQueryable.Select(r => r.ObjectId);

            return metaQueryable.Where(predicate).Select(r => r.ObjectId);
        }

        protected IQueryable<long> getObjectIdsByAttributeExprData<TRecord, TAttr>(long attrType, ExpressionData expressionData, string str)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityAttribute, new()
        {
            Debug.Assert(!str.IsNullOrEmpty());

            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>().Where(r => r.AttrType == attrType);
            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            var result = records.Join(attrs, r => r.AttrId, a => a.Id, (r, a) => new
            {
                ObjectId = r.ObjectId,
                AttrId = a.Id,
                AttrName = a.Value,
            });
            var originalGroups = result.GroupBy(x => x.ObjectId);
            string name = str;
            string[] names = str.Split(GlobalSettings.KeywordSplitter, StringSplitOptions.RemoveEmptyEntries);
            bool isCollection = names.Length > 1;

            if (isCollection)
            {
                switch (expressionData.CompareType)
                {
                    case CompareType.Equal:
                    {
                        var groups = result
                            .Where(x => names.Contains(x.AttrName))
                            .GroupBy(x => x.ObjectId);

                        return GroupCountComparerMetaData<long>
                            .JoinGroup(originalGroups, groups)
                            .Where(x => x.LeftCount == x.RightCount).Select(x => x.Key);
                    }
                    case CompareType.NotEqual:
                    {
                        var groups = result
                            .Where(x => names.Contains(x.AttrName))
                            .GroupBy(x => x.ObjectId);

                        return GroupCountComparerMetaData<long>
                            .JoinGroup(originalGroups, groups)
                            .Where(x => x.LeftCount != x.RightCount).Select(x => x.Key);
                    }
                    case CompareType.Contains:
                    {
                        return result
                            .Where(x => names.Contains(x.AttrName))
                            .GroupBy(x => x.ObjectId)
                            .Where(g => g.Count() == names.Length)
                            .Select(g => g.Key);
                    }
                    case CompareType.IsAnyOf:
                    {
                        return result
                            .Where(x => names.Contains(x.AttrName))
                            .Select(x => x.ObjectId);
                    }
                    case CompareType.ContainsAnyOf:
                    {
                        var iterator = names.GetEnumerator();
                        iterator.MoveNext();
                        var first = (string)iterator.Current;
                        IQueryable<long> ids = result.Where(x => x.AttrName.Contains(first)).Select(x => x.ObjectId);
                        while (iterator.MoveNext())
                        {
                            string _name = (string)iterator.Current;
                            ids.Concat(result.Where(x => x.AttrName.Contains(_name)).Select(x => x.ObjectId));
                        }
                        return ids;
                    }
                    case CompareType.IncludeAnyOf:
                    {
                        var iterator = names.GetEnumerator();
                        iterator.MoveNext();
                        var first = (string)iterator.Current;
                        IQueryable<long> ids = result.Where(x => first.Contains(x.AttrName)).Select(x => x.ObjectId);
                        while (iterator.MoveNext())
                        {
                            string _name = (string)iterator.Current;
                            ids.Concat(result.Where(x => _name.Contains(x.AttrName)).Select(x => x.ObjectId));
                        }
                        return ids;
                    }
                }
            }
            else
            {
                switch (expressionData.CompareType)
                {
                    case CompareType.Equal:
                    {
                        var groups = result
                            .Where(x => x.AttrName == name)
                            .GroupBy(x => x.ObjectId);
                        return GroupCountComparerMetaData<long>
                            .JoinGroup(originalGroups, groups)
                            .Where(x => x.LeftCount == 1).Select(x => x.Key);
                    }
                    case CompareType.NotEqual:
                    {
                        return result
                            .Where(x => x.AttrName != name)
                            .Select(x => x.ObjectId);
                    }
                    case CompareType.Contains:
                    {
                        return result
                            .Where(x => x.AttrName == name)
                            .Select(x => x.ObjectId);
                    }
                    case CompareType.IsAnyOf:
                    {
                        return result
                            .Where(x => x.AttrName == name)
                            .Select(x => x.ObjectId);
                    }
                    case CompareType.ContainsAnyOf:
                    {
                        return result
                            .Where(x => x.AttrName.Contains(name))
                            .Select(x => x.ObjectId);
                    }
                    case CompareType.IncludeAnyOf:
                    {
                        return result
                            .Where(x => name.Contains(x.AttrName))
                            .Select(x => x.ObjectId);
                    }
                }
            }
            return result.Select(x => x.ObjectId);
        }

        protected IQueryable<long> queryAllAttributes<TObj, TRecord>(QueryModel? queryModel)
            where TObj : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
        {
            if (queryModel == null || queryModel.Conditions == null)
            {
                return this.m_dataSource.GetQueryable<TObj>().Select(x => x.Id);
            }
            else
            {
                IQueryable<long>? result = null;
                ApiCondition[] conds = queryModel.Conditions;
                var method_IdsByAttributeBase = m_methodCache["getObjectIdsByAttributeExprData"];
                foreach (ApiCondition cond in conds)
                {
                    if ((CompareType)cond.CompareType == CompareType.None) continue;
                    if (cond.TryGetExpressionData(out ExpressionData? exprData))
                    {
                        if (exprData.ConstantValue is not string str) continue;
                        IQueryable<long>? meta = null;
                        if (cond.Type == "Name")
                        {
                            switch (exprData.CompareType)
                            {
                                case CompareType.Contains:
                                {
                                    if (str.Contains(GlobalSettings.KeywordSplitter))
                                    {
                                        string[] strs = str.Split(GlobalSettings.KeywordSplitter, StringSplitOptions.RemoveEmptyEntries);
                                        IPredicateHolder<TObj>? predicate = null;
                                        foreach (string s in strs)
                                        {
                                            if (predicate == null)
                                            {
                                                predicate = Predicate.NewPredicateHolder(Predicate.GetExpression<TObj>("Name", CompareType.Contains, s));
                                            }
                                            else
                                            {
                                                predicate.And(Predicate.GetExpression<TObj>("Name", CompareType.Contains, s));
                                            }

                                        }
                                        if (predicate != null)
                                        {
                                            meta = this.m_dataSource
                                              .GetQueryable<TObj>()
                                              .Where(predicate.GetPredicate())
                                              .Select(x => x.Id);
                                        }
                                    }
                                    break;
                                }
                                case CompareType.IsAnyOf:
                                    if (str.Contains(','))
                                    {
                                        exprData.ConstantValue = str.Split(GlobalSettings.KeywordSplitter, StringSplitOptions.RemoveEmptyEntries);
                                    }
                                    else
                                    {
                                        exprData.ConstantValue = new string[] { str };
                                    }
                                    break;
                                default:
                                    break;
                            }
                            meta ??= this.m_dataSource
                                .GetQueryable<TObj>()
                                .Where(Predicate.GetExpression<TObj>("Name", exprData.CompareType, exprData.ConstantValue))
                                .Select(x => x.Id);
                        }
                        else
                        {
                            var attrMetaData = AttributeType.GetAttributeMetaData(cond.Type);
                            if (attrMetaData == null) continue;
                            var genericMethod = method_IdsByAttributeBase.MakeGenericMethod(typeof(TRecord), attrMetaData.ModelType);
                            meta = (IQueryable<long>?)genericMethod.Invoke(this, [attrMetaData.TypeId, exprData, str]);
                        }
                        if (meta != null)
                        {
                            result = result != null
                                ? (exprData.LinkType == LinkType.And ? result.Intersect(meta) : result.Concat(meta))
                                : meta;
                        }
                    }
                }
                return result ?? this.m_dataSource.GetQueryable<TObj>().Select(x => x.Id);
            }
        }
    }
}
