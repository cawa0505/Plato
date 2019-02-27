﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Navigation.Abstractions
{

    public class PagerOptions
    {
        private int _total;
        private int _totalPages;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int Total => _total;

        public bool Enabled { get; set; } = true;

        public int TotalPages => _totalPages;
        
        // RowOffset
        public int InitialOffset => PageSize * Page - PageSize + 1;

        // 
        public int SelectedOffset { get; set; }

        public ScrollOptions Scroll { get; set; } = new ScrollOptions();

        public void SetTotal(int total)
        {
            _total = total;
            _totalPages = PageSize > 0 ? (int)Math.Ceiling((double)total / PageSize) : 1;
        }

        public PagerOptions()
        {
        }

        public PagerOptions(int offset)
        {

            if (offset <= 0)
            {
                return;
            }

            Page = offset.ToSafeCeilingDivision(PageSize);
            SelectedOffset = offset;

        }


        public PagerOptions(RouteData routeData)
        {
            Page = GetRouteValueOrDefault<int>("pager.page", routeData, Page);
            PageSize = GetRouteValueOrDefault<int>("pager.size", routeData, PageSize);
        }

        private T GetRouteValueOrDefault<T>(string key, RouteData routeData, T defaultValue)
        {

            if (routeData == null)
            {
                return defaultValue;
            }

            var found = routeData.Values.TryGetValue(key, out object value);
            if (found)
            {
                return (T)value;
            }
            return defaultValue;
        }
        
    }

}
