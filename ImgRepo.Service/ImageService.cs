using Cyh.Net;
using Cyh.Net.Data;
using Cyh.Net.Data.Predicate;
using ImgRepo.Model.ApiModel;
using ImgRepo.Model.Entities;
using ImgRepo.Model.Interface;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Dto;
using AttrTypeEnum = ImgRepo.Model.Enums.AttributeType;

namespace ImgRepo.Service
{
    class ImageService : IImageService
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
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
        class MetaImageFile
        {
            public long ImageId { get; set; }
            public long FileId { get; set; }
            public string Format { get; set; }
            public byte[]? Thumbnail { get; set; }
            public byte[]? Data { get; set; }
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        readonly IDataSource m_dataSource;
        readonly IQueryable<ImageRecord> m_imageRecords;
        readonly IQueryable<AlbumRecord> m_albumRecords;
        readonly IQueryable<AlbumInformation> m_albumInformations;
        readonly IQueryable<ImageInformation> m_imageInformations;
        readonly IQueryable<TagInformation> m_tagInformations;
        readonly IQueryable<CategoryInformation> m_categoryInformations;
        readonly IQueryable<ImageFileData> m_imageFileData;


        IQueryable<BasicDetails> _tagDetails => this.m_tagInformations.Select(BasicDetails.GetExpression<TagInformation>());

        IQueryable<BasicDetails> _catDetails => this.m_categoryInformations.Select(BasicDetails.GetExpression<CategoryInformation>());

        IQueryable<MetaImageFile> _imageFileDatas => this.m_imageInformations.Join(this.m_imageFileData, i => i.FileId, f => f.Id, (i, f) => new MetaImageFile
        {
            ImageId = i.Id,
            FileId = i.FileId,
            Format = f.Format,
            Thumbnail = f.Thumbnail,
            Data = f.Data,
        });

