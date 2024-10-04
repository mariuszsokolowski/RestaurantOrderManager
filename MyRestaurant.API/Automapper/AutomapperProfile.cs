using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyRestaurant.API.Models;
using MyRestaurant.Data.Entities;

namespace MyRestaurant.API.Automapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<OrderLine, OrderLineModels>().ReverseMap();
            CreateMap<OrderLineModels, OrderLine>().ReverseMap();
            CreateMap<List<OrderLine>, List<OrderLineModels>>().ReverseMap();
            CreateMap<List<OrderLineModels>, List<OrderLine>>().ReverseMap();
        }

    }
}
