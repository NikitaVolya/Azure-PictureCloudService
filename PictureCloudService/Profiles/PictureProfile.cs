using AutoMapper;
using PictureCloudService.DTO.Picture;
using PictureCloudService.Models;


namespace PictureCloudService.Profiles
{
    public class PictureProfile : Profile
    {
        public PictureProfile() {

            CreateMap<UploadPictureDto, Picture>()
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
