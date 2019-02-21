﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.StopForumSpam.NotificationTypes;
using Plato.Entities.Stores;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Notifications.Extensions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Internal.Tasks.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Discuss.StopForumSpam.SpamOperators
{
    public class ReplyOperator : ISpamOperatorProvider<Reply>
    {

        private readonly ISpamChecker _spamChecker;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityReplyStore<Reply> _replyStore;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<Reply> _notificationManager;

        public ReplyOperator(
            ISpamChecker spamChecker,
            IPlatoUserStore<User> platoUserStore,
            IDeferredTaskManager deferredTaskManager,
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<Reply> notificationManager,
            IEntityReplyStore<Reply> replyStore)
        {
            _spamChecker = spamChecker;
            _platoUserStore = platoUserStore;
            _deferredTaskManager = deferredTaskManager;
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _notificationManager = notificationManager;
            _replyStore = replyStore;
        }

        public async Task<ISpamOperatorResult<Reply>> ValidateModelAsync(ISpamOperatorContext<Reply> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Reply.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Get user for reply
            var user = await _platoUserStore.GetByIdAsync(context.Model.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<Reply>();

            // User is OK
            var spamResult = await _spamChecker.CheckAsync(user);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);
            
        }

        public async Task<ISpamOperatorResult<Reply>> UpdateModelAsync(ISpamOperatorContext<Reply> context)
        {

            // Perform validation
            var validation = await ValidateModelAsync(context);

            // Create result
            var result = new SpamOperatorResult<Reply>();

            // Not an operator of interest
            if (validation == null)
            {
                return result.Success(context.Model);
            }

            // If validation succeeded no need to perform further actions
            if (validation.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Get topic author
            var user = await _platoUserStore.GetByIdAsync(context.Model.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // Flag user as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                var bot = await _platoUserStore.GetPlatoBotAsync();

                // Mark user as SPAM
                if (!user.IsSpam)
                {
                    user.IsSpam = true;
                    user.IsSpamUpdatedUserId = bot?.Id ?? 0;
                    user.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
                    await _platoUserStore.UpdateAsync(user);
                }

                // Mark reply as SPAM
                if (!context.Model.IsSpam)
                {
                    context.Model.IsSpam = true;
                    await _replyStore.UpdateAsync(context.Model);
                }

            }

            // Defer notifications for execution after request completes
            _deferredTaskManager.AddTask(async ctx =>
            {
                await NotifyAsync(context);
            });

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);

        }

        async Task NotifyAsync(ISpamOperatorContext<Reply> context)
        {

            // Get users to notify
            var users = await GetUsersAsync(context.Operation);

            // No users to notify
            if (users == null)
            {
                return;
            }

            // Get bot
            var bot = await _platoUserStore.GetPlatoBotAsync();

            // Send notifications
            foreach (var user in users)
            {

                // Web notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.ReplySpam))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.ReplySpam)
                    {
                        To = user,
                        From = bot
                    }, context.Model);
                }

                // Email notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.ReplySpam))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.ReplySpam)
                    {
                        To = user
                    }, context.Model);
                }

            }

        }

        async Task<IEnumerable<User>> GetUsersAsync(ISpamOperation operation)
        {

            ConcurrentDictionary<string, User> output = null;

            // Notify administrators 
            if (operation.NotifyAdmin)
            {
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.RoleName.Equals(DefaultRoles.Administrator);
                    })
                    .ToList();
                if (users?.Data != null)
                {
                    output = new ConcurrentDictionary<string, User>();
                    foreach (var user in users.Data)
                    {
                        if (!output.ContainsKey(user.Email))
                        {
                            output.TryAdd(user.Email, user);
                        }
                    }

                }
            }

            // Notify staff 
            if (operation.NotifyStaff)
            {
                var users = await _platoUserStore.QueryAsync()
                    .Select<UserQueryParams>(q =>
                    {
                        q.RoleName.Equals(DefaultRoles.Staff);
                    })
                    .ToList();

                if (users == null)
                {
                    return null;
                }
                if (users?.Data != null)
                {
                    if (output == null)
                    {
                        output = new ConcurrentDictionary<string, User>(); ;
                    }
                    foreach (var user in users.Data)
                    {
                        if (!output.ContainsKey(user.Email))
                        {
                            output.TryAdd(user.Email, user);
                        }
                    }
                }
            }

            return output?.Values ?? null;

        }

    }

}