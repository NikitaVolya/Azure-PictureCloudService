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
        private PictureBlobStorage _blobStorage;

        public PictureService(PictureComputerVisionService pictureComputerVisionService, AppDbContext context, IMapper mapper, PictureBlobStorage pictureBlobStorage)
        {
            _visionService = pictureComputerVisionService;
            _context = context;
            _mapper = mapper;
            _blobStorage = pictureBlobStorage;
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

            string type = uploadPictureDto.File.ContentType.Split("/")[1];
            picture.VirtualPath = "none";

            await _context.AddAsync(picture);
            await _context.SaveChangesAsync();

            picture.VirtualPath = Path.Combine(user.Personne.Login, picture.Id.ToString() + "-" + uploadPictureDto.Title) + "." + type;

            await _blobStorage.UploadPictureAsync(picture, uploadPictureDto.File.OpenReadStream());

            return picture;
        }

        public async Task DeletePictureAsync(Picture picture)
        {
            await _blobStorage.DeletePictureAsync(picture);
            _context.Remove(picture);
            await _context.SaveChangesAsync();
        }
    }
}
