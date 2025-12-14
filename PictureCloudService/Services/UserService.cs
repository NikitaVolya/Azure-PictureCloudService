using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.Models;

namespace PictureCloudService.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public UserService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<User?> GetUserByLoginAsync(string email, string password)
        {
            User? user = await _context.Users
                .Include(u => u.Personne).FirstOrDefaultAsync(u => u.Personne.Email == email);

            return user;
        }

        public async Task<User?> CreateUserAsync(string login, string email, string password)
        {
            Personne? personne = await _authService.CreatePersonneAsync(login, email, password);
            if (personne == null)
            {
                return null;
            }

            User newUser = new User
            {
                PersonneId = personne.Id,
                ConnectionDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}
