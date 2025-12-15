using AutoMapper;
using PictureCloudService.DTO.Collection;
using PictureCloudService.Models;

namespace PictureCloudService.Profiles
{
    public class CollectionProfile : Profile
    {
        public CollectionProfile() {
            CreateMap<CreateCollectionDto, Collection>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Collection, UpdateCollectionDto>();
        }
    }
}
