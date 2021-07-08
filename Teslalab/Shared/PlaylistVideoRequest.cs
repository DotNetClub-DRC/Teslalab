using System.ComponentModel.DataAnnotations;

namespace Teslalab.Shared
{
    public class PlaylistVideoRequest
    {
        [Required]
        public string PlaylistId { get; set; }

        [Required]
        public string VideoId { get; set; }
    }
}