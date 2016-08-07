// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;

namespace SmartPage.Maps.MapControl.Location
{
    public class LocationBindingBuilder<TLocationContainer, TDataItem>
        where TLocationContainer : ILocationContainer
    {
        private readonly LocationBinding<TLocationContainer, TDataItem> locationBinding;

        public LocationBindingBuilder(LocationBinding<TLocationContainer, TDataItem> locationBinding)
        {
            this.locationBinding = locationBinding;
        }

        public LocationBindingBuilder<TLocationContainer, TDataItem> ItemDataBound(Action<TLocationContainer, TDataItem> action)
        {
            this.locationBinding.ItemDataBound = action;
            return this;
        }
    }
}
