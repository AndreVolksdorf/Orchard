// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

namespace SmartPage.Maps.MapControl.Objects.Shapes.Circle
{
    public class Circle : Shape2D
    {
        public Circle(Map map)
            : base(map)
        {
        }

        public Location.Location Center { get; set; }

        public int Radius { get; set; }

        public override ISerializer CreateSerializer()
        {
            return new CircleSerializer(this);
        }
    }
}
