﻿using System.Collections.Generic;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Mentions.NotificationTypes
{

    public class WebNotifications : INotificationTypeProvider
    {

        public static readonly WebNotification NewMention =
            new WebNotification("NewQuestionsMentionWeb", "Question Mentions",
                "Show me a web notification for each new @mention within questions.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        NewMention
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewMention
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewMention
                    }
                }

            };
        }

        public IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        NewMention
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        NewMention
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Member,
                    NotificationTypes = new[]
                    {
                        NewMention
                    }
                }

            };

        }
        
    }

}
