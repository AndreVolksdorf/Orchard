// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using SmartPage.Maps.MapControl.Location;

namespace SmartPage.Maps.MapControl.Objects.Shapes.Polygons
{
    public class Polygon : Shape2D, ILocationContainer
    {
        private readonly List<Location.Location> points;

        public Polygon(Map map)
            : base(map)
        {
            this.points = new List<Location.Location>();
        }

        public IList<Location.Location> Points
        {
            get
            {
                return this.points.AsReadOnly();
            }
        }

        public virtual void AddPoint(Location.Location point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            this.points.Add(point);
        }

        public override ISerializer CreateSerializer()
        {
            return new PolygonSerializer(this);
        }
    }
}