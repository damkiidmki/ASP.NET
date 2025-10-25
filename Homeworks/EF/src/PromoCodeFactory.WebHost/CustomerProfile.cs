using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.PromoCodes,
                opt => opt.MapFrom(src => src.PromoCodes));

        CreateMap<Customer, CustomerShortResponse>();

        CreateMap<PromoCode, PromoCodeShortResponse>()
            .ForMember(dest => dest.BeginDate,
                opt => opt.MapFrom(src => src.BeginDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.EndDate,
                opt => opt.MapFrom(src => src.EndDate.ToString("yyyy-MM-dd")));
    }
}