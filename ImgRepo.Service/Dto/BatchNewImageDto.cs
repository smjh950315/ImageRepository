using Cyh.Net;
using ImageHelperSharp;
using ImgRepo.Model.Entities.Image;
using System.Diagnostics;

namespace ImgRepo.Service.Dto
{
    public class BatchNewImageDto
    {
        class FileDataWithReadMark
        {
            IFileAccessService m_fileAccessService;
            List<ImageFileData> m_fileDatas;

            public IEnumerable<ImageFileData> FileDatas
            {
                get
                {
                    this.ReadExistingUnreadImageInfo();
                    return this.m_fileDatas;
                }
            }

            public FileDataWithReadMark(IFileAccessService fileAccessService)
            {
                this.m_fileDatas = new List<ImageFileData>();
                this.m_fileAccessService = fileAccessService;
            }

            public Stream AddNewImageFile(string uriName, string fileName)
            {
                this.m_fileDatas.Add(new ImageFileData
                {
                    FileName = fileName,
                    Uri = uriName,
                });
                return this.m_fileAccessService.GetWriteStream($"image/{uriName}");
            }

            public IEnumerable<ImageFileData> GetNoInfoFileData()
            {
                return this.m_fileDatas.Where(x => x.Width == 0);
            }

            public void ReadExistingUnreadImageInfo()
            {
                IEnumerable<ImageFileData> noInfoFiles = this.GetNoInfoFileData();
                foreach (ImageFileData noInfoFile in noInfoFiles)
                {
                    byte[] fileBytes = this.m_fileAccessService.GetFile($"image/{noInfoFile.Uri}");
                    if (fileBytes.IsNullOrEmpty()) continue;
                    byte[] thumbnail = StbService.Resize(fileBytes, 256, 256);
                    this.m_fileAccessService.SaveFile($"image/{noInfoFile.Uri}_thumb", thumbnail);
                    ImageHelperSharp.Common.ImInfo imInfo = StbService.GetImageFileInfo(fileBytes);
                    noInfoFile.Format = imInfo.Format;
                    noInfoFile.Width = imInfo.Size.Width;
                    noInfoFile.Height = imInfo.Size.Height;
                    noInfoFile.Channel = imInfo.Channels;
                    noInfoFile.FileSize = fileBytes.Length;
                }
            }
        }

        FileDataWithReadMark m_fileDataWithReadMark;
        List<ImageInformation> m_imageInformationList;

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }
        public int AddedFileCount => this.m_fileDataWithReadMark.FileDatas.Count();

        public BatchNewImageDto(IFileAccessService fileAccessService)
        {
            this.m_fileDataWithReadMark = new FileDataWithReadMark(fileAccessService);
            this.m_imageInformationList = new List<ImageInformation>();
            this.Tags = Array.Empty<string>();
            this.Categories = Array.Empty<string>();
        }

        public void SetTagsByUnsplitedString(string tags)
        {
            this.Tags = tags.SplitNoThrow(',');
        }

        public void SetCategoriesByUnsplitedString(string categories)
        {
            this.Categories = categories.SplitNoThrow(',');
        }

        internal IEnumerable<ImageFileData> GetReadyToSaveFileDatas()
        {
            return this.m_fileDataWithReadMark.FileDatas.Where(x => x.Width != 0);
        }

        internal IEnumerable<ImageFileData> GetSavedFileData()
        {
            return this.m_fileDataWithReadMark.FileDatas.Where(x => x.Id != 0);
        }

        internal IEnumerable<ImageInformation> GetReadyToSaveImageInformations()
        {
            IEnumerable<ImageFileData> hasInfoFiles = this.GetSavedFileData();
            foreach (ImageFileData hasInfoFile in hasInfoFiles)
            {
                Debug.Assert(hasInfoFile.Id != 0);
                if (this.m_imageInformationList.Any(x => x.FileId == hasInfoFile.Id)) continue;
                ImageInformation imageInfo = new ImageInformation
                {
                    Name = this.Name ?? hasInfoFile.FileName,
                    Description = this.Description,
                    Created = DateTime.Now,
                    FileId = hasInfoFile.Id,
                };
                this.m_imageInformationList.Add(imageInfo);
            }
            return this.m_imageInformationList.Where(x => x.Id == 0);
        }

        public Stream WriteAndRecordFile(string uriName, string fileName)
        {
            return this.m_fileDataWithReadMark.AddNewImageFile(uriName, fileName);
        }
    }
}
