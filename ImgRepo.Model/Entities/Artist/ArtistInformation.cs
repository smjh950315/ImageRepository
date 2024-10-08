﻿using ImgRepo.Data.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Artist
{
    /// <summary>
    /// 藝術家的資訊
    /// </summary>
    public class ArtistInformation : IBasicEntityInformation
    {
        [Required]
        public long Id { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }

        [StringLength(4096)]
        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}
