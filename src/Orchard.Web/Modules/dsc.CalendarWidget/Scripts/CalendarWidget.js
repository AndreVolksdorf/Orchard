/// <reference path="ext/fullcalendar/ifullcalendar.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="event.ts" />
var dsc;
(function (dsc) {
    var CalendarWidget = (function () {
        function CalendarWidget(Id, Events) {
            $('#' + Id).fullCalendar({
                lang: navigator.userLanguage,
                theme: false,
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay'
                },
                editable: false,
                timezone: 'local',
                events: Events,
                weekNumbers: false
            });
        }
        return CalendarWidget;
    })();
    dsc.CalendarWidget = CalendarWidget;
})(dsc || (dsc = {}));
//# sourceMappingURL=CalendarWidget.js.map
