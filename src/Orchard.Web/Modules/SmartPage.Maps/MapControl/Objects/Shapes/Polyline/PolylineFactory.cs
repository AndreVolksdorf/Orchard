// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using SmartPage.Maps.MapControl.Helpers;

namespace SmartPage.Maps.MapControl.Objects.Shapes.Polyline
{
    public class PolylineFactory : MapObject, IHideObjectMembers
    {
        public PolylineFactory(Map map)
            : base(map)
        {
        }

        public PolylineBuilder Add()
        {
            var polyline = new Polyline(this.Map);

            this.Map.Polylines.Add(polyline);

            return new PolylineBuilder(polyline);
        }
    }
}
