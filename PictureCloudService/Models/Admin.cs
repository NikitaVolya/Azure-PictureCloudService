using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class Admin
    {
        [Key]
        public int PersonneId { get; set; }

        public Personne Personne { get; set; } = null!;

        public bool SuperAdmin { get; set; } = false;
    }
}
