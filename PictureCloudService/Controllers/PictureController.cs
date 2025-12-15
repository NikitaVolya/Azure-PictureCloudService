using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using PictureCloudService.DTO.Picture;
using PictureCloudService.Models;
using PictureCloudService.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PictureCloudService.Controllers
{
    public class PictureController : Controller
    {
        private PictureComputerVisionService _visionService;
        private PictureService _pictureService;

        const long MaxPictureSize = 100L * 1024 * 1024;

        public PictureController(PictureComputerVisionService visionService, PictureService pictureService)
        {
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

            ImageAnalysis analyzis = await _visionService.AnalyzePictureAsync(uploadPictureDto.File);

            if (analyzis.Adult.IsAdultContent || analyzis.Adult.IsGoryContent || analyzis.Adult.IsRacyContent)
            {
                ModelState.AddModelError("File", "Picture contradicts platform policy");
                return View(uploadPictureDto);
            }

            Picture? new_picture = await _pictureService.CreatePictureAsync(userLogin, uploadPictureDto);

            return RedirectToAction("Index", "Home");
        }
    }
}
