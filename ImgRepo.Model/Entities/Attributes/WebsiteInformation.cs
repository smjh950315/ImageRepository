﻿using ImgRepo.Data.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Attributes
{
    public class WebsiteInformation : IBasicEntityAttribute
    {
        [Required]
        public long Id { get; set; }

        [StringLength(1024)]
        public string Value { get; set; }

        [StringLength(4096)]
        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        [StringLength(1024)]
        public string? Url { get; set; }
    }
}
