﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plato.Docs.Categories.Follow.Notifications;
using Plato.Docs.Categories.Follow.NotificationTypes;
using Plato.Docs.Categories.Follow.Subscribers;
using Plato.Docs.Categories.Follow.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Docs.Models;
using Plato.Follows.Services;
using Plato.Docs.Categories.Models;
using Plato.Internal.Security.Abstractions;
using Plato.Docs.Categories.Follow.Handlers;
using Plato.Internal.Features.Abstractions;

namespace Plato.Docs.Categories.Follow
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

            // Feature installation event handler
            services.AddScoped<IFeatureEventHandler, FeatureEventHandler>();

            // Channel View providers
            services.AddScoped<IViewProviderManager<Category>, ViewProviderManager<Category>>();
            services.AddScoped<IViewProvider<Category>, CategoryViewProvider>();

            // Broker subscriptions
            services.AddScoped<IBrokerSubscriber, FollowSubscriber>();
            services.AddScoped<IBrokerSubscriber, EntitySubscriber<Doc>>();

            // Follow types
            services.AddScoped<IFollowTypeProvider, FollowTypes>();

            // Notification type providers
            services.AddScoped<INotificationTypeProvider, EmailNotifications>();
            services.AddScoped<INotificationTypeProvider, WebNotifications>();

            // Notification managers
            services.AddScoped<INotificationManager<Doc>, NotificationManager<Doc>>();

            // Notification Providers
            services.AddScoped<INotificationProvider<Doc>, NewDocEmail>();
            services.AddScoped<INotificationProvider<Doc>, NewDocWeb>();

            // Register permissions provider
            services.AddScoped<IPermissionsProvider<Permission>, Permissions>();

        }

        public override void Configure(
            IApplicationBuilder app,
            IRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
        }

    }

}