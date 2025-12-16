using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        public string VirtualPath { get; set; } = null!;

        [MaxLength(200)]
        public string Title { get; set; } = null!;
        public string Tags { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsPublic { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<PictureComment> Comments { get; set; } = new();

        public List<Collection> Collections { get; set; } = new();

    }
}
