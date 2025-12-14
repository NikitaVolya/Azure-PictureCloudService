using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.Models;

namespace PictureCloudService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Personne?> CreatePersonneAsync(string email, string login, string password)
        {
            if (await _context.Personnes.AnyAsync(p => p.Email == email || p.Login == login))
            {
                return null;
            }

            Personne personne = new Personne
            {
                Email = email,
                Login = login,
                HeshPassword = HashPassword(password)
            };

            await _context.Personnes.AddAsync(personne);
            await _context.SaveChangesAsync();

            return personne;
        }

        public async Task<Models.Personne?> LoginPersonneAsync(string email, string password)
        {
            var hashedPassword = HashPassword(password);
            var personne = await _context.Personnes
                .FirstOrDefaultAsync(p => p.Email == email && p.HeshPassword == hashedPassword);

            return personne;
        }

        public string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
