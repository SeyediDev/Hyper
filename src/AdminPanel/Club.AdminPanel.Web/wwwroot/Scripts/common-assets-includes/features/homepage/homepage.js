(function($) {
    $(function() {
        addBreaks();
        updateHeightIfNeeded();
        $(window).resize(function() {
            updateHeightIfNeeded();
        });
    });

    function addBreaks() {
        var $mainPart = $('#main-part');
        $mainPart.append($('<span />', { 'class': 'break neo-control' }));
        $mainPart.append($('<span />', { 'class': 'break neo-control' }));
        $mainPart.append($('<span />', { 'class': 'break neo-control' }));
    }

    function updateHeightIfNeeded() {
        var dontBeInfinite = 100;
        while (document.documentElement.scrollWidth >
            Math.max(document.documentElement.clientWidth, window.innerWidth || 0) &&
            dontBeInfinite > 0) {
            var newHeight = $('#main-part').height() + 50;
            $('#main-part').css('height', newHeight + 'px');
            dontBeInfinite--;
        }
    }

})(window.jQuery);