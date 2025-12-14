namespace PictureCloudService.DTO.User
{
    public class ChangePasswordUserDto
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;

    }
}
