﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Admin.Models;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Reporting.Assets;
using Plato.Reporting.Models;
using Plato.Reporting.Navigation;
using Plato.Reporting.Services;
using Plato.Reporting.ViewProviders;

namespace Plato.Reporting
{
    public class Startup : StartupBase
    {
        private readonly IShellSettings _shellSettings;

        public Startup(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {

            // Register assets
            services.AddScoped<IAssetProvider, AssetProvider>();

            // Register navigation provider
            services.AddScoped<INavigationProvider, AdminMenu>();

            // Register admin view providers
            services.AddScoped<IViewProviderManager<Report>, ViewProviderManager<Report>>();
            services.AddScoped<IViewProvider<Report>, AdminViewProvider>();

            // Register admin index View providers
            services.AddScoped<IViewProviderManager<AdminIndex>, ViewProviderManager<AdminIndex>>();
            services.AddScoped<IViewProvider<AdminIndex>, AdminIndexViewProvider>();

            // Services
            services.AddScoped<IDateRangeStorage, RouteValueDateRangeStorage>();

            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}