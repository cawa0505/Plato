﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Badges;
using Plato.Discuss.Tags.Navigation;
using Plato.Discuss.Tags.Tasks;
using Plato.Discuss.Tags.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Discuss.Tags
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

            // Register navigation provider
            services.AddScoped<INavigationProvider, TopicExtraMenu>();

            // Discuss view providers
            services.AddScoped<IViewProviderManager<Topic>, ViewProviderManager<Topic>>();
            services.AddScoped<IViewProvider<Topic>, TopicViewProvider>();
            services.AddScoped<IViewProviderManager<Reply>, ViewProviderManager<Reply>>();
            services.AddScoped<IViewProvider<Reply>, ReplyViewProvider>();

            // Badge providers
            services.AddScoped<IBadgesProvider<Badge>, TagBadges>();

            // Background tasks
            services.AddScoped<IBackgroundTaskProvider, TagBadgesAwarder>();
            
        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }
    }
}