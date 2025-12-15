using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureCloudService.Data;
using PictureCloudService.DTO.User;
using PictureCloudService.Models;
using PictureCloudService.Services;
using System.Security.Claims;


namespace PictureCloudService.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public UserController(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {

            if (!ModelState.IsValid)
            {
                return View(registerUserDto);
            }

            User? user = await _userService.CreateUserAsync(registerUserDto);

            if (user == null) 
            {
                ViewBag.RegisterError = "User with same email or login is already exits";
                return View(registerUserDto);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginUserDto);
            }

            User? user = await _userService.LoginUserAsync(loginUserDto.Email, loginUserDto.Password);

            if (user == null)
            {
                ViewBag.LoginError = "Email or password is invalide";
                return View(loginUserDto);
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Personne.Login),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordUserDto resetPasswordDto)
        {
            if (!ModelState.IsValid) { 
                return View(resetPasswordDto);
            }

            if (resetPasswordDto.OldPassword == resetPasswordDto.NewPassword)
            {
                ViewBag.NewPassword = "The new password and the old password must be different.";
                return View(resetPasswordDto);
            }

            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
            {
                return RedirectToAction("Login", "User");
            }
            
            bool passwordIsChanged = await _userService.ResetPasswordAsync(userLogin, resetPasswordDto.OldPassword, resetPasswordDto.NewPassword);

            if (!passwordIsChanged) {
                ViewBag.Error = "Password is incorect";
                return View(resetPasswordDto);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
