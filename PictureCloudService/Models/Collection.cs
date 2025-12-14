using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
