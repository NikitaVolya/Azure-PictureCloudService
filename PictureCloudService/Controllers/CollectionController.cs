using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.Collection;
using PictureCloudService.Models;
using PictureCloudService.Services;
using System.Security.Claims;

namespace PictureCloudService.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly CollectionService _collectionService;
        private AppDbContext _context;
        private IMapper _mapper;

        public CollectionController(CollectionService collectionService, AppDbContext context, IMapper mapper)
        {
            _collectionService = collectionService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            var collections = await _collectionService.GetUserCollectionsAsync(userLogin);
            return View(collections);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCollectionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = await _context.Users.Include(u => u.Personne).FirstOrDefaultAsync(u => u.Personne.Login == userLogin);
            if (userLogin == null || user == null)
                return RedirectToAction("Login", "User");


            await _collectionService.CreateCollectionAsync(dto, user.PersonneId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var collection = await _collectionService.GetCollectionByIdAsync(id);
            if (collection == null)
                return NotFound();

            UpdateCollectionDto dto = _mapper.Map<UpdateCollectionDto>(collection);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCollectionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool success = await _collectionService.UpdateCollectionAsync(id, dto);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");
            if (!await _context.Users
                .Include(u => u.Personne)
                .Include(u => u.Collections)
                .AnyAsync(u => u.Personne.Login == userLogin && u.Collections.Any(c => c.Id == id)))
                return NotFound();

            var success = await _collectionService.DeleteCollectionAsync(id);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var collection = await _collectionService.GetCollectionByIdAsync(id);
            if (collection == null)
                return NotFound();

            string? userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userLogin == null)
                return RedirectToAction("Login", "User");

            ViewBag.IsUserCollection = await _context.Users
                .Include(u => u.Personne)
                .Include(u => u.Collections)
                .AnyAsync(u => u.Personne.Login == userLogin && u.Collections.Any(c => c.Id == id));

            return View(collection);
        }
    }
}
