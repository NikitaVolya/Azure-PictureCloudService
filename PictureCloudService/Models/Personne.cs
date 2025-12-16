

using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.Models
{
    public class Personne
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Login { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public string HeshPassword { get; set; } = null!;
    }
}
