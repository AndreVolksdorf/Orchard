﻿// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using SmartPage.Maps.MapControl.Location;

namespace SmartPage.Maps.MapControl.Objects.Shapes.Polygons
{
    public class PolygonBuilder : Shape2DBuilder<Polygon>
    {
        public PolygonBuilder(Polygon shape)
            : base(shape)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual PolygonBuilder Points(Action<LocationFactory<Polygon>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var factory = new LocationFactory<Polygon>(this.Shape);
            action(factory);
            return this;
        }
    }
}
