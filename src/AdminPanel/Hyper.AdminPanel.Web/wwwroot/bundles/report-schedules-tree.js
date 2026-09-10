var ScheduledReportFileType = {
    'HTML': 1,
    'Excel': 2
};
var ScheduledReportOutputType = {
    'FileDirectory': 1,
    'Email': 2,
    'FileInfo': 3,
    'SMS': 4
};
var ScheduleType = {
    'Once': 1,
    'Recurring': 2
};
var JobFrequency = {
    'Daily': 1,
    'Weekly': 2,
    'Monthly': 3
};
var DailyFrequency = {
    'OnceAtSpesificTime': 1,
    'Hourly': 2
};
var ScheduledReport = function() {
    var constSubmitUri= 'ScheduledReport/SaveScheduledReport';
    var constSaveUri= 'ScheduledReport/SaveScheduledFilterValues';
    var constDeleteUri= 'ScheduledReport/DeleteScheduledReport';
    var constFetchUri= 'ScheduledReport/FetchScheduledReport';
    var constTakeUri= 'ScheduledReport/TakeReport';
    var constScheduleLogUri= 'Form?EntityId=ScheduledReportLog&NamespaceId=SystemConfigs';

    var deleteSchedule = function (id, configId, cb) {
        var result = confirm("آیا از حذف این زمان‌بندی مطمئنید؟");
        if (!result) return;
	    $.ajax({
		    type: "POST",
		    url: window.top.rootUrl +
            constDeleteUri +
            '?ConfigReportId=' +
	        configId +
            '&ScheduledReportId=' +
			    id,		    
		    success: function () {
		        if (cb) 
		            cb();		        
		    },
		    headers: AddAntiForgeryToken()		    
	    });
    };

    var acquireCurrentPageScheduleId = function () {
        var url = window.location.href;
        if (url.indexOf('/LoadScheduled') !== -1)
            return url.substring(url.indexOf('scheduleId=') + 'scheduleId='.length);
        return undefined;
    };

    var takeReport = function (id, configId) {
        $.ajax({
            type: "POST",
            url: window.top.rootUrl +
                constTakeUri +
                '?ConfigReportId=' +
                configId +
                '&ScheduledReportId=' +
                id,
            headers: AddAntiForgeryToken()

        });
    };

    var acquireFormDataObj = function (withFilter) {
        var result = {};

        result.ReportConfigId = $('#ConfigId').val();
        if (withFilter)
            result.FilterValues = FilterConfig.acquireFilter();

        var $scheduleModal = $("#scheduled-report-modal");
        result.Id = $scheduleModal.find('#schedule-id').val();
        result.Name = $scheduleModal.find('#scheduled-report-name').val();
        result.ScheduleTypeId = $scheduleModal.find('#schedule-type').val();
        result.IsDisable = $scheduleModal.find('#schedule-is-disabled').prop('checked');
        result.Date = $scheduleModal.find('#scheduled-report-date').val();
        result.Time = $scheduleModal.find('#scheduled-report-time').val();
        result.Every = $scheduleModal.find('#scheduled-report-repetition').val();
        result.JobFrequencyId = $scheduleModal.find('#schedule-job-frequency').val();
        result.OnWeekDaysId = 0;
        $scheduleModal.find('input[name="onWeekDays"]').each(function (idx, item) {
            if ($(item).prop('checked'))
                result.OnWeekDaysId = result.OnWeekDaysId | $(item).val();
        });
        result.MonthlyFrequencyId = $('input[name="monthFrequency"]').prop('checked') ? 1 : $scheduleModal.find('#week-counter').val();
        result.DayOfMonth = $scheduleModal.find('#scheduled-report-dayOfMonth').val();
        result.OnWeekFrequencyId = $scheduleModal.find('[name="WeekDays"]').val();
        result.DailyFrequencyId = $scheduleModal.find('[name="dailyFrequency"]:checked').val();
        if (result.ScheduleTypeId == ScheduleType.Recurring)
            result.Time = $scheduleModal.find('#daily-frequency-hour').val();
        result.HourlyEvery = $scheduleModal.find('#scheduled-report-hourlyEvery').val();
        result.StartTime = $scheduleModal.find('#scheduled-report-starttime').val();
        result.EndTime = $scheduleModal.find('#scheduled-report-endtime').val();
        result.StartDate = $scheduleModal.find('#scheduled-report-startdate').val();
        result.EndDate = $scheduleModal.find('#scheduled-report-enddate').val();
        result.ActionName = $scheduleModal.find('#action-name').val();
        result.FileTypeId = $scheduleModal.find('#file-type').val();
        result.OutputTypeId = $scheduleModal.find('#output-type').val();
        result.MaxRecordCount = $scheduleModal.find('#max-record').val();
        result.UserGroupId = $scheduleModal.find('#scheduled-report-userGroup').val();
        result.UserId = $scheduleModal.find('#scheduled-report-user').val();
        result.DestinationPath = $scheduleModal.find('#destination-path').val();
        result.DestinationUserName = $scheduleModal.find('#destination-user-name').val();
        result.DestinationPassword = $scheduleModal.find('#destination-password').val();
        return result;
    };

    var scheduledReportLog = function (id) {
        window.PageAddressManager.navigateTo(
            constScheduleLogUri +
            '&ScheduledReportId=' + id);
    };

    var showScheduleSettingModal = function (id, parent) {
        var $modal = $('#scheduled-report-modal');
        $modal.find('input[name="newFolderName"]').val('پوشه جدید');
        if (window.jsCanPublishConfigs) {
            $modal.find('input[name=newFolderAccess][value=' + 0 + ']').prop('checked', true);
        }
        $modal.modal('show');
    };

    var submitForm = function (withFilter) {
        var saveScheduledReportObj = acquireFormDataObj(withFilter);
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + constSubmitUri,
            data: saveScheduledReportObj,
            headers: AddAntiForgeryToken(),
            success: function (res) {
                if (parseInt(res.Id).toString() === 'NaN')
                    alert("خطا!" + res);
                else
                    ScheduledReportTreeManager.createScheduleNode(res.Id, saveScheduledReportObj.Name);
                console.log(res);
                $('#scheduled-report-modal').modal('hide');
            },
            error: function () {
                //alert('خطایی رخ داد!');
                window.toast.info('خطایی رخ داد!');
                $('#scheduled-report-modal').modal('hide');
            }
        });
    };

    //todo seems wrong to me!
    var saveFilterValues = function (id) {
        var saveScheduledReportObj = acquireFormDataObj(true);
        $.ajax({
            type: "POST",
            data: saveScheduledReportObj,
            url: window.top.rootUrl +
                constSaveUri +
                '?filterId=' + id,
            headers: AddAntiForgeryToken()
        });
    };

    var fillTheForm = function (data, isNew) {
        var $scheduleModal = $("#scheduled-report-modal");
        $scheduleModal.find('#schedule-id').val(data.Id);
        $scheduleModal.find('#scheduled-report-name').val(data.Name);
        $scheduleModal.find('#schedule-type').val(data.ScheduleTypeId).trigger('change');
        $scheduleModal.find('#schedule-is-disabled').prop('checked', data.IsDisable);
        $scheduleModal.find('#scheduled-report-date').val(data.Date);
        if (data.ScheduleTypeId == ScheduleType.Once)
            $scheduleModal.find('#scheduled-report-time').val(data.Time);
        $scheduleModal.find('#scheduled-report-repetition').val(data.Every);
        $scheduleModal.find('#schedule-job-frequency').val(data.JobFrequencyId).trigger('change');
        $scheduleModal.find('input[name="onWeekDays"]').each(function (idx, item) {
            if ($(item).val() & data.OnWeekDaysId)
                $(item).prop('checked', true);
            else $(item).prop('checked', false);
        });
        if (data.MonthlyFrequencyId === '1')
            $('input[name="monthFrequency"][value="1"]').prop('checked', true);
        else {
            $('input[name="monthFrequency"][value="2"]').prop('checked', true);
            $scheduleModal.find('#week-counter').val(data.MonthlyFrequencyId).trigger('change');
        }
        $scheduleModal.find('#scheduled-report-dayOfMonth').val(data.DayOfMonth);
        $scheduleModal.find('[name="WeekDays"]').val(data.OnWeekFrequencyId);
        $scheduleModal.find('[name="dailyFrequency"][value="' + data.DailyFrequencyId + '"]').prop('checked', true);
        if (data.ScheduleTypeId === ScheduleType.Recurring)
            $scheduleModal.find('#daily-frequency-hour').val(data.Time);
        $scheduleModal.find('#scheduled-report-hourlyEvery').val(data.HourlyEvery);
        $scheduleModal.find('#scheduled-report-starttime').val(data.StartTime);
        $scheduleModal.find('#scheduled-report-endtime').val(data.EndTime);
        $scheduleModal.find('#scheduled-report-startdate').val(data.StartDate);
        $scheduleModal.find('#scheduled-report-enddate').val(data.EndDate);
        $scheduleModal.find('#action-name').val(data.ActionName);
        $scheduleModal.find('#file-type').val(data.FileTypeId).trigger('change');
        $scheduleModal.find('#output-type').val(data.OutputTypeId).trigger('change');
        $scheduleModal.find('#max-record').val(data.MaxRecordCount);
        $scheduleModal.find('#scheduled-report-userGroup').val(data.UserGroupId).trigger('change');
        $scheduleModal.find('#scheduled-report-user').val(data.UserId);
        $scheduleModal.find('#destination-path').val(data.DestinationPath);
        $scheduleModal.find('#destination-user-name').val(data.DestinationUserName);
        $scheduleModal.find('#destination-password').val(data.DestinationPassword);
        if (isNew)
            $('#schedule-without-filter-submit').addClass('disabled');
        else $('#schedule-without-filter-submit').removeClass('disabled');
        showOrHideJobScheduleFeatures2();
    };

    var showModal = function (scheduleId) {
        var $modal = $('#scheduled-report-modal');
        if (scheduleId === undefined) {
            fillTheForm({
                "Id": 0,
                "Name": "",
                "IsDisable": false,
                "ScheduleTypeId": 2,
                "JobFrequencyId": 2,
                "DailyFrequencyId": 1,
                "StartTime": "00:00",
                "EndTime": "23:59",
                "Date": $('meta[name="todayDate"]').prop('content'),
                "Time": "02:00",
                "StartDate": $('meta[name="todayDate"]').prop('content'),
                "EndDate": null,
                "OnWeekDaysId": 64,
                "Every": 1,
                "MonthlyFrequencyId": 1,
                "DayOfMonth": 1,
                "OnWeekFrequencyId": 1,
                "HourlyEvery": 1,
                "ReportConfigId": null,
                "ActionName": null,
                "FileTypeId": 2,
                "OutputTypeId": 2,
                "UserGroupId": null,
                "UserId": "",
                "MaxRecordCount": 100,
                "DestinationPath": null,
                "DestinationUserName": null,
                "DestinationPassword": null
            }, true);
            $modal.modal('show');
        } else {
            $.get(window.top.rootUrl +
                constFetchUri +
                '?ConfigReportId=' +
                $('#ConfigId').val() +
                '&ScheduledReportId=' +
                scheduleId)
                .then(function (res) {
                    fillTheForm(res, false);
                    $modal.modal('show');
                });
        }
    };

    return {
        showModal: showModal,
        acquireCurrentPageScheduleId: acquireCurrentPageScheduleId,
        deleteSchedule: deleteSchedule,
        submitForm: submitForm,
        takeReport: takeReport,
        saveFilterValues: saveFilterValues,
        showScheduleSettingModal: showScheduleSettingModal,
        scheduledReportLog: scheduledReportLog
    };
}();

