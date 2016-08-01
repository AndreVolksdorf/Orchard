// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using SmartPage.Maps.MapControl.Helpers;

namespace SmartPage.Maps.MapControl.MapType.ImageMapType
{
    public class ImageMapTypeFactory : IHideObjectMembers
    {
        public ImageMapTypeFactory(Map map)
        {
            this.Map = map;
        }

        protected Map Map { get; private set; }

        public ImageMapTypeBuilder Add()
        {
            var maptype = new ImageMapType();

            this.Map.ImageMapTypes.Add(maptype);

            return new ImageMapTypeBuilder(maptype);
        }
    }
}