        IQueryable<MetaRecord> GetImageMetaQueryable()
        {
            var dataTags = this.m_imageInformations
                .Join(this.m_imageRecords.Where(r => r.AttrType == AttrTypeEnum.Tag),
                i => i.Id, r => r.ObjectId, (i, r) => new
                {
                    ImgId = i.Id,
                    ImgName = i.Name,
                    TagId = r.AttrId,
                }).Join(this.m_tagInformations, ir => ir.TagId, t => t.Id, (ir, t) => new
                {
                    ImgId = ir.ImgId,
                    TagName = t.Name,
                });
            var dataCats = this.m_imageInformations
                .Join(this.m_imageRecords.Where(r => r.AttrType == AttrTypeEnum.Category),
                i => i.Id, r => r.ObjectId, (i, r) => new
                {
                    ImgId = i.Id,
                    ImgName = i.Name,
                    CatId = r.AttrId,
                }).Join(this.m_categoryInformations, ir => ir.CatId, c => c.Id, (ir, c) => new
                {
                    ImgId = ir.ImgId,
                    CatName = c.Name,
                });

            return dataTags.Join(dataCats, dt => dt.ImgId, dc => dc.ImgId, (dt, dc) => new
            {
                ImgId = dt.ImgId,
                TagName = dt.TagName,
                CatName = dc.CatName,
            }).Join(this.m_imageInformations, d => d.ImgId, i => i.Id, (d, i) => new MetaRecord
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

        static IEnumerable<BasicInfo> GetRelatedAttributeInfo<R, T>(IQueryable<R> recQueryable, IQueryable<T> attrQueryable, long objId, long attrType) where R : IBasicEntityRecord where T : IBasicEntityInformation
        {
            return recQueryable.Where(r => r.ObjectId == objId && r.AttrType == attrType)
                .Join(attrQueryable, r => r.AttrId, a => a.Id, (r, a) => new BasicInfo
                {
                    Id = r.ObjectId,
                    Name = a.Name,
                }).Distinct().ToList();
        }

        static IEnumerable<BasicInfo> GetTagInfos<R>(IQueryable<R> recQueryable, IQueryable<TagInformation> tagInformation, long objId) where R : IBasicEntityRecord
        {
            return GetRelatedAttributeInfo(recQueryable, tagInformation, objId, AttrTypeEnum.Tag);
        }

        static IEnumerable<BasicInfo> GetCatInfos<R>(IQueryable<R> recQueryable, IQueryable<CategoryInformation> categoryInformation, long objId) where R : IBasicEntityRecord
        {
            return GetRelatedAttributeInfo(recQueryable, categoryInformation, objId, AttrTypeEnum.Category);
        }

        public ImageService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;
            this.m_imageRecords = dataSource.GetQueryable<ImageRecord>();
            this.m_albumRecords = dataSource.GetQueryable<AlbumRecord>();
            this.m_imageInformations = dataSource.GetQueryable<ImageInformation>();
            this.m_tagInformations = dataSource.GetQueryable<TagInformation>();
            this.m_categoryInformations = dataSource.GetQueryable<CategoryInformation>();
            this.m_albumInformations = dataSource.GetQueryable<AlbumInformation>();
            this.m_imageFileData = dataSource.GetQueryable<ImageFileData>();
        }

        public BasicDetails? GetAlbumDetails(long id)
        {
            AlbumInformation? album = this.m_albumInformations.FirstOrDefault(x => x.Id == id);
            if (album == null) return null;

            IQueryable<AlbumRecord> records = this.m_albumRecords.Where(r => r.ObjectId == id);

            return new BasicDetails
            {
                Id = id,
                Name = album.Name,
                Description = album.Description,
            };
        }

        public BasicDetails? GetImageDetails(long id)
        {
            ImageInformation? img = this.m_imageInformations.FirstOrDefault(x => x.Id == id);
            if (img == null) return null;


            return new BasicDetails
            {
                Id = id,
                Name = img.Name,
                Description = img.Description,
            };
        }


        public BasicDetails? GetTagDetails(long id)
        {
            return this._tagDetails.FirstOrDefault(t => t.Id == id);
        }

        public BasicDetails? GetCategoryDetails(long id)
        {
            return this._catDetails.FirstOrDefault(c => c.Id == id);
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

            return this.GetImageMetaQueryable().Where(holder.GetPredicate()).Select(x => new ImageThumbView
            {
                Id = x.ImgId,
                Name = x.ImgName,
                FileId = x.FileId,
            });
        }

        public ApiFileModel? GetFileBytes(long fileId)
        {
            ImageFileData? fileData = this.m_imageFileData.FirstOrDefault(f => f.Id == fileId);
            if (fileData == null || fileData.Data == null || fileData.Thumbnail == null)
            {
                return null;
            }
            return new ApiFileModel
            {
                Format = fileData.Format,
                Base64 = Convert.ToBase64String(fileData.Data),
            };
        }

        public ApiFileModel? GetFullImage(long imgId)
        {
            MetaImageFile? fileData = this._imageFileDatas.FirstOrDefault(f => f.ImageId == imgId);
            if (fileData == null || fileData.Data == null)
            {
                return null;
            }
            return new ApiFileModel
            {
                Format = fileData.Format,
                Base64 = Convert.ToBase64String(fileData.Data),
            };
        }

        public ApiFileModel? GetThumbnail(long imgId)
        {
            MetaImageFile? fileData = this._imageFileDatas.FirstOrDefault(f => f.ImageId == imgId);
            if (fileData == null || fileData.Thumbnail == null)
            {
                return null;
            }
            return new ApiFileModel
            {
                Format = fileData.Format,
                Base64 = Convert.ToBase64String(fileData.Thumbnail),
            };
        }

        public IEnumerable<BasicInfo> GetTags(long imgId)
        {
            return GetTagInfos(this.m_imageRecords, m_tagInformations, imgId);
        }
        public IEnumerable<BasicInfo> GetCategories(long imgId)
        {
            return GetCatInfos(this.m_imageRecords, m_categoryInformations, imgId);
        }

        public long UpdateImageTag(long imageId, string tagName, bool _delete)
        {
            ImageInformation? imgInfo = this.m_imageInformations.FirstOrDefault(i => i.Id == imageId);
            if (imgInfo == null) return 0;

            TagInformation? tagInfo = this.m_tagInformations.FirstOrDefault(t => t.Name == tagName);
            if (_delete)
            {
                if (tagInfo == null) return 0;
                ImageRecord? record = this.m_imageRecords.Where(r => r.AttrType == AttrTypeEnum.Tag && r.AttrId == tagInfo.Id).FirstOrDefault(r => r.ObjectId == imageId);
                if (record != null)
                {
                    this.m_dataSource.GetWriter<ImageRecord>().Remove(record);
                    this.m_dataSource.Save();
                }
                return 0;
            }
            else
            {
                if (tagInfo == null)
                {
                    tagInfo = new TagInformation { Name = tagName };
                    this.m_dataSource.GetWriter<TagInformation>().Add(tagInfo);
                    this.m_dataSource.Save();
                }
                ImageRecord? record = this.m_imageRecords.Where(r => r.AttrType == AttrTypeEnum.Tag && r.AttrId == tagInfo.Id).FirstOrDefault(r => r.ObjectId == imageId);
                if (record != null)
                {
                    return record.AttrId;
                }
                this.m_dataSource.GetWriter<ImageRecord>().Add(new ImageRecord
                {
                    ObjectId = imageId,
                    AttrType = AttrTypeEnum.Tag,
                    AttrId = tagInfo.Id,
                });
                this.m_dataSource.Save();
                return tagInfo.Id;
            }
        }

        public long GetImageFileId(long imageId)
        {
            return this.m_imageInformations.FirstOrDefault(i => i.Id == imageId)?.FileId ?? 0;
        }

        public long UploadImage(NewImageDto? imageDto)
        {
            if (imageDto == null) return 0;
            ImageFileData fileData = new ImageFileData
            {
                Format = imageDto.Format,
                Data = imageDto.Data,
                FileName = imageDto.FileName,
                Thumbnail = imageDto.ThumbData,
            };
            this.m_dataSource.GetWriter<ImageFileData>().Add(fileData);
            this.m_dataSource.Save();
            ImageInformation imgInfo = new ImageInformation
            {
                Name = imageDto.ImageName,
                Description = imageDto.Description,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                FileId = fileData.Id,
            };
            this.m_dataSource.GetWriter<ImageInformation>().Add(imgInfo);
            this.m_dataSource.Save();
            if (imgInfo.Id != 0)
            {
                foreach(var tag in imageDto.Tags)
                {
                    this.UpdateImageTag(imgInfo.Id, tag, false);
                }
            }
            return imgInfo.Id;
        }

    }
}
