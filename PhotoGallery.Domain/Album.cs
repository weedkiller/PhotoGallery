﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGallery.Domain
{
    public class Album
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<Photo> Photos { get; set; }
        public int MainPhotoId { get; set; }
        public virtual Photo MainPhoto { get; set; }
        public string UserId { get; set; }
    }
}
