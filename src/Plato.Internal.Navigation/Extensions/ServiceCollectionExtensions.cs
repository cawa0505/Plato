﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Internal.Navigation.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoNavigation(
            this IServiceCollection services)
        {

            services.TryAddScoped<INavigationManager, NavigationManager>();
            services.TryAddScoped<IBreadCrumbManager, BreadCrumbManager>();

            return services;

        }
        
    }

}
