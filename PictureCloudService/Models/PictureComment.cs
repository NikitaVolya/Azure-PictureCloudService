using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class PictureComment
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int PictureId { get; set; }
        public Picture Picture { get; set; } = null!;

        [MaxLength(500)]
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
