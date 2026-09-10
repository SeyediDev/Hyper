window.TimeSpanManager = function() {
    var ticksToStructuredTime = function(ticks) {
        return {
            days: Math.floor(ticks / (24 * 60 * 60 * 10e6)),
            hours: Math.floor(ticks / (60 * 60 * 10e6)) % 24,
            minutes: Math.floor(ticks / (60 * 10e6)) % 60,
            seconds: Math.floor(ticks / 10e6) % 60,
            milliseconds: ticks % 10e6
        };
    };

    var structureToText = function (timespan, options) {
        options = options || { displayMilliseconds: true };
        return (timespan.days * 24) +
            timespan.hours +
            ":" +
            timespan.minutes +
            ":" +
            timespan.seconds +
            (options.displayMilliseconds && timespan.milliseconds > 0 ?
                "." + timespan.milliseconds : "");
    };

    var toDisplayText = function(ticks, options) {
        var timeSpan = ticksToStructuredTime(ticks);
        return structureToText(timeSpan, options);
    };

    return {
        toDisplayText: toDisplayText,
        ticksToStructuredTime: ticksToStructuredTime
    };
}();