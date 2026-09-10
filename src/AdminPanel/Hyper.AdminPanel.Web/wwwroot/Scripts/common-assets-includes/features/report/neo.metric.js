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
            iconClass: userOptions.iconClass == '' ? this.defaultOptions.iconClass : userOptions.iconClass,
            value: userOptions.value,
            title: userOptions.title,
            backgroundColor: userOptions.backgroundColor == '' ? this.defaultOptions.backgroundColor : userOptions.backgroundColor,
            textsColor: userOptions.textsColor == '' ? this.defaultOptions.textsColor : userOptions.textsColor
        };
    },

    create: function (selector, userOptions) {
        var $element = $(selector);
        var options = this.obtainOptions(userOptions);

        var widgetStyles = 'border-radius: 5px;padding: 15px 20px;margin-bottom: 10px;margin-top: 10 px; margin: 10px !important';
        var widgetHtml = '<div style="' + widgetStyles + '"><div class="row">' +
            '<div class="col-4">' +
            '<i class="' + options.iconClass + '" style="font-size: 4rem;"></i></div>' +
            '<div class="col-8 text-right"><span>' + options.title +
            '</span><h2 class="font-bold">&nbsp;&nbsp;' + options.value + '</h2></div></div></div>';

        $element.html(widgetHtml);
    }
}

