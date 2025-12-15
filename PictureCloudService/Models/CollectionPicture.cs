

namespace PictureCloudService.Models
{
    public class CollectionPicture
    {
        public int CollectionId { get; set; }
        public Collection Collection { get; set; } = null!;

        public int PictureId { get; set; }
        public Picture? Picture { get; set; } = null!;
    }
}
