using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateProductDto, Product>(); //mapping CreateProductDto to Product
            // We also need to tell our application about this so that we can use it as a service and inject into our product controller
            CreateMap<UpdateProductDto, Product>();
        }
    }
}