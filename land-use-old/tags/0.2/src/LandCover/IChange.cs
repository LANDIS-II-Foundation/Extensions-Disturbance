﻿// This file is part of the Land Use extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/exts/land-use/trunk/

using Landis.SpatialModeling;

namespace Landis.Extension.LandUse.LandCover
{
    /// <summary>
    /// Interface for all types of land cover change
    /// </summary>
    public interface IChange
    {
        string Type { get; }

        /// <summary>
        /// Apply the change to the land cover at an individual site.
        /// </summary>
        void ApplyTo(ActiveSite site);
    }
}
