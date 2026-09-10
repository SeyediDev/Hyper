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
