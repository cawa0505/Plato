﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Badges;

namespace Plato.Users.Badges.ViewComponents
{
    public class BadgeListItemViewComponent : ViewComponent
    {
 
        public BadgeListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Badge badge)
        {
            return Task.FromResult((IViewComponentResult) View(badge));
        }

    }

}
