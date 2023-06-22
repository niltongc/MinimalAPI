using AutoMapper;
using MagicalVilla_CouponAPI.Models;
using MagicalVilla_CouponAPI.Models.DTO;

namespace MagicalVilla_CouponAPI
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
