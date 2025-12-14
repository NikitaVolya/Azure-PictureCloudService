using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class CollectionPicture
    {
        [Key]
        public int Id { get; set; }

        public int CollectionId { get; set; }
        public Collection Collection { get; set; } = null!;

        public int? PictureId { get; set; }
        public Picture? Picture { get; set; }
    }
}
