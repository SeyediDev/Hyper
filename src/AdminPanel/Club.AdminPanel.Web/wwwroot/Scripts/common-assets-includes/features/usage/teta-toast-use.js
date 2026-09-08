/*
 * Provides api consistency and compatibility with previously used toastr
 */
window.toast = function($) {

    var getDelay = function (options) {
        var delay = 0;
        if(options) {
            delay = options.timeOut || options.delay;
        }
        return delay || 5000;
    };

    var showToast = function (type, message, title, options) {
        if(!title) {
            $.snack(type, message, getDelay(options));
        }
        else {
        $.toast({
            type: type,
            title: title,
//          subtitle: '',
            content: message,
            delay: getDelay(options)
//            ,
//            img: {
//                src: '',
//                class: 'rounded-0',
//                alt: ''
//            }
        });
        }
    }

    var info = function(message, title, options) {
        showToast('info', message, title, options);
    };
    var warning = function(message, title, options) {
        showToast('warning', message, title, options);
    };
    var success = function(message, title, options) {
        showToast('success', message, title, options);
    };
    var error = function(message, title, options) {
        showToast('error', message, title, options);
    };

    return {
        info: info,
        warning: warning,
        success: success,
        error: error
    }
}(window.jQuery);
