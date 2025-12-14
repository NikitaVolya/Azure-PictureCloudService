using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class Liked
    {
        [Key]
        public int? UserId { get; set; }
        public User? User { get; set; }

        [Key]
        public int PictureId { get; set; }
        public Picture Picture { get; set; } = null!;
    }
}
