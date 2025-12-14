using Microsoft.AspNetCore.Mvc;
using PictureCloudService.Data;
using PictureCloudService.DTO.User;
using PictureCloudService.Models;
using PictureCloudService.Services;

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

            User? user = await _userService.CreateUserAsync(registerUserDto.Login, registerUserDto.Email, registerUserDto.Password);

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

            return RedirectToAction("Index", "Home");
        }
    }
}
