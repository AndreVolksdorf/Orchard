// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartPage.Maps.MapControl.Helpers.Serialization
{
    public class LocationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Location.Location);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // TODO: camelCase for javascript serialization
            var location = (Location.Location)value;
            var jo = new JObject { { "lat", location.Latitude }, { "lng", location.Longitude } };
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // TODO: camelCase for javascript serialization
            JObject jo = JObject.Load(reader);
            return new Location.Location((double)jo["lat"], (double)jo["lng"]);
        }
    }
}