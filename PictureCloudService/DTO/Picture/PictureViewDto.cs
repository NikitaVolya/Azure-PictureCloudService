using PictureCloudService.Models;

namespace PictureCloudService.DTO.Picture
{
    public class PictureViewDto
    {
        public Models.Picture Picture { get; set; } = null!;
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
    }
}
