using AutoMapper;
using Instagraph.DataProcessor.DtoModels;
using Instagraph.Models;

namespace Instagraph.App
{
    public class InstagraphProfile : Profile
    {
        public InstagraphProfile()
        {
            CreateMap<UserDto, User>()
               .ForMember(u => u.ProfilePicture, pp => pp.UseValue<Picture>(null));
        }
    }
}
