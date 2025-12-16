using PictureCloudService.Models;

namespace PictureCloudService.Services
{
    public interface IAuthService
    {
        public Task<Models.Personne?> CreatePersonneAsync(string email, string login, string password);

        public Task<Personne?> LoginPersonneAsync(string login, string password);

        public string HashPassword(string password);
    }
}
