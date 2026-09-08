var ProgressManager = function () {
	 var removeUnStartedIcon = function(aiId) {
		  $('#ai-row-' + aiId).find('.unstarted-work-icon').remove();
	 }

    var updateProgress = function (aiId, progress) {
        $('#ai-row-' + aiId).find('.resource-state-text').text('شروع شده');
        if (!$.isNumeric(progress)) return;
		  $('#ai-row-' + aiId).find('.ai-progress').html(progress + '%');
		  removeUnStartedIcon(aiId);
    };

    var updateUserTaskState = function(aiId, readableState, rowClass) {
        $('#ai-row-' + aiId).find('.ai-progress').html('');
        $('#ai-row-' + aiId).find('.resource-state-text').text(readableState);
        if(rowClass !== undefined)
            $('#ai-row-' + aiId).addClass(rowClass);
	 };

    var completeTask = function (aiId) {
        updateUserTaskState(aiId, 'تکمیل شده', 'success');
		  removeUnStartedIcon(aiId);
	 };

    var failTask = function (aiId) {
        updateUserTaskState(aiId, 'ناموفق', 'danger');
		  removeUnStartedIcon(aiId);
	 };

    var inTheQueue = function (aiId) {
        updateUserTaskState(aiId, 'در صفِ انتظار');
    };

    var updateServiceState = function (idx, serviceInfo) {
        //todo more sane way of handling this enum
        switch (serviceInfo.ServiceTaskState) {
            case 1: //created
                inTheQueue(serviceInfo.ActivityInstanceId);
            case 21: //AllocatedToASingleResource
                break;
            case 31: //Started
                updateProgress(serviceInfo.ActivityInstanceId, serviceInfo.Progress);
                break;
            case 41: //completed
                completeTask(serviceInfo.ActivityInstanceId);
                break;
            case 42: //failed
                failTask(serviceInfo.ActivityInstanceId);
                break;
        }
    };

    var updateServiceStates = function (servicesInfo) {        
        $.each(servicesInfo, updateServiceState);
    };

    var getServiceStates = function () {
        if (!window.jsTasks.length) return;
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + 'Process/ServiceStates',
            data: JSON.stringify(window.jsTasks),
            contentType: 'application/json; charset=utf-8',
            headers: window.AddAntiForgeryToken(),
            success: updateServiceStates,
            error: function () {
                console.error('update progress failed');
            }
        });
    };

    var initialize = function () {
        setInterval(getServiceStates, 2000);
    };

    return {
        //            getServiceStates: getServiceStates,
        initialize: initialize
    };
}();