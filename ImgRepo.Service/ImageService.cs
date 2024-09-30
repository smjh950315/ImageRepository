using Cyh.Net;
using Cyh.Net.Data;
using Cyh.Net.Data.Predicate;
using ImgRepo.Model.Entities;
using ImgRepo.Model.ViewModel;

namespace ImgRepo.Service
{
    class ImageService : IImageService
    {
        class ObjTypeEnum
        {
            public const long Image = 1;
            public const long Album = 2;
        }
        class AttTypeEnum
        {
            public const long Tag = 1;
            public const long Category = 2;
        }
        class MetaRecord
        {
            public long ImgId { get; set; }
            public string ImgName { get; set; }
            public string TagName { get; set; }
            public string CatName { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
            public long FileId { get; set; }
        }

        IDataSource m_dataSource;
        IQueryable<BindingRecord> m_bindingRecords;
        IQueryable<AlbumInformation> m_albumInformations;
        IQueryable<ImageInformation> m_imageInformations;
        IQueryable<TagInformation> m_tagInformations;
        IQueryable<CategoryInformation> m_categoryInformations;
        IQueryable<ImageFileData> m_imageFileData;

        IQueryable<BindingRecord> _imageRecordQueryable => this.m_bindingRecords.Where(r => r.ObjType == ObjTypeEnum.Image);
        IQueryable<BindingRecord> _albumRecordQueryable => this.m_bindingRecords.Where(r => r.ObjType == ObjTypeEnum.Album);
        IQueryable<BasicDetails> _tagDetails => this.m_tagInformations.Select(BasicDetails.GetExpression<TagInformation>());
        IQueryable<BasicDetails> _catDetails => this.m_categoryInformations.Select(BasicDetails.GetExpression<CategoryInformation>());

        IQueryable<MetaRecord> GetMetaQueryable()
        {
            var dataTags = this.m_imageInformations
                .Join(_imageRecordQueryable.Where(r => r.AttType == AttTypeEnum.Tag),
                i => i.Id, r => r.ObjId, (i, r) => new
                {
                    ImgId = i.Id,
                    ImgName = i.Name,
                    TagId = r.AttId,
                }).Join(m_tagInformations, ir => ir.TagId, t => t.Id, (ir, t) => new
                {
                    ImgId = ir.ImgId,
                    TagName = t.Name,
                });
            var dataCats = this.m_imageInformations
                .Join(_imageRecordQueryable.Where(r => r.AttType == AttTypeEnum.Category),
                i => i.Id, r => r.ObjId, (i, r) => new
                {
                    ImgId = i.Id,
                    ImgName = i.Name,
                    CatId = r.AttId,
                }).Join(m_categoryInformations, ir => ir.CatId, c => c.Id, (ir, c) => new
                {
                    ImgId = ir.ImgId,
                    CatName = c.Name,
                });

            return dataTags.Join(dataCats, dt => dt.ImgId, dc => dc.ImgId, (dt, dc) => new
            {
                ImgId = dt.ImgId,
                TagName = dt.TagName,
                CatName = dc.CatName,
            }).Join(m_imageInformations, d => d.ImgId, i => i.Id, (d, i) => new MetaRecord
            {
                ImgId = d.ImgId,
                ImgName = i.Name,
                TagName = d.TagName,
                CatName = d.CatName,
                Created = i.Created,
                Updated = i.Updated,
                FileId = i.FileId,
            });
        }

        IEnumerable<string> GetTagNames(long objType, long objId)
        {
            return this.m_bindingRecords.Where(r => r.ObjId == objId && r.ObjType == objType && r.AttType == AttTypeEnum.Tag).Join(this.m_tagInformations, r => r.AttId, t => t.Id, (r, t) => t.Name).Distinct().ToList();
        }
        IEnumerable<string> GetCatNames(long objType, long objId)
        {
            return this.m_bindingRecords.Where(r => r.ObjId == objId && r.ObjType == objType && r.AttType == AttTypeEnum.Category).Join(this.m_categoryInformations, r => r.AttId, c => c.Id, (r, c) => c.Name).Distinct().ToList();
        }

        public ImageService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
            this.m_bindingRecords = dataSource.GetQueryable<BindingRecord>();
            this.m_imageInformations = dataSource.GetQueryable<ImageInformation>();
            this.m_tagInformations = dataSource.GetQueryable<TagInformation>();
            this.m_categoryInformations = dataSource.GetQueryable<CategoryInformation>();
            this.m_albumInformations = dataSource.GetQueryable<AlbumInformation>();
            this.m_imageFileData = dataSource.GetQueryable<ImageFileData>();
        }

        public ArtworkDetails? GetAlbumDetails(long id)
        {
            var album = this.m_albumInformations.FirstOrDefault(x => x.Id == id);
            if (album == null) return null;

            IQueryable<BindingRecord> records = this._albumRecordQueryable.Where(r => r.ObjId == id);

            return new ArtworkDetails
            {
                Id = id,
                Name = album.Name,
                Description = album.Description,
                Tags = this.GetTagNames(ObjTypeEnum.Image, id),
                Categories = this.GetCatNames(ObjTypeEnum.Image, id),
            };
        }

