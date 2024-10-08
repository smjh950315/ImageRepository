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

namespace ImgRepo.Service.CommonService
{
    internal abstract class CommonObjectServiceBase : ICommonObjectService
    {
        static readonly Dictionary<string, MethodInfo> CachedMethods;

        static CommonObjectServiceBase()
        {
            CachedMethods = new();
            string implPrefix = "Impl_";
            var methodImpls = typeof(CommonObjectServiceBase).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (MethodInfo methodImpl in methodImpls)
            {
                if (!methodImpl.Name.Contains(implPrefix)) continue;
                CachedMethods[methodImpl.Name] = methodImpl;
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
                    x.Key,
                    Count = x.Count()
                });
                var rmeta = right.Select(x => new
                {
                    x.Key,
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

        long Impl_SetObjectAttributeData<TObject, TRecord, TAttribute>(long objectId, string attrValue, bool _delete)
            where TObject : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            // no object
            if (objectId == 0) return 0;
            IQueryable<TObject> objs = this.m_dataSource.GetQueryable<TObject>();
            IQueryable<TAttribute> attrs = this.m_dataSource.GetQueryable<TAttribute>();
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IDataWriter<TAttribute> attrWriter = this.m_dataSource.GetWriter<TAttribute>();
            IDataWriter<TRecord> recordWriter = this.m_dataSource.GetWriter<TRecord>();

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
                    TRecord? record = records.FirstOrDefault(r => r.ObjectId == objectId && r.AttrId == attr.Id && r.AttrType == attributeTypeId);
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
                    attr = new TAttribute
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
                        AttrType = attributeTypeId,
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
                        AttrType = attributeTypeId,
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

        IEnumerable<BasicInfo> Impl_GetObjectAttributeInformations<TRecord, TAttribute>(long objectId)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            long attrType = AttributeType.GetAttributeTypeId<TAttribute>();
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttribute> attrs = this.m_dataSource.GetQueryable<TAttribute>();
            return from r in records
                   join a in attrs on r.AttrId equals a.Id
                   where r.ObjectId == objectId && r.AttrType == attrType
                   select new BasicInfo
                   {
                       Id = a.Id,
                       Name = a.Value,
                   };
        }

        long Impl_RenameObject<TObject>(long objectId, string newName) where TObject : class, IBasicEntityInformation, new()
        {
            if (objectId == 0) return 0;
            TObject? obj = this.m_dataSource.GetQueryable<TObject>().FirstOrDefault(i => i.Id == objectId);
            if (obj == null || obj.Name == newName) return 0;
            obj.Name = newName;
            obj.Updated = DateTime.Now;
            this.m_dataSource.GetWriter<TObject>().Update(obj);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return obj.Id;
        }

        long Impl_RemoveObject<TObject>(long objectId) where TObject : class, IBasicEntityInformation, new()
        {
            if (objectId == 0) return 0;
            TObject? obj = this.m_dataSource.GetQueryable<TObject>().FirstOrDefault(i => i.Id == objectId);
            if (obj == null) return 0;
            this.m_dataSource.GetWriter<TObject>().Remove(obj);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return obj.Id;
        }

        IQueryable<long> Impl_GetObjectIdsByAttributeName<TRecord, TAttribute>(IEnumerable<ExpressionData> exprDatas)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            long attrType = AttributeType.GetAttributeTypeId<TAttribute>();
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttribute> attrs = this.m_dataSource.GetQueryable<TAttribute>();
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

        IQueryable<long> Impl_GetObjectIdsByAttributeExpressionData<TRecord, TAttribute>(ExpressionData expressionData, string str)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            Debug.Assert(!str.IsNullOrEmpty());
            long attrType = AttributeType.GetAttributeTypeId<TAttribute>();
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>().Where(r => r.AttrType == attrType);
            IQueryable<TAttribute> attrs = this.m_dataSource.GetQueryable<TAttribute>();
            var result = records.Join(attrs, r => r.AttrId, a => a.Id, (r, a) => new
            {
                r.ObjectId,
                AttrId = a.Id,
                AttrName = a.Value,
            }).Where(x => x.AttrName.Length != 0);
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
                        IQueryable<long> ids = result.Where(x => x.AttrName.Contains(first) || x.AttrName == first).Select(x => x.ObjectId);
                        while (iterator.MoveNext())
                        {
                            string _name = (string)iterator.Current;
                            Debug.Assert(_name.Length != 0);
                            var temp = result.Where(x => x.AttrName.Contains(_name) || x.AttrName == _name);
                            ids = ids.Concat(result.Where(x => x.AttrName.Contains(_name) || x.AttrName == _name).Select(x => x.ObjectId));
                        }
                        return ids;
                    }
                    case CompareType.IncludedByAnyOf:
                    {
                        var iterator = names.GetEnumerator();
                        iterator.MoveNext();
                        var first = (string)iterator.Current;
                        IQueryable<long> ids = result.Where(x => first.Contains(x.AttrName) || first == x.AttrName).Select(x => x.ObjectId);
                        while (iterator.MoveNext())
                        {
                            string _name = (string)iterator.Current;
                            var temp = result.Where(x => _name.Contains(x.AttrName) || _name == x.AttrName);
                            ids = ids.Concat(result.Where(x => _name.Contains(x.AttrName) || _name == x.AttrName).Select(x => x.ObjectId));
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
                            .Where(x => x.AttrName.Contains(name) || x.AttrName == name)
                            .Select(x => x.ObjectId);
                    }
                    case CompareType.IncludedByAnyOf:
                    {
                        return result
                            .Where(x => name.Contains(x.AttrName) || name == x.AttrName)
                            .Select(x => x.ObjectId);
                    }
                }
            }
            return result.Select(x => x.ObjectId);
        }

        BasicDetails? Impl_GetBasicDetails<TObject, TRecord>(long objectId)
            where TObject : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
        {
            if (objectId == 0) return null;
            TObject? @object = this.m_dataSource.GetQueryable<TObject>().FirstOrDefault(i => i.Id == objectId);
            if (@object == null) return null;
            return new BasicDetails
            {
                Id = @object.Id,
                Name = @object.Name,
                Description = @object.Description,
            };
        }

        IQueryable<long> m_GetAllAttributesQueryable<TObj, TRecord>(QueryModel? queryModel)
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
                            var targetAttrType = AttributeType.GetAttributeType(cond.Type);

                            if (targetAttrType != null)
                            {
                                meta = this.GetIdsByAttributeExpressionData(targetAttrType, exprData, str);
                            }
                        }
                        if (meta != null)
                        {
                            result = result != null
                                ? exprData.LinkType == LinkType.And ? result.Intersect(meta) : result.Concat(meta)
                                : meta;
                        }
                    }
                }
                return result ?? this.m_dataSource.GetQueryable<TObj>().Select(x => x.Id);
            }
        }

        IQueryable<long> Impl_GetQueryableIdsByQueryModel<TObject, TRecord>(QueryModel? queryModel)
            where TObject : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
        {
            return this.m_GetAllAttributesQueryable<TObject, TRecord>(queryModel);
        }

        public abstract Type ObjectType { get; }
        public abstract Type RecordType { get; }

        protected IDataSource m_dataSource;

        protected CommonObjectServiceBase(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
        }

        public virtual long SetAttribute<TAttribute>(long objectId, string attrValue, bool _delete)
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            if (objectId == 0) return 0;
            return (long)CachedMethods.FindAndExecuteByName(
                this, "Impl_SetObjectAttributeData",
                [this.ObjectType, this.RecordType, typeof(TAttribute)],
                objectId, attrValue, _delete)!;
        }

        public virtual IEnumerable<BasicInfo> GetAttributes<TAttribute>(long objectId)
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            if (objectId == 0) return Enumerable.Empty<BasicInfo>();
            return (IEnumerable<BasicInfo>)CachedMethods.FindAndExecuteByName(
                this, "Impl_GetObjectAttributeInformations",
                [this.RecordType, typeof(TAttribute)],
                objectId)!;
        }

        public virtual long Rename(long objectId, string newName)
        {
            if (objectId == 0) return 0;
            return (long)CachedMethods.FindAndExecuteByName(
                this, "Impl_RenameObject",
                [this.ObjectType],
                objectId, newName)!;
        }

        public virtual long Remove(long objectId)
        {
            if (objectId == 0) return 0;
            return (long)CachedMethods.FindAndExecuteByName(
                this, "Impl_RemoveObject",
                [this.ObjectType],
                objectId)!;
        }

        public virtual IQueryable<long> GetIdsByAttributeName<TAttribute>(IEnumerable<ExpressionData> exprDatas)
             where TAttribute : class, IBasicEntityAttribute, new()
        {
            return (IQueryable<long>)CachedMethods.FindAndExecuteByName(
                this, "Impl_GetObjectIdsByAttributeName",
                [this.RecordType, typeof(TAttribute)],
                exprDatas)!;
        }

        public virtual IQueryable<long>? GetIdsByAttributeExpressionData(Type attrType, ExpressionData expressionData, string str)
        {
            return (IQueryable<long>?)CachedMethods.FindAndExecuteByName(
                this, "Impl_GetObjectIdsByAttributeExpressionData",
                [this.RecordType, attrType],
                expressionData, str);
        }

        public virtual IEnumerable<long> GetIdsByAttributeName<TAttribute>(IEnumerable<ExpressionData> exprDatas, DataRange? range)
            where TAttribute : class, IBasicEntityAttribute, new()
        {
            IQueryable<long> ids = this.GetIdsByAttributeName<TAttribute>(exprDatas);
            return range != null ? ids.Skip(range.Begin).Take(range.Count).ToList() : (IEnumerable<long>)ids.ToList();
        }

        public virtual BasicDetails? GetBasicDetails(long objectId)
        {
            if (objectId == 0) return null;
            return (BasicDetails?)CachedMethods.FindAndExecuteByName(
               this, "Impl_GetBasicDetails",
               [this.ObjectType, this.RecordType],
               objectId);
        }

        public virtual IQueryable<long> GetQueryableIdsByQueryModel(QueryModel? queryModel)
        {
            return (IQueryable<long>)CachedMethods.FindAndExecuteByName(
               this, "Impl_GetQueryableIdsByQueryModel",
               [this.ObjectType, this.RecordType],
               queryModel)!;
        }
    }
}
