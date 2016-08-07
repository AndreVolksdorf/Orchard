// Copyright (c) Juan M. Elosegui. All rights reserved.
// Licensed under the GPL v2 license. See LICENSE.txt file in the project root for full license information.

using System;
using SmartPage.Maps.MapControl.Helpers;

namespace SmartPage.Maps.MapControl.Objects.Markers.Events
{
    public class MarkerEventsBuilder : IHideObjectMembers
    {
        public MarkerEventsBuilder(MarkerClientEvents clientEvents)
        {
            if (clientEvents == null)
            {
                throw new ArgumentNullException(nameof(clientEvents));
            }

            this.ClientEvents = clientEvents;
        }

        protected MarkerEventsBuilder(MarkerEventsBuilder builder)
            : this(PassThroughNonNull(builder).ClientEvents)
        {
        }

        protected MarkerClientEvents ClientEvents { get; private set; }

        public MarkerEventsBuilder OnMarkerClick(string onMarkerClickHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerClickHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerClickHandlerName));
            }

            this.ClientEvents.OnMarkerClick.HandlerName = onMarkerClickHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerDoubleClick(string onMarkerDoubleClickHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerDoubleClickHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerDoubleClickHandlerName));
            }

            this.ClientEvents.OnMarkerDoubleClick.HandlerName = onMarkerDoubleClickHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerDragStart(string onMarkerDragStartHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerDragStartHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerDragStartHandlerName));
            }

            this.ClientEvents.OnMarkerDragStart.HandlerName = onMarkerDragStartHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerDrag(string onMarkerDragHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerDragHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerDragHandlerName));
            }

            this.ClientEvents.OnMarkerDrag.HandlerName = onMarkerDragHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerDragEnd(string onMarkerDragEndHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerDragEndHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerDragEndHandlerName));
            }

            this.ClientEvents.OnMarkerDragEnd.HandlerName = onMarkerDragEndHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerFlatChanged(string onMarkerFlatChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerFlatChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerFlatChangedHandlerName));
            }

            this.ClientEvents.OnMarkerFlatChanged.HandlerName = onMarkerFlatChangedHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerIconChanged(string onMarkerIconChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerIconChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerIconChangedHandlerName));
            }

            this.ClientEvents.OnMarkerIconChanged.HandlerName = onMarkerIconChangedHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerMouseDown(string onMarkerMouseDownHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerMouseDownHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerMouseDownHandlerName));
            }

            this.ClientEvents.OnMarkerMouseDown.HandlerName = onMarkerMouseDownHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerMouseOut(string onMarkerMouseOutHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerMouseOutHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerMouseOutHandlerName));
            }

            this.ClientEvents.OnMarkerMouseOut.HandlerName = onMarkerMouseOutHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerMouseOver(string onMarkerMouseOverHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerMouseOverHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerMouseOverHandlerName));
            }

            this.ClientEvents.OnMarkerMouseOver.HandlerName = onMarkerMouseOverHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerMouseUp(string onMarkerMouseUpHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerMouseUpHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerMouseUpHandlerName));
            }

            this.ClientEvents.OnMarkerMouseUp.HandlerName = onMarkerMouseUpHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerPositionChanged(string onMarkerPositionChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerPositionChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerPositionChangedHandlerName));
            }

            this.ClientEvents.OnMarkerPositionChanged.HandlerName = onMarkerPositionChangedHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerRightClick(string onMarkerRightClickHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerRightClickHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerRightClickHandlerName));
            }

            this.ClientEvents.OnMarkerRightClick.HandlerName = onMarkerRightClickHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerShapeChanged(string onMarkerShapeChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerShapeChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerShapeChangedHandlerName));
            }

            this.ClientEvents.OnMarkerShapeChanged.HandlerName = onMarkerShapeChangedHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerTitleChanged(string onMarkerTitleChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerTitleChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerTitleChangedHandlerName));
            }

            this.ClientEvents.OnMarkerTitleChanged.HandlerName = onMarkerTitleChangedHandlerName;

            return this;
        }

        public MarkerEventsBuilder OnMarkerVisibleChanged(string onMarkerVisibleChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerVisibleChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerVisibleChangedHandlerName));
            }

            this.ClientEvents.OnMarkerVisibleChanged.HandlerName = onMarkerVisibleChangedHandlerName;

            return this;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Markerz", Justification = "I meant Marker and zIndex")]
        public MarkerEventsBuilder OnMarkerzIndexChanged(string onMarkerzIndexChangedHandlerName)
        {
            if (String.IsNullOrEmpty(onMarkerzIndexChangedHandlerName))
            {
                throw new ArgumentNullException(nameof(onMarkerzIndexChangedHandlerName));
            }

            this.ClientEvents.OnMarkerZIndexChanged.HandlerName = onMarkerzIndexChangedHandlerName;

            return this;
        }

        private static MarkerEventsBuilder PassThroughNonNull(MarkerEventsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder;
        }
    }
}