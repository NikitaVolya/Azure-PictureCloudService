using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.Comment;
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
        private IMapper _mapper;

        const long MaxPictureSize = 100L * 1024 * 1024;

        public PictureController(AppDbContext appDbContext, PictureComputerVisionService visionService, PictureService pictureService, IMapper mapper)
        {
            _context = appDbContext;
            _visionService = visionService;
            _pictureService = pictureService;
            _mapper = mapper;
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
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);


            Picture? picture = await _context.Pictures
                .Include(p => p.User)
                .ThenInclude(u => u.Personne)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (picture == null)
                return NotFound();

            ViewBag.PictureUrl = await _pictureService.GetPictureHref(id);
            ViewBag.LikeCount = await _pictureService.CountLikesAsync(picture.Id);

            if (userLogin != null)
            {
                ViewBag.IsOwner = picture.User.Personne.Login == userLogin;
                ViewBag.IsLiked = await _pictureService.IsLiked(picture.UserId, picture.Id);
            }
            else
            {
                ViewBag.IsOwner = false;
                ViewBag.IsLiked = false;
            }

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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(UploadCommentDto dto)
        {
            ModelState.Remove("UserId");
            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { id = dto.PictureId });

            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin);

            if (user == null)
                return RedirectToAction("Login", "User");

            dto.UserId = user.PersonneId;

            bool result = await _pictureService.AddCommentAsync(dto);

            return RedirectToAction("Details", new { id = dto.PictureId });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q, int page = 1, int pageSize = 12)
        {
            ViewBag.Query = q;

            if (string.IsNullOrWhiteSpace(q))
                return RedirectToAction("Index", "Home");

            var pictures = _pictureService.FindPicturesByText(q).ToList();

            int totalItems = pictures.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedPictures = pictures
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var urls = new Dictionary<int, string>();
            foreach (var pic in pagedPictures)
                urls[pic.Id] = await _pictureService.GetPictureHref(pic.Id);

            ViewBag.PictureUrls = urls;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedPictures);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            Picture? picture = await _context.Pictures
                .Include(p => p.User)
                .ThenInclude(u => u.Personne)
                .Include(p => p.Collections)
                .FirstOrDefaultAsync(p => p.User.Personne.Login == userLogin && p.Id == id);

            if (picture == null)
                return NotFound();

            UpdatePictureDto? updatePictureDto = _mapper.Map<Picture, UpdatePictureDto>(picture);

            ViewBag.UserCollections = await _context.Users
                .Include(u => u.Personne)
                .Include(u => u.Collections)
                .Where(u => u.Personne.Login == userLogin)
                .Select(u => u.Collections)
                .FirstOrDefaultAsync();

            return View(updatePictureDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdatePictureDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _pictureService.UpdatePictureAsync(id, dto);

            if (!success)
                return NotFound();

            return RedirectToAction("Details", new { id });
        }
    }
}
