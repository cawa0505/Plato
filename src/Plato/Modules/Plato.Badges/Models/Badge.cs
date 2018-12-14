﻿using System;
using Plato.Internal.Models.Reputations;

namespace Plato.Badges.Models
{

    public class Badge : IBadge
    {

        private string _name;

        private string _description;

        // Globally multiply the default thredhold and bounus points for all badges
        // Default value should be set to 1 increase the accomodate requirements
        public static readonly int ThresholdMultiplier = 0;
        public static readonly int PointsMultiplier = 0;

        public string Category { get; set; }

        public string Name
        {
            get => _name.Replace("{threshold}", this.Threshold.ToString());
            set => _name = value;
        }

        public string Description
        {
            get => _description.Replace("{threshold}", this.Threshold.ToString());
            set => _description = value;
        }

        public string BackgroundIconCss { get; set; } = "fas fa-badge";

        public string IconCss { get; set; } = "fal fa-star";
        
        public int Threshold { get; set; }

        public int BonusPoints { get; set; }

        public bool Enabled { get; set; }

        public BadgeLevel Level { get; set; }

        public IReputation GetReputation()
        {
            return new Reputation(this.Name, string.Empty, this.BonusPoints);
        }
        
        public Badge()
        {
        }

        public Badge(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name)); ;
        }

        public Badge(string name,string description) : this(name)
        {
            this.Description = description ?? throw new ArgumentNullException(nameof(description)); ;
        }
        
        public Badge(
            string name,
            string description,
            string backgroundIconCss) : this(name, description)
        {
            this.BackgroundIconCss = backgroundIconCss;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss) : this(name, description, backgroundIconCss)
        {
            this.IconCss = iconCss;
        }

        public Badge(
            string name,
            string description,
            string iconCss,
            BadgeLevel level) : this(name, description, "fas fa-badge", iconCss)
        {
            this.Level = level;
        }
        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level) : this(name, description, backgroundIconCss, iconCss)
        {
            this.Level = level;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level) : this(name, description)
        {
            this.Level = level;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold) : this(name, description, backgroundIconCss, iconCss, level)
        {
            this.Threshold = ThresholdMultiplier > 0
                ? threshold * ThresholdMultiplier
                : threshold;
        }

        public Badge(
            string name,
            string description,
            string iconCss,
            BadgeLevel level,
            int threshold) : this(name, description, "fas fa-badge", iconCss, level)
        {
            this.Threshold = Badge.ThresholdMultiplier > 0
                ? threshold * ThresholdMultiplier
                : threshold;
        }

        public Badge(
            string name,
            string description,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, "fas fa-badge", iconCss, level, threshold)
        {
            this.BonusPoints = PointsMultiplier > 0 
                ? bonusPoints * PointsMultiplier
                : bonusPoints;
        }

        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold) : this(name, description, level)
        {
            this.Threshold = ThresholdMultiplier > 0 
                ? threshold * ThresholdMultiplier
                : threshold;
        }

        public Badge(
            string name,
            string description,
            string backgroundIconCss,
            string iconCss,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, backgroundIconCss, iconCss, level, threshold)
        {
            this.BonusPoints = Badge.PointsMultiplier > 0
                ? bonusPoints * PointsMultiplier
                : bonusPoints;
        }
        
        public Badge(
            string name,
            string description,
            BadgeLevel level,
            int threshold,
            int bonusPoints) : this(name, description, level, threshold)
        {
            this.BonusPoints = PointsMultiplier > 0
                ? bonusPoints * PointsMultiplier
                : bonusPoints;
        }
        
    }

}
