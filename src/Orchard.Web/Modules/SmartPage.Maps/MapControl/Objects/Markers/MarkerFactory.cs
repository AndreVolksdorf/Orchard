// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using SmartPage.Maps.MapControl.Helpers;

namespace SmartPage.Maps.MapControl.Objects.Markers
{
    public class MarkerFactory : IHideObjectMembers
    {
        public MarkerFactory(Map map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            this.Map = map;
        }

        protected Map Map { get; private set; }

        public MarkerBuilder Add()
        {
            var marker = new Marker(this.Map);

            this.Map.Markers.Add(marker);

            return new MarkerBuilder(marker);
        }
    }
}
