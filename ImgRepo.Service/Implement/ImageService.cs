using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.ApiModel;
using ImgRepo.Model.Entities;
using ImgRepo.Model.Enums;
using ImgRepo.Model.Interface;
using ImgRepo.Model.ViewModel;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class ImageService : IImageService
    {
        class MetaImageFileBinding
        {
            public long ImageId { get; set; }
            public string ImageName { get; set; }
            public long FileId { get; set; }
            public string FileName { get; set; }
            public string Format { get; set; }
            public byte[] Data { get; set; }
            public byte[] Thumbnail { get; set; }
        }

        IDataSource m_dataSource;
        IQueryable<ImageInformation> m_images;
        IDataWriter<ImageInformation> m_imageWriter;
        IQueryable<TagInformation> m_tags;
        IDataWriter<TagInformation> m_tagWriter;
        IQueryable<CategoryInformation> m_categories;
        IDataWriter<CategoryInformation> m_categoryWriter;
        IQueryable<ArtistInformation> m_artists;
        IDataWriter<ArtistInformation> m_artistWriter;

        IQueryable<ImageRecord> m_imageRecords;
        IDataWriter<ImageRecord> m_imageRecordWriter;

        IQueryable<ImageFileData> m_imagefiles;
        IDataWriter<ImageFileData> m_imageFileWriter;

        IQueryable<ImageRecord> ImageTagRecordQueryable => this.m_imageRecords.Where(r => r.AttrType == AttributeType.Tag);
        IQueryable<ImageRecord> ImageCatRecordQueryable => this.m_imageRecords.Where(r => r.AttrType == AttributeType.Category);

        long setObjectAttrData<TRecord, TAttr>(long objectId, long attrType, string attrName, bool _delete) where TRecord : class, IBasicEntityRecord, new() where TAttr : class, IBasicEntityInformation, new()
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
        long setObjectAttr<TObj, TRecord, TAttr>(long objectId, long attrType, string attrName, bool _delete)
            where TObj : class, IBasicEntityInformation, new()
            where TRecord : class, IBasicEntityRecord, new()
            where TAttr : class, IBasicEntityInformation, new()
        {
            if (objectId == 0) return 0;
            IQueryable<TObj> objs = this.m_dataSource.GetQueryable<TObj>();
            if (!objs.Any(o => o.Id == objectId)) return 0;
            return this.setObjectAttrData<TRecord, TAttr>(objectId, attrType, attrName, _delete);
        }

        IEnumerable<BasicInfo> getBasicInfo<TRecord, TAttr>(long objectId, long attrType) where TRecord : class, IBasicEntityRecord, new() where TAttr : class, IBasicEntityInformation, new()
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

        IQueryable<MetaImageFileBinding> ImageFiles => this.m_images.Join(this.m_imagefiles, i => i.FileId, f => f.Id, (i, f) => new MetaImageFileBinding
        {
            ImageId = i.Id,
            ImageName = i.Name,
            FileId = f.Id,
            FileName = f.FileName,
            Format = f.Format,
            Data = f.Data,
            Thumbnail = f.Thumbnail,
        });

        public ImageService(IDataSource dataSource)
        {
            this.m_dataSource = dataSource;

            this.m_images = dataSource.GetQueryable<ImageInformation>();
            this.m_imageWriter = dataSource.GetWriter<ImageInformation>();
            this.m_tags = dataSource.GetQueryable<TagInformation>();
            this.m_tagWriter = dataSource.GetWriter<TagInformation>();
            this.m_categories = dataSource.GetQueryable<CategoryInformation>();
            this.m_categoryWriter = dataSource.GetWriter<CategoryInformation>();
            this.m_artists = dataSource.GetQueryable<ArtistInformation>();
            this.m_artistWriter = dataSource.GetWriter<ArtistInformation>();

            this.m_imageRecords = dataSource.GetQueryable<ImageRecord>();
            this.m_imageRecordWriter = dataSource.GetWriter<ImageRecord>();

            this.m_imagefiles = dataSource.GetQueryable<ImageFileData>();
            this.m_imageFileWriter = dataSource.GetWriter<ImageFileData>();
        }

        public long CreateImage(NewImageDto? imageDto)
        {
            if (imageDto == null)
            {
                return 0;
            }
            this.m_dataSource.BeginTransaction();
            ImageFileData imageFileData = new ImageFileData
            {
                FileName = imageDto.FileName,
                Format = imageDto.Format,
                Data = imageDto.Data,
                Thumbnail = imageDto.ThumbData
            };
            this.m_imageFileWriter.Add(imageFileData);
            try
            {
                this.m_dataSource.Save();
            }
            catch
            {
                this.m_dataSource.RollbackTransaction();
                return -1;
            }
            ImageInformation image = new ImageInformation
            {
                Name = imageDto.ImageName,
                Description = imageDto.Description,
                FileId = imageFileData.Id,
                Created = DateTime.Now,
                Updated = DateTime.Now,
            };
            this.m_imageWriter.Add(image);
            try
            {
                this.m_dataSource.Save();
            }
            catch
            {
                this.m_dataSource.RollbackTransaction();
                return -1;
            }
            if (image.Id == 0)
            {
                this.m_dataSource.RollbackTransaction();
                return 0;
            }
            this.m_dataSource.CommitTransaction();

            if (imageDto.Tags != null)
            {
                foreach (string tag in imageDto.Tags)
                {
                    this.AddTag(image.Id, tag);
                }
            }
            if (imageDto.Categories != null)
            {
                foreach (string category in imageDto.Categories)
                {
                    this.AddCategory(image.Id, category);
                }
            }

            return image.Id;
        }
        public long SetAuthor(long imageId, long authorDataId, bool _delete)
        {
            if (imageId == 0 || authorDataId == 0) return 0;
            ImageInformation? image = this.m_images.FirstOrDefault(i => i.Id == imageId);
            if (image == null) return 0;
            ArtistInformation? author = this.m_artists.FirstOrDefault(a => a.Id == authorDataId);
            if (author == null) return 0;
            ImageRecord? record = this.m_imageRecords.FirstOrDefault(r => r.ObjectId == imageId && r.AttrType == AttributeType.Artist);
            if (_delete)
            {
                if (record == null) return 0;
                this.m_imageRecordWriter.Remove(record);
                if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
                return author.Id;
            }
            else
            {
                if (record == null)
                {
                    this.m_imageRecordWriter.Add(new ImageRecord
                    {
                        AttrId = author.Id,
                        AttrType = AttributeType.Artist,
                        ObjectId = imageId,
                    });
                }
                else
                {
                    record.AttrId = author.Id;
                    this.m_imageRecordWriter.Update(record);
                }
                if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
                return author.Id;
            }
        }

        public long AddCategory(long imageId, string categoryName)
        {
            return this.setObjectAttr<ImageInformation, ImageRecord, CategoryInformation>(imageId, AttributeType.Category, categoryName, false);
        }
        public long RemoveCategory(long imageId, string categoryName)
        {
            return this.setObjectAttr<ImageInformation, ImageRecord, CategoryInformation>(imageId, AttributeType.Category, categoryName, true);
        }
        public long AddTag(long imageId, string tagName)
        {
            return this.setObjectAttr<ImageInformation, ImageRecord, TagInformation>(imageId, AttributeType.Tag, tagName, false);
        }
        public long RemoveTag(long imageId, string tagName)
        {
            return this.setObjectAttr<ImageInformation, ImageRecord, TagInformation>(imageId, AttributeType.Tag, tagName, true);
        }



        public BasicDetails? GetImageDetail(long id)
        {
            if (id == 0) return null;
            ImageInformation? image = this.m_images.FirstOrDefault(i => i.Id == id);
            if (image == null) return null;
            return new BasicDetails
            {
                Id = image.Id,
                Name = image.Name,
                Description = image.Description,
            };
        }
        public BasicDetails? GetTagDetail(long id)
        {
            return this.m_tags.Select(x => new BasicDetails
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).FirstOrDefault(t => t.Id == id);
        }
        public BasicDetails? GetCategoryDetail(long id)
        {
            return this.m_categories.Select(x => new BasicDetails
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).FirstOrDefault(t => t.Id == id);
        }

        public ApiFileModel? GetFullImage(long imgId)
        {
            if (imgId == 0) return null;
            var meta = this.ImageFiles.Select(x => new
            {
                ImageId = x.ImageId,
                FileName = x.FileName,
                Format = x.Format,
                Data = x.Data,
            }).FirstOrDefault(x => x.ImageId == imgId);
            if (meta == null) return null;
            return new ApiFileModel
            {
                FileName = meta.FileName,
                Format = meta.Format,
                Base64 = Convert.ToBase64String(meta.Data),
            };
        }

        public ApiFileModel? GetThumbnail(long imgId)
        {
            if (imgId == 0) return null;
            var meta = this.ImageFiles.Select(x => new
            {
                ImageId = x.ImageId,
                FileName = x.FileName,
                Format = x.Format,
                Data = x.Thumbnail,
            }).FirstOrDefault(x => x.ImageId == imgId);
            if (meta == null) return null;
            return new ApiFileModel
            {
                FileName = meta.FileName,
                Format = meta.Format,
                Base64 = Convert.ToBase64String(meta.Data),
            };
        }

        public IEnumerable<ApiThumbViewModel> GetThumbnails(QueryModel? queryModel)
        {
            if (queryModel == null)
            {
                return this.ImageFiles.Select(x => new ApiThumbViewModel
                {
                    FileName = x.FileName,
                    ImageName = x.ImageName,
                    Format = x.Format,
                    Base64 = Convert.ToBase64String(x.Thumbnail),
                    ImageId = x.ImageId,
                    FileId = x.FileId,
                }).ToList();
            }
            return [];
        }

        public IEnumerable<BasicInfo> GetImageTags(long imgId)
        {
            if (imgId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<ImageRecord, TagInformation>(imgId, AttributeType.Tag);
        }

        public IEnumerable<BasicInfo> GetImageCategories(long imgId)
        {
            if (imgId == 0) return Enumerable.Empty<BasicInfo>();
            return this.getBasicInfo<ImageRecord, CategoryInformation>(imgId, AttributeType.Category);
        }
    }
}
