﻿using Cyh.Net;
using Cyh.Net.Data;
using ImgRepo.Data;
using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using ImgRepo.Model.Entities.Artist;
using ImgRepo.Model.Entities.Attributes;
using ImgRepo.Model.Entities.Image;
using ImgRepo.Model.Image;
using ImgRepo.Model.Query;
using ImgRepo.Service.CommonService;
using ImgRepo.Service.Dto;

namespace ImgRepo.Service.Implement
{
    internal class ImageService : CommonObjectService<ImageInformation, ImageRecord>, IImageService
    {
        class MetaImageFileBinding : IImageFileUriConvertable
        {
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
            public long ImageId { get; set; }
            public string ImageName { get; set; }
            public long FileId { get; set; }
            public string FileName { get; set; }
            public string Format { get; set; }
            public string Uri { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        }

        readonly IQueryable<ImageInformation> m_images;
        readonly IDataWriter<ImageInformation> m_imageWriter;
        readonly IQueryable<ArtistInformation> m_artists;

        readonly IQueryable<ImageFileData> m_imagefiles;
        readonly IDataWriter<ImageFileData> m_imageFileWriter;
        readonly IFileAccessService m_fileAccessService;

        IQueryable<MetaImageFileBinding> ImageFiles => this.m_images.Join(this.m_imagefiles, i => i.FileId, f => f.Id, (i, f) => new MetaImageFileBinding
        {
            ImageId = i.Id,
            ImageName = i.Name,
            FileId = f.Id,
            FileName = f.FileName,
            Format = f.Format,
            Uri = f.Uri,
        });

        long _UpdateAuthorIdUnchecked(ImageInformation image, long? authorDataId)
        {
            image.ArtistId = authorDataId;
            image.Updated = DateTime.Now;
            this.m_imageWriter.Update(image);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return authorDataId ?? 0;
        }

        public ImageService(IDataSource dataSource, IFileAccessService fileAccessService) : base(dataSource)
        {
            this.m_images = dataSource.GetQueryable<ImageInformation>();
            this.m_imageWriter = dataSource.GetWriter<ImageInformation>();
            this.m_artists = dataSource.GetQueryable<ArtistInformation>();

            this.m_imagefiles = dataSource.GetQueryable<ImageFileData>();
            this.m_imageFileWriter = dataSource.GetWriter<ImageFileData>();
            this.m_fileAccessService = fileAccessService;
        }

        public IEnumerable<long> BatchCreateImage(BatchNewImageDto? batchNewImageDto)
        {
            if (batchNewImageDto == null) return Array.Empty<long>();
            IEnumerable<ImageFileData> imageFileDatas = batchNewImageDto.GetReadyToSaveFileDatas();
            if (imageFileDatas.IsNullOrEmpty()) return Array.Empty<long>();
            this.m_imageFileWriter.AddRange(imageFileDatas);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return Array.Empty<long>();
            IEnumerable<ImageInformation> imageInformations = batchNewImageDto.GetReadyToSaveImageInformations();
            if (imageInformations.IsNullOrEmpty()) return Array.Empty<long>();
            this.m_imageWriter.AddRange(imageInformations);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return Array.Empty<long>();
            CommonAttributeService commonAttrService = new CommonAttributeService(this.m_dataSource);
            if (batchNewImageDto.Tags.Length > 0)
            {
                IEnumerable<long> tagIds = commonAttrService.GetIdsByNames<TagInformation>(batchNewImageDto.Tags);
                this.BatchLinkObjectAttributesUnchecked<TagInformation>(imageInformations.Select(x => x.Id), tagIds);
            }
            if (batchNewImageDto.Categories.Length > 0)
            {
                IEnumerable<long> cateIds = commonAttrService.GetIdsByNames<CategoryInformation>(batchNewImageDto.Categories);
                this.BatchLinkObjectAttributesUnchecked<TagInformation>(imageInformations.Select(x => x.Id), cateIds);
            }
            return imageInformations.Select(x => x.Id);
        }

        public double GetImageDifferential(long lhsId, long rhsId)
        {
            byte[] lhsFile = this.m_fileAccessService.GetFile(this.m_imagefiles.FirstOrDefault(f => f.Id == lhsId)?.Uri ?? string.Empty);
            byte[] rhsFile = this.m_fileAccessService.GetFile(this.m_imagefiles.FirstOrDefault(f => f.Id == rhsId)?.Uri ?? string.Empty);
            return ImageHelperSharp.OpenCVService.GetImageDifferential(lhsFile, rhsFile);
        }

        public override long Remove(long imageId)
        {
            long removedId = base.Remove(imageId);
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
                return image.ArtistId.HasValue ? this._UpdateAuthorIdUnchecked(image, null) : 0;
            }
            else
            {
                ArtistInformation? author = this.m_artists.FirstOrDefault(a => a.Id == authorDataId);
                if (author == null)
                {
                    return image.ArtistId.HasValue ? this._UpdateAuthorIdUnchecked(image, null) : 0;
                }

                if (image.ArtistId.HasValue)
                {
                    if (image.ArtistId.Value == authorDataId) return authorDataId;
                    return this._UpdateAuthorIdUnchecked(image, authorDataId);
                }
                else
                {
                    return this._UpdateAuthorIdUnchecked(image, authorDataId);
                }
            }
        }

        public ApiFileModel? GetFullImage(long imgId)
        {
            if (imgId == 0) return null;
            MetaImageFileBinding? meta = this.ImageFiles.FirstOrDefault(x => x.ImageId == imgId);
            if (meta == null) return null;
            byte[] data = this.m_fileAccessService.GetFile(meta.GetFullUri());
            return new ApiFileModel
            {
                FileName = meta.FileName,
                Format = meta.Format,
                Base64 = Convert.ToBase64String(data),
                //FileName = meta.Uri,
                //Format = meta.Format,
                //Base64 = "",
            };
        }

        public ApiFileModel? GetThumbnail(long imgId)
        {
            if (imgId == 0) return null;
            MetaImageFileBinding? meta = this.ImageFiles.FirstOrDefault(x => x.ImageId == imgId);
            if (meta == null) return null;
            byte[] data = this.m_fileAccessService.GetFile(meta.GetThumbFullUri());
            return new ApiFileModel
            {
                FileName = meta.FileName,
                Format = meta.Format,
                Base64 = Convert.ToBase64String(data),
            };
        }

        public IEnumerable<ApiThumbFileModel> GetThumbnails(QueryModel? queryModel, DataRange? dataRange = null)
        {
            IQueryable<ApiThumbFileModel> files = this.ImageFiles.Select(x => new ApiThumbFileModel
            {
                FileName = x.FileName,
                ImageName = x.ImageName,
                Format = x.Format,
                ImageId = x.ImageId,
                FileId = x.FileId,
                Uri = x.Uri,
            });
            List<ApiThumbFileModel> results = new List<ApiThumbFileModel>();
            if (queryModel == null)
            {
                results = files.ToList();
            }
            else
            {
                IEnumerable<long> ids = this.GetIdsByQueryModel(queryModel, dataRange);
                results = files.Where(f => ids.Contains(f.ImageId)).ToList();
            }
            for (int i = 0; i < results.Count; i++)
            {
                ApiThumbFileModel result = results[i];
                result.Base64 = Convert.ToBase64String(this.m_fileAccessService.GetFile(result.GetThumbFullUri()));
            }
            return results;
        }
    }
}
