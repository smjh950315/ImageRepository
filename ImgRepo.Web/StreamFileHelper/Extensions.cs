using Cyh.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Text;

namespace ImgRepo.Web.StreamFileHelper
{
    public static class Extensions
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        public static async Task<FormValueProvider> StreamFile(this HttpRequest request, Func<FileMultipartSection, Stream> createStream)
        {
            if (request.ContentType.IsNullOrEmpty())
            {
                throw new Exception("Request content type is null or empty.");
            }
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                throw new Exception($"Expected a multipart request, but got {request.ContentType}");
            }

            // 把 request 中的 Form 依照 Key 及 Value 存到此物件
            KeyValueAccumulator formAccumulator = new KeyValueAccumulator();

            string boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            MultipartReader reader = new MultipartReader(boundary, request.Body);

            MultipartSection? section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                // 把 Form 的欄位內容逐一取出
                bool hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue? contentDisposition);

                if (contentDisposition != null)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        // 若此欄位是檔案，就寫入至 Stream;
                        FileMultipartSection? fileSection = section.AsFileSection();
                        if (fileSection != null)
                        {
                            using (Stream targetStream = createStream(fileSection))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // 若此欄位不是檔案，就把 Key 及 Value 取出，存入 formAccumulator
                        string? key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        Encoding? encoding = GetEncoding(section);
                        if (!key.IsNullOrEmpty() && encoding != null)
                        {
                            using (StreamReader streamReader = new StreamReader(
                                section.Body,
                                encoding,
                                detectEncodingFromByteOrderMarks: true,
                                bufferSize: 1024,
                                leaveOpen: true))
                            {
                                string value = await streamReader.ReadToEndAsync();
                                if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                                {
                                    value = String.Empty;
                                }
                                formAccumulator.Append(key, value);

                                if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                                {
                                    throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                                }
                            }
                        }
                    }
                }

                // 取得 Form 的下一個欄位
                section = await reader.ReadNextSectionAsync();
            }

            // Bind form data to a model
            FormValueProvider formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            return formValueProvider;
        }
        private static Encoding? GetEncoding(MultipartSection section)
        {
            bool hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out MediaTypeHeaderValue? mediaType);
            if (mediaType == null) return null;
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in
            // most cases.
#pragma warning disable SYSLIB0001 // 類型或成員已經過時
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
#pragma warning restore SYSLIB0001 // 類型或成員已經過時
            return mediaType.Encoding;
        }
    }
}
