using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.Picture
{
    public class UploadPictureDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