        public BasicDetails? GetCategoryDetails(long id)
        {
            return this._catDetails.FirstOrDefault(c => c.Id == id);
        }

        public byte[] GetFileBytes(long fileId)
        {
            return this.m_imageFileData.FirstOrDefault(f => f.Id == fileId)?.Data ?? new byte[0];
        }

        public ArtworkDetails? GetImageDetails(long id)
        {
            ImageInformation? img = this.m_imageInformations.FirstOrDefault(x => x.Id == id);
            if (img == null) return null;
            IQueryable<BindingRecord> records = this._imageRecordQueryable.Where(r => r.ObjId == id);

            return new ArtworkDetails
            {
                Id = id,
                Name = img.Name,
                Description = img.Description,
                Tags = this.GetTagNames(ObjTypeEnum.Image, id),
                Categories = this.GetCatNames(ObjTypeEnum.Image, id),
            };
        }

        public IEnumerable<ImageThumbView> GetImageThumbViews(QueryModel? queryModel)
        {
            IPredicateHolder<MetaRecord> holder = Predicate.NewPredicateHolder<MetaRecord>(x => true);
            if (queryModel != null)
            {
                if (!queryModel.Name.IsNullOrEmpty())
                {
                    holder.And(x => x.ImgName.Contains(queryModel.Name));
                }
                if (!queryModel.Tags.IsNullOrEmpty())
                {
                    holder.And(x => queryModel.Tags.Contains(x.TagName));
                }
                if (!queryModel.Categories.IsNullOrEmpty())
                {
                    holder.And(x => queryModel.Categories.Contains(x.CatName));
                }
                if (queryModel.CreateBegin.HasValue)
                {
                    holder.And(x => x.Created >= queryModel.CreateBegin.Value);
                }
                if (queryModel.CreateEnd.HasValue)
                {
                    holder.And(x => x.Created <= queryModel.CreateEnd.Value);
                }
                if (queryModel.UpdateBegin.HasValue)
                {
                    holder.And(x => x.Updated >= queryModel.UpdateBegin.Value);
                }
                if (queryModel.UpdateEnd.HasValue)
                {
                    holder.And(x => x.Updated <= queryModel.UpdateEnd.Value);
                }
            }

            return this.GetMetaQueryable().Where(holder.GetPredicate()).Select(x => new ImageThumbView
            {
                Id = x.ImgId,
                Name = x.ImgName,
                FileId = x.FileId,
            });
        }

        public BasicDetails? GetTagDetails(long id)
        {
            return this._tagDetails.FirstOrDefault(t => t.Id == id);
        }

        public void UpdateImageTag(long imageId, string tagName, bool _delete)
        {
            var imgInfo = this.m_imageInformations.FirstOrDefault(i => i.Id == imageId);
            if (imgInfo == null) return;

            var tagInfo = this.m_tagInformations.FirstOrDefault(t => t.Name == tagName);
            if (_delete)
            {
                if (tagInfo == null) return;
                var record = this._imageRecordQueryable.Where(r => r.AttType == AttTypeEnum.Tag && r.AttId == tagInfo.Id).FirstOrDefault(r => r.ObjId == imageId);
                if (record != null)
                {
                    this.m_dataSource.GetWriter<BindingRecord>().Remove(record);
                    this.m_dataSource.Save();
                }
            }
            else
            {
                if (tagInfo == null)
                {
                    tagInfo = new TagInformation { Name = tagName };
                    this.m_dataSource.GetWriter<TagInformation>().Add(tagInfo);
                    this.m_dataSource.Save();
                }
                var record = this._imageRecordQueryable.Where(r => r.AttType == AttTypeEnum.Tag && r.AttId == tagInfo.Id).FirstOrDefault(r => r.ObjId == imageId);
                if (record != null)
                {
                    return;
                }
                this.m_dataSource.GetWriter<BindingRecord>().Add(new BindingRecord
                {
                    ObjType = ObjTypeEnum.Image,
                    ObjId = imageId,
                    AttType = AttTypeEnum.Tag,
                    AttId = tagInfo.Id,
                });
                this.m_dataSource.Save();
            }
        }

        public long GetImageFileId(long imageId)
        {
            return this.m_imageInformations.FirstOrDefault(i => i.Id == imageId)?.FileId ?? 0;
        }

        public void SaveImageFile(BasicDetails? imgDetails, byte[]? data)
        {
            if(imgDetails == null || data==null || data.Length==0) return;
            var newFileData=new ImageFileData
            {
                Data = data
            };
            m_dataSource.GetWriter<ImageFileData>().Add(newFileData);
            m_dataSource.Save();
            m_dataSource.GetWriter<ImageInformation>().Add(new ImageInformation
            {
                Name = imgDetails.Name,
                Description = imgDetails.Description,
                FileId = newFileData.Id,
            });
            m_dataSource.Save();
        }
    }
}
