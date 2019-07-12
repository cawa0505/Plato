﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Middleware
{

    public class AuthenticatedUserMiddleware
    {
     
        private readonly RequestDelegate _next;

        public AuthenticatedUserMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            // Hydrate HttpContext.Features with our user
            await HydrateHttpContextFeatureAsync(context);
            
            // Return next delegate
            await _next.Invoke(context);
            
        }

        #region "Private Methods"

        async Task HydrateHttpContextFeatureAsync(HttpContext context)
        {

            // We are not authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // Get context facade
            var contextFacade = context.RequestServices.GetRequiredService<IContextFacade>();
            if (contextFacade == null)
            {
                return;
            }

            // Attempt tto get user from data store
            var user = await contextFacade.GetAuthenticatedUserAsync();

            // User not found
            if (user == null)
            {
                return;
            }
            
            // Add authenticated user to features for subsequent use
            context.Features[typeof(User)] = user;
            
        }
        
        #endregion

    }

}
