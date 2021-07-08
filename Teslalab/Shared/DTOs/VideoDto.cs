using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Teslalab.Shared.DTOs
{
    public class VideoDto
    {
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [StringLength(255)]
        public string VideoUrl { get; set; }

        public string ThumpUrl { get; set; } // from server to client
        public IFormFile ThumbFile { get; set; }// submit video url from client to server

        public int Views { get; set; }
        public int Likes { get; set; }
        public DateTime PublishedDate { get; set; }

        public Category Category { get; set; }
        public VideoPrivacy Privacy { get; set; }

        //public virtual List<PlaylistVideo> PlaylistVideos { get; set; }
        //public virtual List<Comment> Comments { get; set; }
        public virtual List<string> Tags { get; set; }

        public IEnumerable<CommentDto> Comments { get; set; }
    }
}