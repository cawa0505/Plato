﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListViewComponent : ViewComponent
    {

        private readonly ICollection<Filter> _defaultFilters = new List<Filter>()
        {
            new Filter()
            {
                Text = "All",
                Value = FilterBy.All
            },
            new Filter()
            {
                Text = "-" // represents menu divider
            },
            new Filter()
            {
                Text = "My Topics",
                Value = FilterBy.MyTopics
            },
            new Filter()
            {
                Text = "Participated",
                Value = FilterBy.Participated
            },
            new Filter()
            {
                Text = "Following",
                Value = FilterBy.Following
            },
            new Filter()
            {
                Text = "Starred",
                Value = FilterBy.Starred
            },
            new Filter()
            {
                Text = "-"  // represents menu divider
            },
            new Filter()
            {
                Text = "Unanswered",
                Value = FilterBy.Unanswered
            },
            new Filter()
            {
                Text = "No Replies",
                Value = FilterBy.NoReplies
            }
        };

        private readonly ICollection<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Last Reply",
                Value = SortBy.LastReply
            },
            new SortColumn()
            {
                Text = "Replies",
                Value =  SortBy.Replies
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "Participants",
                Value =  SortBy.Participants
            },
            new SortColumn()
            {
                Text = "Reactions",
                Value =  SortBy.Reactions
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  SortBy.Follows
            },
            new SortColumn()
            {
                Text = "Stars",
                Value =  SortBy.Stars
            },
            new SortColumn()
            {
                Text = "Created",
                Value = SortBy.Created
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.Modified
            }
        };

        private readonly ICollection<SortOrder> _defaultSortOrder = new List<SortOrder>()
        {
            new SortOrder()
            {
                Text = "Descending",
                Value = OrderBy.Desc
            },
            new SortOrder()
            {
                Text = "Ascending",
                Value = OrderBy.Asc
            },
        };

        private readonly IFeatureFacade _featureFacade;
        private readonly IEntityService<Topic> _entityService;

        public TopicListViewComponent(
            IEntityService<Topic> entityService,
            IFeatureFacade featureFacade)
        {
            _entityService = entityService;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new EntityIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }
        
        async Task<EntityIndexViewModel<Topic>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Get results
            var results = await _entityService.GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new EntityIndexViewModel<Topic>
            {
                SortColumns = _defaultSortColumns,
                SortOrder = _defaultSortOrder,
                Filters = _defaultFilters,
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}

