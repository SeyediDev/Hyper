function submitColorsSettings(modalKey) {
	var ajaxParams = {
        NamespaceId: window.top.modalObjects[modalKey].NamespaceId,
        EntityId: window.top.modalObjects[modalKey].EntityId,
        ReportId: window.top.modalObjects[modalKey].ReportId,
        ConfigId: window.top.modalObjects[modalKey].ConfigId,
        success: $("#_color_success-" + modalKey).val(),
        danger: $("#_color_danger-" + modalKey).val(),
        info: $("#_color_info-" + modalKey).val(),
        warning: $("#_color_warning-" + modalKey).val(),
        active: $("#_color_active-" + modalKey).val()
    };
    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Report/ApplyColorsSettings',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: AddAntiForgeryToken(),
        success: function (res) {
            location.reload();
        },
        error: function (e) {
				alert(window.tetaI18n.t('Error Occured') + e.responseText);
        }
    });
}
