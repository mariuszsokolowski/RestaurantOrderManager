using AutoMapper;
using RestaurantOrderManager.Data.Entities;
using RestaurantOrderManager.API.Models;

namespace RestaurantOrderManager.API.Automapper
{
    internal sealed class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<OrderLine, OrderLineModels>().ReverseMap();
            CreateMap<OrderLineModels, OrderLine>().ReverseMap();
            //CreateMap<List<OrderLine>, List<OrderLineModels>>().ReverseMap();
            //CreateMap<List<OrderLineModels>, List<OrderLine>>().ReverseMap();
        }

    }
}
