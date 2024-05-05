﻿using AutoMapper;
using iSchool.Svs.Appliaction.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace iSchool.Svs.Api.Configurations
{
    public static class AutoMapperSetup
    {
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));
        }
    }
}
