// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Drawing;
using SmartPage.Maps.MapControl.Extensions;

namespace SmartPage.Maps.MapControl.MapType.ImageMapType
{
    public class ImageMapTypeBuilder : MapTypeBuilder<ImageMapType>
    {
        public ImageMapTypeBuilder(ImageMapType mapType)
            : base(mapType)
        {
        }

        public ImageMapTypeBuilder TileSize(Size value)
        {
            this.MapType.TileSize = value;
            return this;
        }

        public ImageMapTypeBuilder RepeatHorizontally(bool value)
        {
            this.MapType.RepeatHorizontally = value;
            return this;
        }

        public ImageMapTypeBuilder RepeatVertically(bool value)
        {
            this.MapType.RepeatVertically = value;
            return this;
        }

        public ImageMapTypeBuilder TileUrlPattern(string value)
        {
            this.MapType.TileUrlPattern = new Uri(value.ToAbsoluteUrl());
            return this;
        }
    }
}
