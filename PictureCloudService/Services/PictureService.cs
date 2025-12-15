using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.Picture;
using PictureCloudService.Models;


namespace PictureCloudService.Services
{
    public class PictureService
    {
        private PictureComputerVisionService _visionService;
        private AppDbContext _context;
        private IMapper _mapper;

        public PictureService(PictureComputerVisionService pictureComputerVisionService, AppDbContext context, IMapper mapper)
        {
            _visionService = pictureComputerVisionService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Picture?> CreatePictureAsync(string userLogin, UploadPictureDto uploadPictureDto)
        {
            User? user = await _context.Users
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Personne.Login == userLogin);

            if (user == null)
                return null;

            Picture picture = _mapper.Map<Picture>(uploadPictureDto);

            picture.UserId = user.PersonneId;

            var analysis = await _visionService.AnalyzePictureAsync(uploadPictureDto.File);
            picture.Tags = String.Join(", ", analysis.Description.Tags);

            return picture;
        }
    }
}