$(function() {
    var scheduleId = ScheduledReport.acquireCurrentPageScheduleId();
    if (scheduleId)
        ScheduledReport.showModal(scheduleId);
});
var ScheduledReportTreeManager = function () {    
    var contextMenu = function (node) {
        if (node.type === 'default') return {};
        var items = {
            'execute': {
                'icon': "fa fa-play-circle-o",
                'label': 'اجرا',
                'action': function () {
                    ScheduledReport.takeReport(node.id, node.data.configId);
                }
            },
            'reportLog': {
                'icon': "fa fa-history",
                'label': 'سابقه گزارش گیری',
                'action': function () {
                    ScheduledReport.scheduledReportLog(node.id);
                }
            },
            'remove': {
                'icon': "fa fa-times",
                'label': 'حذف',
                'action': function () {
                    ScheduledReport.deleteSchedule(node.id, node.data.configId, function () {
                        $('#scheduledReport-tree').jstree('delete_node', node.id);
                    });
                }
            }
        };
        if (node.data.configId === $('#ConfigId').val())
            items['settings'] = {
                'icon': "fa fa-cogs",
                'label': 'تنظیمات',
                'action': function () {
                    ScheduledReport.showModal(node.id);
                }
            }
        return items;
    };

    var types = {
        "default": {
            "icon": window.top.rootUrl + "Content/common-assets-includes/icons/folder.svg",
            "valid_children": ["ScheduledReport"]
        },
        "ScheduledReport": {
            "icon": "fa fa-clock-o",
            "valid_children": []
        }
    };

    var createScheduleNode = function (id, text) {
        var $tree = $("#scheduledReport-tree");
        var nodes = $tree.jstree(true).get_json('#', {'flat': true});
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i]["id"] === id) return;
        }
        $tree
            .jstree('create_node',
                'thisConfig',
                {
                    "id": id,
                    "text": text,
                    "data": {"configId": $('#ConfigId').val()},
                    "type": 'ScheduledReport'
                },
                "last");
    };
