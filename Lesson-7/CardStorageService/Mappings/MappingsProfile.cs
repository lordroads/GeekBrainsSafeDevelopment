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

        #region Mappings Proto

        CreateMap<CardStorageServiceProtos.CreateClientRequest, Client>()
			.ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(x => x.Surname, opt => opt.MapFrom(src => src.Surname))
			.ForMember(x => x.Patronymic, opt => opt.MapFrom(src => src.Patronymic));

        CreateMap<CardStorageServiceProtos.CreateCardRequest, Card>()
            .ForMember(x => x.CardNo, opt => opt.MapFrom(src => src.CardNo))
            .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(x => x.CVV2, opt => opt.MapFrom(src => src.CVV2))
            .ForMember(x => x.ExpDate, opt => opt.MapFrom(src => src.ExpDate));

        CreateMap<Card, CardStorageServiceProtos.CardDto>();

        #endregion
    }
}