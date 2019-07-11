﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Issues.Categories.Models;
using Plato.Issues.Categories.Roles.QueryAdapters;
using Plato.Issues.Categories.Roles.ViewProviders;
using Plato.Issues.Models;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Stores.Abstractions.QueryAdapters;

namespace Plato.Issues.Categories.Roles
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

            // Category role view providers
            services.AddScoped<IViewProviderManager<CategoryAdmin>, ViewProviderManager<CategoryAdmin>>();
            services.AddScoped<IViewProvider<CategoryAdmin>, CategoryRolesViewProvider>();

            // Query adapters to limit access by role
            services.AddScoped<IQueryAdapterProvider<Issue>, IssueQueryAdapter>();
            services.AddScoped<IQueryAdapterProvider<Category>, CategoryQueryAdapter>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}