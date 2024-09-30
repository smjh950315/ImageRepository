﻿using System.Text.Json.Serialization;

namespace ImgRepo.Model.ViewModel
{
    public class FileModel
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("base64")]
        public string Base64 { get; set; }
    }
}
