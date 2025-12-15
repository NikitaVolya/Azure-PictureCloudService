using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PictureCloudService.Data;
using PictureCloudService.DTO.Collection;
using PictureCloudService.Models;

namespace PictureCloudService.Services
{
    public class CollectionService
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public CollectionService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Collection> CreateCollectionAsync(CreateCollectionDto dto, int userId)
        {
            Collection collection = _mapper.Map<Collection>(dto);

            collection.UserId = userId;

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            return collection;
        }

        public async Task<bool> UpdateCollectionAsync(int id, UpdateCollectionDto dto)
        {
            Collection? collection = await _context.Collections.FirstOrDefaultAsync(c => c.Id == id);

            if (collection == null)
                return false;

            collection.Title = dto.Title;
            collection.Description = dto.Description;
            collection.IsPublic = dto.IsPublic;

            _context.Update(collection);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCollectionAsync(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
                return false;

            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Collection>> GetUserCollectionsAsync(string userLogin)
        {
            return await _context.Collections
                .Include(c => c.Pictures)
                .Include(c => c.User)
                .ThenInclude(u => u.Personne)
                .Where(c => c.User.Personne.Login == userLogin)
                .ToListAsync();
        }

        public async Task<Collection?> GetCollectionByIdAsync(int id)
        {
            return await _context.Collections
                .Include(c => c.Pictures)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
