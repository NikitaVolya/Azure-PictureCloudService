using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.User
{
    public class RegisterUserDto
    {
        public string Login { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
