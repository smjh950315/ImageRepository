using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Artist;
using ImgRepo.Model.Entities.Image;
using ImgRepo.Model.Image;
using ImgRepo.Model.Query;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class ImageService : CommonObjectService<ImageInformation, ImageRecord>, IImageService
    {
        class MetaImageFileBinding
        {
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
            public long ImageId { get; set; }
            public string ImageName { get; set; }
            public long FileId { get; set; }
            public string FileName { get; set; }
            public string Format { get; set; }
            public byte[] Data { get; set; }
            public byte[] Thumbnail { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        }

        IQueryable<ImageInformation> m_images;
        IDataWriter<ImageInformation> m_imageWriter;
        IQueryable<ArtistInformation> m_artists;
        IDataWriter<ArtistInformation> m_artistWriter;

        IQueryable<ImageRecord> m_imageRecords;
        IDataWriter<ImageRecord> m_imageRecordWriter;

        IQueryable<ImageFileData> m_imagefiles;
        IDataWriter<ImageFileData> m_imageFileWriter;

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

        long updateAuthorIdUnchecked(ImageInformation image, long? authorDataId)
        {
            image.ArtistId = authorDataId;
            image.Updated = DateTime.Now;
            this.m_imageWriter.Update(image);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return authorDataId ?? 0;
        }

        public ImageService(IDataSource dataSource) : base(dataSource)
        {
            this.m_images = dataSource.GetQueryable<ImageInformation>();
            this.m_imageWriter = dataSource.GetWriter<ImageInformation>();
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

        public long RenameImage(long imageId, string newName)
        {
            return this.renameObject<ImageInformation>(imageId, newName);
        }

        public override long RemoveObject(long imageId)
        {
            long removedId = this.removeObject<ImageInformation>(imageId);
            if (removedId <= 0) return removedId;
            ImageFileData? imageFile = this.m_imagefiles.FirstOrDefault(f => f.Id == removedId);
            if (imageFile == null) return removedId;
            this.m_imageFileWriter.Remove(imageFile);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return removedId;
        }

        public long SetAuthor(long imageId, long authorDataId, bool _delete)
        {
            if (imageId == 0) return 0;
            if (authorDataId <= 0) _delete = true;

            ImageInformation? image = this.m_images.FirstOrDefault(i => i.Id == imageId);
            if (image == null) return 0;

            if (_delete)
            {
                return image.ArtistId.HasValue ? this.updateAuthorIdUnchecked(image, null) : 0;
            }
            else
            {
                ArtistInformation? author = this.m_artists.FirstOrDefault(a => a.Id == authorDataId);
                if (author == null)
                {
                    return image.ArtistId.HasValue ? this.updateAuthorIdUnchecked(image, null) : 0;
                }

                if (image.ArtistId.HasValue)
                {
                    if (image.ArtistId.Value == authorDataId) return authorDataId;
                    return this.updateAuthorIdUnchecked(image, authorDataId);
                }
                else
                {
                    return this.updateAuthorIdUnchecked(image, authorDataId);
                }
            }
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

        public IEnumerable<ApiThumbFileModel> GetThumbnails(QueryModel? queryModel)
        {
            if (queryModel == null)
            {
                return this.ImageFiles.Select(x => new ApiThumbFileModel
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
    }
}
