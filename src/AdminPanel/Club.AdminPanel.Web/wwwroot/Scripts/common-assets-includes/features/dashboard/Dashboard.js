window.dashboard_configSelection = function (span) {
    $("input[name='ConfigId']").val($(span).attr("configId"));
    $($("input[name='ConfigId']").closest("form")[0]).submit();
}

window.dashboard_deleteConfig = function (i) {
    var configName = $(i).attr('ch_name');
	 if (!confirm(window.tetaI18n.t("SureToDeleteDashboardConfig", configName))) return;
    var ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: $(i).attr('cid')
    }
    $(i).closest("ul").dropdown('toggle');
    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Dashboard/DeleteConfig',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: window.AddAntiForgeryToken(),
        success: function (res) {
            if (res) {
                $(i).closest("li").remove();
            }
        },
        error: function () {
				alert(window.tetaI18n.t('Error Occured') /*+ e.responseText*/);
            console.log(ajaxParams);
        }
    });
}

window.dashboard_renameConfig = function (i) {
    var configName = $(i).attr('ch_name');
    var newName = prompt(window.tetaI18n.t('EnterNewNameOfConfig'), configName);
    if (newName) {
        var ajaxParams = {
            newName: newName,
            NamespaceId: window.PageAddressManager.getNamespaceId(),
            EntityId: window.PageAddressManager.getEntityId(),
            DashboardId: window.PageAddressManager.getPageId(),
            ConfigId: $(i).attr('cid')
        }
        $(i).closest("ul").dropdown('toggle');
        var obj = JSON.stringify(ajaxParams);
        $.ajax({
            type: 'POST',
            url: window.top.rootUrl+'Dashboard/RenameConfig',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            processData: true,
            cache: false,
            data: obj,
            headers: window.AddAntiForgeryToken(),
            success: function (res) {
                if (res) {
                    var title = $(i).closest("li").find('span').attr("title");
                    var tObj = title.split('-');
                    title = tObj[0] + ' - ' + res;
                    $(i).closest("li").find('span').attr("title", title);
                    var out = "";
                    if (res.length > 75) {
                        $(i).closest("li").find('span').html(res.substring(0, 75) + '...');
                    } else {
                        $(i).closest("li").find('span').html(res);
                    }
                }
            },
            error: function (e) {
					 alert(window.tetaI18n.t('Error Occured') + e.responseText);
                Error(e);
            }
        });
    }
}

window.dashboard_addConfig = function (i) {
	 var configName = prompt(window.tetaI18n.t('EnterNewConfigName'), "");
    if (configName) {
        var ajaxParams = {
            NamespaceId: window.PageAddressManager.getNamespaceId(),
            EntityId: window.PageAddressManager.getEntityId(),
            DashboardId: window.PageAddressManager.getPageId(),
            ConfigName: configName
        }
        var obj = JSON.stringify(ajaxParams);
        $.ajax({
            type: 'POST',
            url: window.top.rootUrl + 'Dashboard/AddNewConfig',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            processData: true,
            cache: false,
            data: obj,
            headers: window.AddAntiForgeryToken(),
            success: function (res) {
                if (res) {
                    $("input[name='ConfigId']").val(res);
                    $($("input[name='ConfigId']").closest("form")[0]).submit();
                }
            },
            error: function (e) {
					 alert(window.tetaI18n.t('Error Occured') + e.responseText);
                Error(e);
            }
        });
    }
}

window.dashboard_addWidget = function (i) {
    var ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: $("input[name='ConfigId']").val(),
        reportNamespaceId: $(i).attr('rns'),
        reportEntityId: $(i).attr('re'),
        ReportId: $(i).attr('rid'),
        ReportConfigId: $(i).attr('cid')
    };
    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Dashboard/AddNewWidget',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: window.AddAntiForgeryToken(),
        success: function (res) {
            changeOccured();
        },
        error: function (e) {
				alert(window.tetaI18n.t('Error Occured') + e.responseText);
            Error(e);
        }
    });
}

window.dashboard_removeWidget = function (i) {
    var ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: $("input[name='ConfigId']").val(),
        widgetId: $(i).attr('widgetId')
    }
    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Dashboard/RemoveWidget',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: window.AddAntiForgeryToken(),
        success: function (res) {
            $(i).parent().addClass('flipOutY animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                $(this).remove();
            });
//            changeOccured();
        },
        error: function (e) {
				alert(window.tetaI18n.t('Error Occured') + e.responseText);
            Error(e);
        }
    });
}

var eReportViewType = {'Chart' : 2};

window.dashboard_submitWidgetSettings = function (i, event) {
    // Prevent default button behavior and stop event propagation
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }

    var ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: $("input[name='ConfigId']").val(),
        widgetId: $(i).attr('widgetId'),
        widgetWidth: $('#widgetWidth-' + $(i).attr('widgetId')).val(),
        widgetHeight: $('#widgetHeight-' + $(i).attr('widgetId')).val(),
        recordsCount: $('#recordsCount-' + $(i).attr('widgetId')).val(),
        ChartType : $('#selectChart-' + $(i).attr('widgetId')).val()
    }
    
    $('#widgetSettingsModal-' + $(i).attr('widgetId')).modal('hide');

    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Dashboard/SubmitWidgetSettings',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: window.AddAntiForgeryToken(),
        success: function (res) {
            location.reload(true);
            changeOccured();
        },
        error: function (e) {
				alert(window.tetaI18n.t('Error Occured') + e.responseText);
            Error(e);
        }
    });
}

var weAreInDesignMode = false;
window.dashboard_showDesignControls = function () {
    weAreInDesignMode = weAreInDesignMode ? false : true;
    $('.design-control').toggleClass('hidden');
//    console.log($('#ReportBody sortable').sortable("toArray")); //debug
    if (weAreInDesignMode) {
        $('#ReportBody')
            .sortable({
                update: function(event, ui) {
                    //                    var data = $(this).sortable('serialize');
                    var orderedWidgetIds = [];
                    $('#ReportBody .sortable')
                        .each(function() {
                        orderedWidgetIds[orderedWidgetIds.length] = $(this).attr('widgetId');
                    });
                    var ajaxParams = {
                        NamespaceId: window.PageAddressManager.getNamespaceId(),
                        EntityId: window.PageAddressManager.getEntityId(),
                        DashboardId: window.PageAddressManager.getPageId(),
                        ConfigId: $("input[name='ConfigId']").val(),
                        widgetIds: orderedWidgetIds
                    };
                   var obj = JSON.stringify(ajaxParams);
                   $.ajax({
                       type: 'POST',
                       url: window.top.rootUrl + 'Dashboard/SubmitWidgetsOrder',                       
                       contentType: 'application/json; charset=utf-8',
                       dataType: 'json',
                       async: true,
                       processData: true,
                       cache: false,
                       data: obj,
                       headers: window.AddAntiForgeryToken(),
                       success: function (res) {
//                            changeOccured();
                       },
                       error: function (e) {
									alert(window.tetaI18n.t('Error Occured') + e.responseText);
                       }
                   });
                }
            });
        $('#ReportBody').sortable("enable");
    } else {
        $('#ReportBody').sortable("disable");
}
}

var isSeeChangesEnabled = false;
function changeOccured() {
    console.log("majid: ", isSeeChangesEnabled);
    if (!isSeeChangesEnabled) {
        $('#seeChanges').toggleClass('hidden');
        isSeeChangesEnabled = true;
    }
}

$('#seeChanges').on('click', function() {
    location.reload(true);
});
