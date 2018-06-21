﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Hosting;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Features;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Users.Social.ViewProviders;

namespace Plato.Users.Social
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

            services.AddScoped<IViewProviderManager<User>, ViewProviderManager<User>>();
            services.AddScoped<IViewProvider<User>, UserViewProvider>();


        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}