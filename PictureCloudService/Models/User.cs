using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class User
    {
        [Key]
        public int PersonneId { get; set; }

        public Personne Personne { get; set; } = null!;

        public DateOnly ConnectionDate { get; set; }

        public List<Picture> Pictures { get; set; } = new();
        public List<Collection> Collections { get; set; } = new();
    }
}
