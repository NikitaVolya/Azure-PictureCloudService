using System.ComponentModel.DataAnnotations;

namespace PictureCloudService.DTO.User
{
    public class ResetPasswordUserDto
    {
        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords must match")]
        public string ConfirmedPassword { get; set; } = null!;

    }
}
