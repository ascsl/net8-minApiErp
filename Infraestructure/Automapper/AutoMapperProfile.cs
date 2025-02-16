﻿using AutoMapper;
using ERP;
using ERP.Dtos;
using ERP.Extensions;

namespace MinimalAPIERP.Infraestructure.Automapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.CategoryGuid, opt => opt.MapFrom(src => src.CategoryGuid))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<Store, StoreDto>()
            .ForMember(dest => dest.StoreGuid, opt => opt.MapFrom(src => src.StoreGuid))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.ProductGuid, opt => opt.MapFrom(src => src.ProductGuid))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<Raincheck, RaincheckDto>()
            .ForMember(dest => dest.RaincheckGuid, opt => opt.MapFrom(src => src.RaincheckGuid))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.SalePrice))
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Store))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
    }
}

