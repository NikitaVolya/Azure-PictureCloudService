using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class BannedUser
    {
        [Key]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
