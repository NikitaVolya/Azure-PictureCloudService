using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.Comment
{
    public class UploadCommentDto
    {

        public int UserId { get; set; }

        public int PictureId { get; set; }

        [MaxLength(500)]
        public string Content { get; set; } = null!;
    }
}
