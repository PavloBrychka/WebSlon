using AutoMapper;
using WebSlon.Data.Entities;
using WebSlon.Data.Entities.Identity;
using WebSlon.Models.Account;
using WebSlon.Models.Category;

namespace WebSlon.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            CreateMap<CategoryCreateViewModel, CategoryEntity>();

            CreateMap<RegisterViewModel, UserEntity>()
                .ForMember(x => x.UserName, dto => dto.MapFrom(x => x.Email));
        }
    }
}
