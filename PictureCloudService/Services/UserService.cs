using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.User;
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

        public async Task<User?> CreateUserAsync(RegisterUserDto registerUserDto)
        {
            Personne? personne = await _authService.CreatePersonneAsync(registerUserDto.Email, registerUserDto.Login, registerUserDto.Password);
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

        public async Task<User?> LoginUserAsync(string email, string password)
        {
            string passwordHash = _authService.HashPassword(password);

            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Email == email && u.Personne.HeshPassword == passwordHash);

            if (user != null && await IsBannedAsync(user.PersonneId))
            {
                return null;
            }

            return user;
        }

        public async Task<bool> IsBannedAsync(int userId)
        {
            return await _context.BannedUsers
                .AnyAsync(u => u.UserId == userId);
        }

        public async Task BanneUserAsync(User user)
        {
            if (!(await IsBannedAsync(user.PersonneId)))
            {
                BannedUser bannedUser = new BannedUser
                {
                    UserId = user.PersonneId
                };
                await _context.BannedUsers.AddAsync(bannedUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnbanneUserAsync(User user)
        {
            BannedUser? bannedUser = await _context.BannedUsers.FirstOrDefaultAsync(bu => bu.UserId == user.PersonneId);

            if (bannedUser != null)
            {
                _context.BannedUsers.Remove(bannedUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ResetPasswordAsync(string userLogin, string old_password, string new_password)
        {
            string HashOldPassword = _authService.HashPassword(old_password);
            string HashNewPassowrd = _authService.HashPassword(new_password);

            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin && u.Personne.HeshPassword == HashOldPassword);

            if (user == null)
                return false;

            user.Personne.HeshPassword = HashNewPassowrd;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
