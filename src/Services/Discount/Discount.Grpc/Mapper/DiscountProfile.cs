using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;

namespace Discount.Grpc.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile() 
        {
            // You use this to convert the Coupon to the one that was created
            // by visual studio e.g. <Coupon -> Coupon Model>
            CreateMap<CouponModel, Coupon>().ReverseMap();
        }
    }
}
