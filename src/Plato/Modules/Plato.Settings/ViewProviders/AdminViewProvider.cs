﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Theming.Abstractions;
using Plato.Settings.Models;
using Plato.Settings.ViewModels;

namespace Plato.Settings.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<SettingsIndex>
    {

        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ISiteThemeLoader _themeLoader;
        private readonly ILocaleProvider _localeProvider;
        private readonly IHomeRouteManager _homeRouteManager;
        private readonly IPlatoHost _platoHost;
        private readonly IShellSettings _shellSettings;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminViewProvider(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            ITimeZoneProvider timeZoneProvider,
            ISiteThemeLoader themeLoader,
            ILocaleProvider localeProvider,
            ISiteSettingsStore siteSettingsStore,
            IPlatoHost platoHost,
            IShellSettings shellSettings,
            IHomeRouteManager homeRouteManager)
        {

            _timeZoneProvider = timeZoneProvider;
            _themeLoader = themeLoader;
            _localeProvider = localeProvider;
            _siteSettingsStore = siteSettingsStore;
            _shellSettings = shellSettings;
            _homeRouteManager = homeRouteManager;
            _platoHost = platoHost;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public override Task<IViewProviderResult> BuildIndexAsync(SettingsIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SettingsIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override  async Task<IViewProviderResult> BuildEditAsync(SettingsIndex settings, IViewProviderContext context)
        {

            var viewModel = await GetModel();
            return Views(
                View<SiteSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header"),
                View<SiteSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools"),
                View<SiteSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1),
                View<SiteSettingsViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(SettingsIndex viewModel,
            IViewProviderContext context)
        {
            var model = new SiteSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(viewModel, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {


                var settings = await _siteSettingsStore.GetAsync();
                if (settings != null)
                {
                    settings.SiteName = model.SiteName;
                    settings.TimeZone = model.TimeZone;
                    settings.DateTimeFormat = model.DateTimeFormat;
                    settings.Culture = model.Culture;
                    settings.Theme = model.Theme;
                }
                else
                {
                    // Create new settings
                    settings = new SiteSettings()
                    {
                        SiteName = model.SiteName,
                        TimeZone = model.TimeZone,
                        DateTimeFormat = model.DateTimeFormat,
                        Culture = model.Culture,
                        Theme = model.Theme
                    };
                }

                // Update settings
                var result = await _siteSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);

                }
            }

            return await BuildEditAsync(viewModel, context);
            
        }

        // ---------------

        async Task<SiteSettingsViewModel> GetModel()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                return new SiteSettingsViewModel()
                {
                    SiteName = settings.SiteName,
                    TimeZone = settings.TimeZone,
                    DateTimeFormat = settings.DateTimeFormat,
                    Culture = settings.Culture,
                    Theme = settings.Theme,
                    HomePageRoute = settings.HomeRoute.Id,
                    AvailableTimeZones = await GetAvailableTimeZonesAsync(),
                    AvailableDateTimeFormat = GetAvailableDateTimeFormats(),
                    AvailableCultures = await GetAvailableCulturesAsync(),
                    AvailableThemes = GetAvailableThemes(),
                    AvailableHomePageRoutes = GetAvailableHomePageRoutes()
                };
            }

            // return default settings
            return new SiteSettingsViewModel()
            {
                SiteName = "Example Site"
            };

        }
        
        IEnumerable<SelectListItem> GetAvailableDateTimeFormats()
        {

            var formats = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            foreach (var value in DateTimeFormats.Defaults)
            {
                formats.Add(new SelectListItem
                {
                    Text = System.DateTime.UtcNow.ToString(value),
                    Value = value
                });
            }

            return formats;

        }

        async Task<IEnumerable<SelectListItem>> GetAvailableTimeZonesAsync()
        {
            // Build timezones 
            var timeZones = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };
            foreach (var z in await _timeZoneProvider.GetTimeZonesAsync())
            {
                timeZones.Add(new SelectListItem
                {
                    Text = z.DisplayName,
                    Value = z.Id
                });
            }

            return timeZones;
        }

        async Task<IEnumerable<SelectListItem>> GetAvailableCulturesAsync()
        {
            // Build timezones 
            var locales = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            // From all locale descriptors get a unique list of supported culture cdes
            var uniqueLocales = new List<string>();
            var localeDescriptors = await _localeProvider.GetLocalesAsync();
            foreach (var localDescriptor in localeDescriptors)
            {
                if (!uniqueLocales.Contains(localDescriptor.Descriptor.Name))
                {
                    uniqueLocales.Add(localDescriptor.Descriptor.Name);
                }
            }

            foreach (var locale in uniqueLocales)
            {
                locales.Add(new SelectListItem
                {
                    Text = locale,
                    Value = locale
                });
            }

            return locales;
        }
        
        IEnumerable<SelectListItem> GetAvailableThemes()
        {

            // Build timezones 
            var themes = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            foreach (var theme in _themeLoader.AvailableThemes)
            {
                themes.Add(new SelectListItem
                {
                    Text = theme.Name,
                    Value = theme.FullPath.ToLower()
                });
            }

            return themes;
        }

        IEnumerable<SelectListItem> GetAvailableHomePageRoutes()
        {
            // Build routes 
            var output = new List<SelectListItem>();
            var routes = _homeRouteManager.GetDefaultRoutes();
            if (routes != null)
            {
                foreach (var route in routes)
                {
                    output.Add(new SelectListItem
                    {
                        Text = route.Id,
                        Value = route.Id
                    });
                }
            }

            return output;

        }

    }

}
