// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System.Collections.Generic;
using SmartPage.Maps.MapControl.MapType.StyledMapType.Styles;

namespace SmartPage.Maps.MapControl.MapType.StyledMapType
{
    public class StyledMapType : MapTypeBase
    {
        public StyledMapType()
        {
            this.Styles = new List<MapTypeStyle>();
        }

        public IList<MapTypeStyle> Styles { get; private set; }

        public override ISerializer CreateSerializer()
        {
            return new StyledMapTypeSerializer(this);
        }
    }
}