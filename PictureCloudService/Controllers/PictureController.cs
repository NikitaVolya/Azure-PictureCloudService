using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.Picture;
using PictureCloudService.Models;
using PictureCloudService.Services;
using System.Security.Claims;


namespace PictureCloudService.Controllers
{
    public class PictureController : Controller
    {
        private AppDbContext _context;
        private PictureComputerVisionService _visionService;
        private PictureService _pictureService;

        const long MaxPictureSize = 100L * 1024 * 1024;

        public PictureController(AppDbContext appDbContext, PictureComputerVisionService visionService, PictureService pictureService)
        {
            _context = appDbContext;
            _visionService = visionService;
            _pictureService = pictureService;
        }


        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Upload(UploadPictureDto uploadPictureDto)
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
            {
                return RedirectToAction("Login", "User");
            }
            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (!ModelState.IsValid)
            {
                return View(uploadPictureDto);
            }

            if (uploadPictureDto.File.Length == 0)
            {
                ModelState.AddModelError("File", "Picture is clear");
                return View(uploadPictureDto);
            }
            if (uploadPictureDto.File.Length > MaxPictureSize)
            {
                ModelState.AddModelError("File", "Picture is to big");
                return View(uploadPictureDto);
            }
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(uploadPictureDto.File.ContentType))
            {
                ModelState.AddModelError("File", "Wrong picture format");
                return View(uploadPictureDto);
            }

            ImageAnalysis analyzis = await _visionService.AnalyzePictureAsync(uploadPictureDto.File);

            if (analyzis.Adult.IsAdultContent || analyzis.Adult.IsGoryContent || analyzis.Adult.IsRacyContent)
            {
                ModelState.AddModelError("File", "Picture contradicts platform policy");
                return View(uploadPictureDto);
            }

            Picture? new_picture = await _pictureService.CreatePictureAsync(userLogin, uploadPictureDto);

            if (new_picture == null)
            {
                return View(uploadPictureDto);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
            {
                return RedirectToAction("Login", "User");
            }

            Picture? picture = await _context.Pictures
                .Include(p => p.User)
                .ThenInclude(u => u.Personne)
                .FirstOrDefaultAsync(p => p.User.Personne.Login == userLogin && p.Id == id);

            if (picture == null) { 
                return NotFound();
            }

            await _pictureService.DeletePictureAsync(picture);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like([FromRoute] int id) {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin);

            if (user == null)
                return RedirectToAction("Login", "User");

            await _pictureService.AddLikeAsync(user.PersonneId, id);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unlike([FromRoute] int id)
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin);

            if (user == null)
                return RedirectToAction("Login", "User");

            await _pictureService.RemoveLikeAsync(user.PersonneId, id);

            return Ok();
        }

        [Authorize]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            Picture? picture = await _context.Pictures
                .Include(p => p.User)
                .ThenInclude(u => u.Personne)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (picture == null)
                return NotFound();

            ViewBag.PictureUrl = await _pictureService.GetPictureHref(id);
            ViewBag.IsLiked = await _pictureService.IsLiked(picture.UserId, picture.Id);
            ViewBag.LikeCount = await _pictureService.CountLikesAsync(picture.Id);

            return View(picture);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int pictureId)
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin);

            if (user == null)
                return RedirectToAction("Login", "User");

            if (await _pictureService.IsLiked(user.PersonneId, pictureId))
                await _pictureService.RemoveLikeAsync(user.PersonneId, pictureId);
            else
                await _pictureService.AddLikeAsync(user.PersonneId, pictureId);

            return RedirectToAction("Details", new { id = pictureId });
        }
    }
}
