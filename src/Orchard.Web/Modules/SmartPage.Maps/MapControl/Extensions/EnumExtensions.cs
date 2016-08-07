// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Linq;
using SmartPage.Maps.MapControl.Helpers;

namespace SmartPage.Maps.MapControl.Extensions
{
    internal static class EnumExtensions
    {
        public static string ToClientSideString(this Enum source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var valueAttribute = source.GetType().GetField(source.ToString())
                                    .GetCustomAttributes(true)
                                    .OfType<ClientSideEnumValueAttribute>()
                                    .FirstOrDefault();

            if (valueAttribute == null)
            {
                throw new InvalidOperationException("You must decorate the enum value with the attribute \"ClientSideEnumValueAttribute\"");
            }

            return valueAttribute.Value.Replace("'", string.Empty);
        }
    }
}
