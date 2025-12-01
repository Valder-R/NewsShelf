
using AutoMapper;
using BusinessLogic.DTO;
using DataAccess.Entities;

namespace BusinessLogic.Mappers
{
    public class NewsMappingProfile : Profile
    {
        public NewsMappingProfile()
        {
            CreateMap<News, NewsResponse>()
                .ForMember(
                    dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList())
                );

            CreateMap<CreateNewsRequest, News>()
                .ForMember(
                    dest => dest.Images,
                    opt => opt.Ignore() 
                )
                .ForMember(
                    dest => dest.PublishedAt,
                    opt => opt.Ignore() 
                )
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore() 
                );

            CreateMap<UpdateNewsRequest, News>()
                .ForMember(
                    dest => dest.Images,
                    opt => opt.Ignore() 
                )
                .ForMember(
                    dest => dest.PublishedAt,
                    opt => opt.Ignore() 
                )
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore()
                );
        }
    }
}
