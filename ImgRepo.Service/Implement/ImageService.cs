using Cyh.Net;
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

        long updateAuthorIdUnchecked(ImageInformation image, long? authorDataId)
        {
            image.ArtistId = authorDataId;
            image.Updated = DateTime.Now;
            this.m_imageWriter.Update(image);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return -1;
            return authorDataId ?? 0;
        }

        long CreateImageRecord(NewImageDto imageDto, Guid guid)
        {
            ImageFileData imageFileData = new ImageFileData
            {
                FileName = imageDto.FileName,
                Width = imageDto.ImInfo.Size.Width,
                Height = imageDto.ImInfo.Size.Height,
                Channel = imageDto.ImInfo.Channels,
                FileSize = imageDto.Data.Length,
                Format = imageDto.ImInfo.Format,
                Uri = guid.ToString(),
            };
            this.m_imageFileWriter.Add(imageFileData);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return 0;
            var image = new ImageInformation
            {
                Name = imageDto.Name,
                Description = imageDto.Description,
                Created = DateTime.Now,
                FileId = imageFileData.Id,
            };
            this.m_imageWriter.Add(image);
            if (!Lib.TryExecute(() => this.m_dataSource.Save())) return 0;
            if (!imageDto.Tags.IsNullOrEmpty())
            {
                var tagIds = new CommonAttributeService(this.m_dataSource).GetIdsByNames<TagInformation>(imageDto.Tags);
                this.BatchLinkObjectAttributesUnchecked<TagInformation>([image.Id], tagIds);
            }
            if (!imageDto.Categories.IsNullOrEmpty())
            {
                var cateIds = new CommonAttributeService(this.m_dataSource).GetIdsByNames<CategoryInformation>(imageDto.Categories);
                this.BatchLinkObjectAttributesUnchecked<TagInformation>([image.Id], cateIds);
            }
            return image.Id;
        }
        long BatchCreateImageRecord(IDictionary<Guid, NewImageDto> imageDtos, bool is_same_batch)
        {
            List<ImageInformation> imageInformations = new List<ImageInformation>();
            List<ImageFileData> imageFileDatas = new List<ImageFileData>();
            foreach (var imageDto in imageDtos)
            {
                imageInformations.Add(new ImageInformation
                {
                    Name = imageDto.Value.Name,
                    Description = imageDto.Value.Description,
                    Created = DateTime.Now,
                });
                imageFileDatas.Add(new ImageFileData
                {
                    FileName = imageDto.Value.FileName,
                    Width = imageDto.Value.ImInfo.Size.Width,
                    Height = imageDto.Value.ImInfo.Size.Height,
                    Channel = imageDto.Value.ImInfo.Channels,
                    FileSize = imageDto.Value.Data.Length,
                    Format = imageDto.Value.ImInfo.Format,
                    Uri = imageDto.Key.ToString(),
                });
            }
            this.m_imageFileWriter.AddRange(imageFileDatas);
            this.m_dataSource.Save();

            for (int i = 0; i < imageInformations.Count; i++)
            {
                imageInformations[i].FileId = imageFileDatas[i].Id;
            }
            this.m_imageWriter.AddRange(imageInformations);
            this.m_dataSource.Save();
            var commonAttrService = new CommonAttributeService(this.m_dataSource);
            if (is_same_batch)
            {
                var tagIds = commonAttrService.GetIdsByNames<TagInformation>(imageDtos.First().Value.Tags);
                var cateIds = commonAttrService.GetIdsByNames<CategoryInformation>(imageDtos.First().Value.Categories);

                this.BatchLinkObjectAttributesUnchecked<TagInformation>(imageInformations.Select(x => x.Id), tagIds);
                this.BatchLinkObjectAttributesUnchecked<CategoryInformation>(imageInformations.Select(x => x.Id), cateIds);
            }
            else
            {
                var dtoIterator = imageDtos.GetEnumerator();
                for (int i = 0; i < imageInformations.Count && dtoIterator.MoveNext(); i++)
                {
                    var tagIds = commonAttrService.GetIdsByNames<TagInformation>(dtoIterator.Current.Value.Tags);
                    this.LinkObjectAttributesUnchecked<TagInformation>(imageInformations[i].Id, tagIds);

                    var cateIds = commonAttrService.GetIdsByNames<CategoryInformation>(dtoIterator.Current.Value.Categories);
                    this.LinkObjectAttributesUnchecked<CategoryInformation>(imageInformations[i].Id, cateIds);
                }
            }
            return imageInformations.First().Id;
        }

        async Task _WriteToFileAsync(ImageContentData imageContentData)
        {
            if (imageContentData.Guid == Guid.Empty || imageContentData == null) return;
            if (imageContentData.Data.IsNullOrEmpty()) return;

            Task<bool> resizeAndWrite = Task.Run(() =>
            {
                var thumbnailBytes = ImageHelperSharp.StbService.Resize(imageContentData.Data, 256, 256);
                if (thumbnailBytes.IsNullOrEmpty()) return false;
                string thumbUriName = $"{imageContentData.Guid}_thumb.{imageContentData.ExtName}";
                return this.m_fileAccessService.SaveObject("image", thumbUriName, imageContentData.Data);
            });
            string uriName = $"{imageContentData.Guid}.{imageContentData.ExtName}";
            this.m_fileAccessService.SaveObject("image", uriName, imageContentData.Data);
            await resizeAndWrite;
        }

        async Task _BatchWriteToFile(IEnumerable<ImageContentData> imageContentDatas)
        {
            List<Task> tasks = new List<Task>();
            foreach (var imageContentData in imageContentDatas)
            {
                tasks.Add(this._WriteToFileAsync(imageContentData));
            }
            tasks.ForEach(async x => await x);
            await Parallel.ForEachAsync(imageContentDatas, async (content, cancellationToken) =>
            {
                await this._WriteToFileAsync(content);
            });
        }


        async Task<long> _CreateImageAsync(NewImageDto? imageDto)
        {
            if (imageDto == null) return 0;
            var guid = Guid.NewGuid();
            var taskWriteFile = this._WriteToFileAsync(new ImageContentData
            {
                Guid = guid,
                Data = imageDto.Data,
                ExtName = imageDto.ImInfo.Format
            });
            var imageId = this.CreateImageRecord(imageDto, guid);
            if (imageId == 0)
            {
                return 0;
            }
            await taskWriteFile;
            return imageId;
        }

        async Task<long> _BatchCreateImageAsync(IEnumerable<NewImageDto> imageDtos, bool is_same_batch)
        {
            List<ImageContentData> imageContentDatas = new();
            Dictionary<Guid, NewImageDto> imageDtosDict = new();
            foreach (var imageDto in imageDtos)
            {
                var guid = Guid.NewGuid();
                imageContentDatas.Add(new ImageContentData
                {
                    Guid = guid,
                    Data = imageDto.Data,
                    ExtName = imageDto.ImInfo.Format
                });
                imageDtosDict[guid] = imageDto;
            }
            var fCreate = this._BatchWriteToFile(imageContentDatas);
            var imgId = this.BatchCreateImageRecord(imageDtosDict, is_same_batch);
            await fCreate;
            return imgId;
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

        public Task<long> CreateImageAsync(NewImageDto? imageDto)
        {
            return this._CreateImageAsync(imageDto);
        }

        public Task<long> BatchCreateImageAsync(IEnumerable<NewImageDto> imageDtos, bool is_same_batch)
        {
            return this._BatchCreateImageAsync(imageDtos, is_same_batch);
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
            MetaImageFileBinding? meta = this.ImageFiles.FirstOrDefault(x => x.ImageId == imgId);
            if (meta == null) return null;
            byte[] data = this.m_fileAccessService.GetFile(meta.GetFullUri());
            return new ApiFileModel
            {
                FileName = meta.FileName,
                Format = meta.Format,
                Base64 = Convert.ToBase64String(data),
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
