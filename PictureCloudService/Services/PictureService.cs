using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.Comment;
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

        public async Task<bool> IsLiked(int userId, int pictureId)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.PersonneId == userId);

            if (user == null)
                return false;

            return await _context.Likes.AnyAsync(l => l.UserId == user.PersonneId && l.PictureId == pictureId);
        }

        public async Task<bool> AddLikeAsync(int userId, int pictureId)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.PersonneId == userId);

            if (user == null) 
                return false;

            Picture? picture = await _context.Pictures.FirstOrDefaultAsync(p => p.Id == pictureId);

            if (picture == null)
                return false;

            if (await IsLiked(userId, pictureId)) 
                return false;

            Liked liked = new Liked { UserId = userId, PictureId = pictureId };
            await _context.Likes.AddAsync(liked);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveLikeAsync(int userId, int pictureId)
        {
            if (!(await IsLiked(userId, pictureId)))
                return false;

            Liked? liked = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PictureId == pictureId);

            if (liked == null) 
                return false;

            _context.Remove(liked);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountLikesAsync(int pictureId)
        {
            return await _context.Likes.CountAsync(l => l.PictureId == pictureId);
        }

        public async Task<bool> AddCommentAsync(UploadCommentDto uploadCommentDto)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.PersonneId == uploadCommentDto.UserId);
            if (user == null)
                return false;

            if (await _context.BannedUsers.AnyAsync(bu => bu.UserId == uploadCommentDto.UserId))
                return false;

            PictureComment comment = _mapper.Map<PictureComment>(uploadCommentDto);

            await _context.PictureComments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        private static List<string> Tokenize(string text)
        {
            return text
                .ToLowerInvariant()
                .Split(new[] { ' ', ',', '.', '-', '_', '#', '/', '\\' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToList();
        }

        public static int CalculateImageRelevanceScore(string queryText, string imageTitle, string tags)
        {
            if (string.IsNullOrWhiteSpace(queryText))
                return 0;

            int score = 0;

            var queryTokens = Tokenize(queryText);
            var titleTokens = Tokenize(imageTitle);
            var tagTokens = Tokenize(tags);

            foreach (var token in queryTokens)
            {
                if (titleTokens.Contains(token))
                    score += 10;
            }

            if (!string.IsNullOrWhiteSpace(imageTitle) &&
                imageTitle.Contains(queryText, StringComparison.OrdinalIgnoreCase))
            {
                score += 30;
            }

            foreach (var token in queryTokens)
            {
                if (tagTokens.Contains(token))
                    score += 5;
            }

            if (tags.Split(", ").Any(t =>
                t.Equals(queryText, StringComparison.OrdinalIgnoreCase)))
            {
                score += 15;
            }

            return score;
        }

        public IEnumerable<Picture> FindPicturesByText(string text)
        {
            return _context.Pictures
                .Select(p => new { Picture = p, Score = CalculateImageRelevanceScore(text, p.Title, p.Tags) })
                .Where(item => item.Score > 0)
                .OrderBy(item => item.Score)
                .Select(item => item.Picture);
        }
    }
}
