var OperationButtons = function (notification, address, texts) {
    var startLoader = function ($btn) {
        var $laddaButton = $btn.ladda();
        $laddaButton.ladda('start');
    };

    var stopLoader = function ($btn) {
        $btn.ladda('stop');
    };

    var callOperationAjax = function (controlId, cb) {
        $.ajax({
            type: 'POST',
            headers: window.AddAntiForgeryToken(),
            contentType: 'application/x-www-form-urlencoded',
            data: $('.teta-form').serialize(),
            url: window.top.rootUrl + "form/RunOperationControl?ControlId=" + controlId + "&" + address.getQueryParameters()
        }).then(function (response) {
            cb();
            notification.success(response.Message || texts.t('Successful!'));
        }).catch(function () {
            cb();
            notification.error(texts.t('Error Occured'));
        });
    };

    var getConfirmationMessage = function ($btn) {
        return $btn.data('confirm-message');
    };

    function callOperation($btn) {
        startLoader($btn);
        var controlId = $btn.data('id');
        callOperationAjax(controlId, function () {
            stopLoader($btn);
        });
    }

    $(document).on('click',
        '.operation-button',
        function (e) {
            var $btn = $(this);
            var confirmationMessage = getConfirmationMessage($btn);
            if (confirmationMessage) {
                window.ModalManager.confirm({
                    callback: function (answer) {
                        if (answer === true)
                            callOperation($btn);
                    },
                    message: confirmationMessage
                });
            } else {
                callOperation($btn);
            }
        });
}(window.toast, window.PageAddressManager, window.tetaI18n);
