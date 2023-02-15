using AutoMapper;
using CardStorageService.Data.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests;
using System.Security;

namespace CardStorageService.Mappings;

public class MappingsProfile : Profile
{
	public MappingsProfile()
	{
		CreateMap<Card, CardDto>();
		CreateMap<CreateCardRequest, Card>();
		CreateMap<CreateClientRequest, Client>();

		CreateMap<AccountSession, SessionDto>()
			.ForMember(x => x.SessionId, opt => opt.MapFrom(src => src.SessionId))
			.ForMember(x => x.SessionToken, opt => opt.MapFrom(src => src.SessionToken))
			.ForMember(x => x.Account, opt => opt.MapFrom(src => src.Account));
		CreateMap<Account, AccountDto>();
	}
}