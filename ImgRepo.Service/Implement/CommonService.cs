using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.Interface;

namespace ImgRepo.Service.Implement
{
    internal class CommonService
    {
        protected IDataSource m_dataSource;
        protected IQueryable<TagInformation> m_tags;
        protected IDataWriter<TagInformation> m_tagWriter;
        protected IQueryable<CategoryInformation> m_categories;
        protected IDataWriter<CategoryInformation> m_categoryWriter;

        protected CommonService(IDataSource dataSource)
        {
            m_dataSource = dataSource;
            m_tags = dataSource.GetQueryable<TagInformation>();
            m_tagWriter = dataSource.GetWriter<TagInformation>();
            m_categories = dataSource.GetQueryable<CategoryInformation>();
            m_categoryWriter = dataSource.GetWriter<CategoryInformation>();
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

        protected IEnumerable<BasicInfo> getBasicInfo<TRecord, TAttr>(long objectId, long attrType) where TRecord : class, IBasicEntityRecord, new() where TAttr : class, IBasicEntityInformation, new()
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
    }
}