//    nodeMoved: function (e, data) {
//        if (data['old_parent'] !== data['parent'])
//            console.warn('Must save ' + data.node.id + '\'s parent as ' + data['parent']); //todo
//    },

    var goToSchedule = function (id, configId) {
        window.location = window.top
            .rootUrl +
            'Report/LoadScheduled?configId=' +
            configId +
            '&scheduleId=' +
            id;
    };

    var instantiateTree = function () {
        $('#scheduledReport-tree')
            .jstree({
                'core': {
                    'data': window.scheduledReportsTree,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': contextMenu
                },
                "types": types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (node, event) {
                    if (node.type === 'default')
                        return false;
                    goToSchedule(node.id, node.data.configId);
                    return false;
                },
                "plugins": ["contextmenu", "types", "conditionalselect", "wholerow"]
            })
            //.on('move_node.jstree', ScheduledReportTreeManager.nodeMoved)
            .on('ready.jstree', function () {
                $("#scheduledReport-tree").jstree("open_all");
                $("#scheduledReport-tree").jstree("select_node",
                    ScheduledReport.acquireCurrentPageScheduleId());
            })
            //            .on('search.jstree before_open.jstree', function (e, data) {
            //                if (data.instance.settings.search.show_only_matches) {
            //                    data.instance._data.search.dom.find('.jstree-node')
            //                        .show().filter('.jstree-last').filter(function () {
            //                        return this.nextSibling;
            //                    }).removeClass('jstree-last')
            //                        .end().end().end().find(".jstree-children").each(function () {
            //                        $(this).children(".jstree-node:visible").eq(-1).addClass("jstree-last");
            //                    });
            //                }
            //            })
            .off('keydown');
    };

    $(instantiateTree);
    return {
        createScheduleNode: createScheduleNode
    };
}();

