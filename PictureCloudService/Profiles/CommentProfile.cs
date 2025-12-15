using AutoMapper;
using PictureCloudService.DTO.Comment;
using PictureCloudService.Models;


namespace PictureCloudService.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<UploadCommentDto, PictureComment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
