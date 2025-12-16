using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.Collection
{
    public class UpdateCollectionDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