var jobScheduleFeatures = {
    scheduleType: function () {
        var a = $('#schedule-type').val();
        switch (a) {
            case '1':
                $('.scheduled-report-DateTime').show();
                $('.job-frequency,.daily-frequency,.duration').hide();
                break;
            case '2':
                $('.scheduled-report-DateTime').hide();
                $('.job-frequency,.daily-frequency,.duration').show();
                break;
            default:
                $('.scheduled-report-DateTime,.job-frequency,.daily-frequency,.duration').hide();
        } 
    },
    scheduleJobFrequency: function () {
        var a = $('#schedule-job-frequency').val();
        switch (a) {
            case '1':
                $('.week-days,.month-frequency').hide();
                break;
            case '2':
                $('.week-days').show();
                $('.month-frequency').hide();
                break;
            case '3':
                $('.month-frequency').show();
                $('.week-days').hide();
                break;
            default:
                $('.week-days,.month-frequency').hide();
        }
    },
    scheduleDailyFrequency: function () {
        if ($('.specific-time').prop('checked')) {
            $('#daily-frequency-hour').removeAttr('disabled');
        } else {
            $('#daily-frequency-hour').attr('disabled','true');
        }
        if ($('.hourly-every').prop('checked')) {
            $('#scheduled-report-hourlyEvery,#scheduled-report-starttime,#scheduled-report-endtime').removeAttr('disabled');
        } else {
            $('#scheduled-report-hourlyEvery,#scheduled-report-starttime,#scheduled-report-endtime').attr('disabled', 'true');
            }
    },
    scheduleMonthlyFrequency: function () {
        if ($('.day-of-month').prop('checked')) {
            $('#scheduled-report-dayOfMonth, #scheduled-report-monthfrequency').removeAttr('disabled');
        } else {
            $('#scheduled-report-dayOfMonth, #scheduled-report-monthfrequency').attr('disabled', 'true');
        }
        if ($('.mf').prop('checked')) {
            $('#week-counter,.weekday,#scheduled-report-dailyfrequency').removeAttr('disabled');
        } else {
            $('#week-counter, .weekday, #scheduled-report-dailyfrequency').attr('disabled', 'true');
        }
    },
    scheduleOutput: function () {
        var a = $('#output-type').val();
        switch (a) {
            case '1':
                $('.output-type').show();
                break;
            case '2':
                $('.output-type').hide();
                break;
            case '3':
                $('.output-type').hide();
                break;
            default:
                $('.output-type').hide();
        }
    }
};

