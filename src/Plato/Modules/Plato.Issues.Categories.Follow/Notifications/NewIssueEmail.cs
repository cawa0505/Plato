﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Issues.Categories.Follow.NotificationTypes;
using Plato.Issues.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;


namespace Plato.Issues.Categories.Follow.Notifications
{

    public class NewIssueEmail : INotificationProvider<Issue>
    {
        
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IContextFacade _contextFacade;
        private readonly IEmailManager _emailManager;
        private readonly ILocaleStore _localeStore;

        public NewIssueEmail(
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
        }

        public async Task<ICommandResult<Issue>> SendAsync(INotificationContext<Issue> context)
        {
            
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.NewIssue.Name, StringComparison.Ordinal))
            {
                return null;
            }
            
            // Create result
            var result = new CommandResult<Issue>();

            // Get email template
            const string templateId = "NewIssue";
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email != null)
            {

                // Build topic url
                var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
                var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
                {
                    ["area"] = "Plato.Issues",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = context.Model.Id,
                    ["opts.alias"] = context.Model.Alias
                });
                
                // Build message from template
                var message = email.BuildMailMessage();
                message.Body = string.Format(
                    email.Message,
                    context.Notification.To.DisplayName,
                    context.Model.Title,
                    baseUri + url);
                message.IsBodyHtml = true;
                message.To.Add(new MailAddress(context.Notification.To.Email));

                // Send message
                var emailResult = await _emailManager.SaveAsync(message);
                if (emailResult.Succeeded)
                {
                    return result.Success(context.Model);
                }

                return result.Failed(emailResult.Errors?.ToArray());

            }

            return result.Failed($"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");

        }

    }
    
}
