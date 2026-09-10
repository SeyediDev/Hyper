var RestorationManager = function () {
    var postRestoreTask = function(ai) {
        return $.ajax({
            type: "POST",
            url: window.top.rootUrl + 'Process/RestoreTask',
            data: JSON.stringify(ai),
            contentType: 'application/json; charset=utf-8',
            headers: AddAntiForgeryToken()
        });
    };

    var removeRow = function ($row) {
        $row.fadeTo("slow", 0, function() {
                $(this).remove();
        });
    };

    var createAiAddressingOfRow = function ($row) {        
        var taskAddressing = {};
        taskAddressing.ProcessId = $row.data('processid');
        taskAddressing.ProcessInstanceId = $row.data('piid');
        taskAddressing.TaskId = $row.data('activityid');
        taskAddressing.ActivityInstanceId = $row.data('aiid');
        taskAddressing.ProcessVersion = $row.data('version');
        return taskAddressing;
    }; // todo repeated code with work-assignment.js I'll resolve it if it actually needed this addressing

    var restoreTask = function (buttonElement) {
        var $row = $(buttonElement).closest('tr');
        var ai = createAiAddressingOfRow($row);
        postRestoreTask(ai).then(function() {
            removeRow($row);
        }).catch(function(er) {
            window.toast.error('خطا در بازگرداندن کار', er.responseText);
            console.error(er);
        });
    };
    return {
        restoreTask: restoreTask
    };
}();