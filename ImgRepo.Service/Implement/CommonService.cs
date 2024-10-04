using Cyh.Net;
using Cyh.Net.Data;
using Cyh.Net.Data.Predicate;
using ImgRepo.Data.Enums;
using ImgRepo.Data.Interface;
using ImgRepo.Model;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Artist;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.MetaResult;
using ImgRepo.Model.Query;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace ImgRepo.Service.Implement
{
    internal class CommonService
    {
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


        protected IDataSource m_dataSource;

        protected CommonService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
        }

        protected long setObjectAttrData<TRecord, TAttr>(long objectId, long attrType, string attrName, bool _delete) where TRecord : class, IBasicEntityRecord, new() where TAttr : class, IBasicEntityInformation, new()
        {
            // no image
            if (objectId == 0) return 0;

            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IDataWriter<TAttr> attrWriter = this.m_dataSource.GetWriter<TAttr>();
            IDataWriter<TRecord> recordWriter = this.m_dataSource.GetWriter<TRecord>();

            TAttr? attr = attrs.FirstOrDefault(a => a.Name == attrName);

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
                        Name = attrName,
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

        protected long setObjectAttr<TObj, TRecord, TAttr>(long objectId, long attrType, string attrName, bool _delete)
            where TObj : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityInformation, new()
        {
            if (objectId == 0) return 0;
            IQueryable<TObj> objs = this.m_dataSource.GetQueryable<TObj>();
            if (!objs.Any(o => o.Id == objectId)) return 0;
            return this.setObjectAttrData<TRecord, TAttr>(objectId, attrType, attrName, _delete);
        }

        protected IEnumerable<BasicInfo> getObjectAttrInfos<TRecord, TAttr>(long objectId, long attrType) where TRecord : class, IBasicEntityRecord, new() where TAttr : class, IBasicEntityInformation, new()
        {
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            return from r in records
                   join a in attrs on r.AttrId equals a.Id
                   where r.ObjectId == objectId && r.AttrType == attrType
                   select new BasicInfo
                   {
                       Id = a.Id,
                       Name = a.Name,
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
            where TAttr : class, IBasicEntityInformation, new()
        {
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TAttr> attrs = this.m_dataSource.GetQueryable<TAttr>();
            var metaQueryable = records
                .Join(attrs,
                    r => r.AttrId,
                    a => a.Id,
                    (r, a) => new MetaAttributeNameResult
                    {
                        ObjectId = r.ObjectId,
                        AttrType = r.AttrType,
                        Value = a.Name
                    }).Where(r => r.AttrType == attrType);

            if (exprDatas.IsNullOrEmpty())
                return metaQueryable.Select(r => r.ObjectId);

            foreach (var exprData in exprDatas)
            {
                exprData.MemberName = "Value";
            }

            var predicate = Predicate.GetExpression<MetaAttributeNameResult>(exprDatas);

            if (predicate == null)
                return metaQueryable.Select(r => r.ObjectId);

            return metaQueryable.Where(predicate).Select(r => r.ObjectId);
        }

        protected IQueryable<long> queryAllAttributes<TObj, TRecord>(IEnumerable<ExpressionData> expressionDatas)
            where TObj : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
        {
            IQueryable<TRecord> records = this.m_dataSource.GetQueryable<TRecord>();
            IQueryable<TagInformation> tags = this.m_dataSource.GetQueryable<TagInformation>();
            IQueryable<CategoryInformation> categories = this.m_dataSource.GetQueryable<CategoryInformation>();
            IQueryable<WebsiteInformation> websites = this.m_dataSource.GetQueryable<WebsiteInformation>();
            IQueryable<TObj> objs = this.m_dataSource.GetQueryable<TObj>();
            var rObjs = records
                .Join(objs,
                r => r.ObjectId,
                o => o.Id, (r, o) => new
                {
                    Rid = r.Id,
                    Oid = o.Id,
                    OName = o.Name,
                    AttrId = r.AttrId,
                    AttrType = r.AttrType,
                });

            var rTags = rObjs.Where(r => r.AttrType == AttributeType.Tag).Join(tags, r => r.AttrId, t => t.Id, (r, t) => new
            {
                Oid = r.Oid,
                TName = t.Name,
            });
            var rCats = rObjs.Where(r => r.AttrType == AttributeType.Category).Join(categories, c => c.AttrId, cat => cat.Id, (c, cat) => new
            {
                Oid = c.Oid,
                CName = cat.Name,
            });
            var rWebs = rObjs.Where(r => r.AttrType == AttributeType.Website).Join(websites, w => w.AttrId, web => web.Id, (w, web) => new
            {
                Oid = w.Oid,
                WName = web.Name,
            });

            IQueryable<MetaAllAttributeName> rAll = rObjs
                .GroupJoin(rTags, o => o.Oid, t => t.Oid, (o, t) => new
                {
                    Oid = o.Oid,
                    Name = o.OName,
                    Tags = t
                })
                .SelectMany(xy => xy.Tags.DefaultIfEmpty(), (x, y) => new
                {
                    Oid = x.Oid,
                    Name = x.Name,
                    Tag = y,
                })
                .GroupJoin(rCats, o => o.Oid, c => c.Oid, (o, c) => new
                {
                    Oid = o.Oid,
                    Name = o.Name,
                    Tag = o.Tag,
                    Cats = c,
                })
                .SelectMany(xy => xy.Cats.DefaultIfEmpty(), (x, y) => new
                {
                    Oid = x.Oid,
                    Name = x.Name,
                    Tag = x.Tag,
                    Cat = y
                })
                .GroupJoin(rWebs, o => o.Oid, w => w.Oid, (o, w) => new
                {
                    Oid = o.Oid,
                    Name = o.Name,
                    Tag = o.Tag,
                    Cat = o.Cat,
                    Webs = w,
                })
                .SelectMany(xy => xy.Webs.DefaultIfEmpty(), (x, y) => new MetaAllAttributeName
                {
                    ObjectId = x.Oid,
                    Name = x.Name,
                    Tag = x.Tag.TName,
                    Category = x.Cat.CName,
                    Website = y.WName,
                });

            return rAll.Where(Predicate.GetExpression<MetaAllAttributeName>(expressionDatas)).Select(x => x.ObjectId);
        }

        protected IQueryable<long> getObjectIdsByAttributeExprData<TRecord, TAttr>(long attrType, ExpressionData expressionData, string str)
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityInformation, new()
        {
            Debug.Assert(!str.IsNullOrEmpty());

            var records = this.m_dataSource.GetQueryable<TRecord>().Where(r => r.AttrType == attrType);
            var attrs = this.m_dataSource.GetQueryable<TAttr>();
            var result = records.Join(attrs, r => r.AttrId, a => a.Id, (r, a) => new
            {
                ObjectId = r.ObjectId,
                AttrId = a.Id,
                AttrName = a.Name,
            });
            var originalGroups = result.GroupBy(x => x.ObjectId);
            string name = str;
            string[] names = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
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
                var conds = queryModel.Conditions;
                Dictionary<string, ExpressionData> conditions = new();
                foreach (var cond in conds)
                {
                    if (cond.TryGetExpressionData(out ExpressionData? exprData))
                    {
                        conditions.Add(exprData.MemberName, exprData);
                        if (exprData.ConstantValue is not string str) continue;
                        IQueryable<long>? meta = null;
                        if (exprData.MemberName == "Name")
                        {
                            switch (exprData.CompareType)
                            {
                                case CompareType.Contains:
                                {
                                    if (str.Contains(','))
                                    {
                                        var strs = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                        IPredicateHolder<TObj>? predicate = null;
                                        foreach (var s in strs)
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
                                        exprData.ConstantValue = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
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
                        else if (exprData.MemberName == "Tag")
                        {
                            meta = this.getObjectIdsByAttributeExprData<TRecord, TagInformation>(AttributeType.Tag, exprData, str);
                        }
                        else if (exprData.MemberName == "Category")
                        {
                            meta = this.getObjectIdsByAttributeExprData<TRecord, CategoryInformation>(AttributeType.Category, exprData, str);
                        }
                        else if (exprData.MemberName == "Website")
                        {
                            meta = this.getObjectIdsByAttributeExprData<TRecord, WebsiteInformation>(AttributeType.Website, exprData, str);
                        }
                        else if (exprData.MemberName == "Artist")
                        {
                            meta = this.getObjectIdsByAttributeExprData<TRecord, ArtistInformation>(0, exprData, str);
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
