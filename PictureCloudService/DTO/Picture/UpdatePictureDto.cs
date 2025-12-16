using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.Picture
{
    public class UpdatePictureDto
    {

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }

        public List<int> CollectionIds { get; set; } = new();
    }
}
