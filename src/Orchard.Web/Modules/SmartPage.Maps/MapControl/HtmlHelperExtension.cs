// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Web.Mvc;

namespace SmartPage.Maps.MapControl
{
    public static class HtmlHelperExtension
    {
        public static MapBuilder GoogleMap(this HtmlHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return new MapBuilder(helper.ViewContext);
        }
    }
}