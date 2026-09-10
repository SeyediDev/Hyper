var FormAddress = (function() {
    function FormAddress(isColumns) {
        this.NamespaceId = window.PageAddressManager.getNamespaceId();
        this.EntityId = window.PageAddressManager.getEntityId();
        this.FormId = window.PageAddressManager.getPageId();
        this.IsAddressingColumns = !!isColumns;
    }

    return FormAddress;
}());

var FormDesignSaver = (function() {
    function FormDesignSaver($, toast, rootUrl, isColumns, addAntiForgeryTokenToHeader) {
        this.$ = $;
        this.toast = toast;
        this.rootUrl = rootUrl;
        this.addAntiForgeryTokenToHeader = addAntiForgeryTokenToHeader;
        this.formAddress = new FormAddress(isColumns);
    }

    FormDesignSaver.prototype.saveTheForm = function(controls, reloadAfterSave) {
        var _this = this;
        var ajaxData = {
            formAddress: this.formAddress,
            controls: controls
        };
        var jsonString = JSON.stringify(ajaxData);
        this.$.ajax({
            type: 'POST',
            url: this.rootUrl + 'Form/SaveTheForm',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            processData: true,
            cache: false,
            data: jsonString,
            headers: this.addAntiForgeryTokenToHeader(),
            success: function(res) {
                _this.toast.success("با موفقیت ذخیره شد.");
                if (reloadAfterSave)
                    location.reload();
            },
            error: function(e) {
                switch (e.status) {
                case 0:
                    _this.toast.error("وقوع خطا در ارتباط با سرور");
                    break;
                case 400:
                    _this.toast.error(e.responseText,
                        "خطای اعتبار سنجی",
                        {
                            "timeOut": 0,
                            "extendedTimeOut": 0
                        });
                    break;
                case 401:
                    _this.toast.error("شما مجاز به طراحی فرم نیستید.");
                    break;
                default:
                    _this.toast.error("ذخیره ی ناموفق");
                    break;
                }
            }
        });
    };
    return FormDesignSaver;
}());