function showOrHideJobScheduleFeatures() {
    jobScheduleFeatures.scheduleType();
    jobScheduleFeatures.scheduleJobFrequency();
    jobScheduleFeatures.scheduleDailyFrequency();
    jobScheduleFeatures.scheduleMonthlyFrequency();
    jobScheduleFeatures.scheduleOutput();
}

function showOrHideJobScheduleFeatures2() {
    var scheduleType = $('#schedule-type').val();
    var scheduleJobfrequency = $('#schedule-job-frequency').val();
    var specificTime = $('.specific-time').prop('checked');
    var hourlyEvery = $('.hourly-every').prop('checked');
    var dayOfMonth = $('.day-of-month').prop('checked');
    var monthFrequency = $('.month-frequency').prop('checked');
    var outputType = $('#output-type').val();

    if (scheduleType === '1') {
        $('.scheduled-report-DateTime').show();
    } else {
        $('.scheduled-report-DateTime').hide();
    }
    if (scheduleType === '2') {
        $('.job-frequency,.daily-frequency,.duration').show();
    } else {
        $('.job-frequency,.daily-frequency,.duration').hide();
    }
    if (scheduleJobfrequency === '2') {
        $('.week-days').show();
    } else {
        $('.week-days').hide();
    }
    if (scheduleJobfrequency === '3') {
        $('.month-frequency').show();
    } else {
        $('.month-frequency').hide();
    }
    if (specificTime) {
        $('#daily-frequency-hour').removeAttr('disabled');
    } else {
        $('#daily-frequency-hour').attr('disabled', 'true');
    }

    if (hourlyEvery) {
        $('#scheduled-report-hourlyEvery,#scheduled-report-starttime,#scheduled-report-endtime').removeAttr('disabled');
    } else {
        $('#scheduled-report-hourlyEvery,#scheduled-report-starttime,#scheduled-report-endtime')
            .attr('disabled', 'true');
    }

    if (dayOfMonth) {
        $('#scheduled-report-dayOfMonth, #scheduled-report-monthfrequency').removeAttr('disabled');
    } else {
        $('#scheduled-report-dayOfMonth, #scheduled-report-monthfrequency').attr('disabled', 'true');
    }

    if (monthFrequency) {
        $('#week-counter,.weekday,#scheduled-report-dailyfrequency').removeAttr('disabled');
    } else {
        $('#week-counter, .weekday, #scheduled-report-dailyfrequency').attr('disabled', 'true');
    }

    if (outputType === '1') {
        $('.output-type').show();
    } else {
        $('.output-type').hide();
    }
}
