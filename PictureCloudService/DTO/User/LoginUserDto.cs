using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.User
{
    public class LoginUserDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
