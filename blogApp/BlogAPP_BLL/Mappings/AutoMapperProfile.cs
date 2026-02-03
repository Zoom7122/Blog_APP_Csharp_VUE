using AutoMapper;
using BlogAPP_Core.Models;
using blogApp_DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid().ToString())) 
                .ForMember(dest => dest.Created_at,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Is_active,
                    opt => opt.MapFrom(_ => true)) 
                .ForMember(dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password,
                    opt => opt.MapFrom(src => src.Password)) 
                .ForMember(dest => dest.Avatar_url,
                    opt => opt.MapFrom(src => src.Avatar_url ?? string.Empty))
                .ForMember(dest => dest.Bio,
                    opt => opt.MapFrom(src => src.Bio ?? string.Empty))
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.Role) ? "User" : src.Role));

            //CreateMap<User, UserEnrance>()
            //    .ForMember(x => x.Email,
            //     y => y.MapFrom(y => y.Email))
            //    .ForMember(x => x.Role,
            //     y => y.MapFrom(y => y.Role))
            //    .ForMember(x => x.Avatar_url,
            //     y => y.MapFrom(y => y.Avatar_url))
            //    .ForMember(x => x.Bio,
            //     y => y.MapFrom(y => y.Bio))
            //    .ForMember(x => x.Name,
            //     y => y.MapFrom(y => y.FirstName));
            CreateMap<User, UserEnrance>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName));

            CreateMap<CreateArticleModel, Article>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));
        }

    }
}
