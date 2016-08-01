// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using SmartPage.Maps.MapControl.Helpers;

namespace SmartPage.Maps.MapControl.Events
{
    public interface IClientEventObject
    {
        void SerializeTo(ClientSideObjectWriter writer);
    }
}