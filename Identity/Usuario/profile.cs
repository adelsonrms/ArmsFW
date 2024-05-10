using app.core.Domain.Request;
using app.core.Domain.Response;
using ArmsFW.Domain.Entities;
using AutoMapper;

namespace ArmsFW.AutoMapperExtensions
{
    public class UsuarioProfile : Profile
	{
		public UsuarioProfile()
		{
			CreateMap<UsuarioLocal, UsuarioResponse>().ReverseMap();
			CreateMap<UsuarioRequest, UsuarioLocal>().ReverseMap();
		}
    }
}
