(function () {

    var refreshIframe = function (iframe) {
        iframe.src = iframe.src;
    };

    $('.report-iframe').each(function () {
        var refreshTime = $(this).data('refresh-time');
        if (Number(refreshTime) > 0) {
            setInterval(refreshIframe, Number(refreshTime), this);
        }
    });
})();