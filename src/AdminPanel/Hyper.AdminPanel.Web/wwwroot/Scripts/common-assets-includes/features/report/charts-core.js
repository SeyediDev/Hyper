// Charts Core Functions
// This file contains NeoMetrics and helper functions for charts

var NeoMetrics = {
    defaultOptions: {
        iconClass: 'fa fa-info-circle',
        value: '',
        title: '',
        backgroundColor: '#17a2b8',//If it got changed, ReportResult.cs -> GetDefaultPropertyValue should also change
        textsColor: '#ffffff'//If it got changed, ReportResult.cs -> GetDefaultPropertyValue should also change
    },
    obtainOptions: function (userOptions) {
        return {
            iconClass: userOptions.iconClass === '' ? this.defaultOptions.iconClass : userOptions.iconClass,
            value: userOptions.value,
            title: userOptions.title,
            backgroundColor: userOptions.backgroundColor === '' ? this.defaultOptions.backgroundColor : userOptions.backgroundColor,
            textsColor: userOptions.textsColor === '' ? this.defaultOptions.textsColor : userOptions.textsColor
        };
    },

    create: function (selector, userOptions) {
        var $element = $(selector);
        var options = this.obtainOptions(userOptions);

        var widgetStyles = 'padding: 15px 20px; margin: 10px !important';
        var widgetHtml = '<div style="' + widgetStyles + '"><div class="row mm-metrix-box">' +
            '<div class="col-2">' +
            '<i class="' + options.iconClass + '" style="font-size: 4rem;"></i></div>' +
            '<div class="col-10 text-right"><span>' + options.title +
            '</span><h2 class="font-bold">&nbsp;&nbsp;' + options.value + '</h2></div></div></div>';

        $element.html(widgetHtml);
    }
};

function tooltipFormatter(tooltipsObj) {
    var tooltipHtml =
        "<div style='direction: rtl; z-index:100; background-color:white; margin:-8px; padding:5px;'>";
    for (var field in tooltipsObj) {
        if (Object.prototype.hasOwnProperty.call(tooltipsObj, field)) {
            tooltipHtml += tooltipFormatterItem(field, tooltipsObj[field]);
        }
    }
    tooltipHtml += "</div>";
    return tooltipHtml;
}

function tooltipFormatterItem(field, fieldValue) {
    return '<span class="float-right teta-bidi" style="direction: rtl;">' +
        field +
        ': ' +
        fieldValue +
        '</span><br/>';
}

function grabReportData(key) {
    if (!window.top.reportsData)
        window.top.reportsData = {};
    window.top.reportsData[key] = [];
    return window.top.reportsData[key];
}

window.TimeSpanManager = function () {
    var ticksToStructuredTime = function (ticks) {
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

    var toDisplayText = function (ticks, options) {
        var timeSpan = ticksToStructuredTime(ticks);
        return structureToText(timeSpan, options);
    };

    return {
        toDisplayText: toDisplayText,
        ticksToStructuredTime: ticksToStructuredTime
    };
}();

