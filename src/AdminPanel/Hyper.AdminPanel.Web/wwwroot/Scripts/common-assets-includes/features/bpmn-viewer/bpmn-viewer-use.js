var TetaBpmnViewerManager = function () {
    var tetaBpmnModule = window.TetaBpmnViewerModule;
    var rootUrl = (window.top || {}).rootUrl || '/';
    var loadProcess = function (processId, taskId, versionId) {
        var downloadUrl = rootUrl +
            'Process/DownloadProcess?processId=' +
            processId +
            (versionId ? ('&versionId=' + versionId) : '');
        $.get(downloadUrl,
            function (result) {
                if (result.errors && result.errors.length)
                    console.error(result.errors);
                $('#bpmnModal').find('.modal-title').text(result.processName);
                tetaBpmnModule.openDiagram(result.xml, taskId);
            });
    };
    var openBpmnViewerModal = function (processId, taskId, versionId) {
        $('#bpmnModal').modal('show');
        TetaBpmnViewerManager.loadProcess(processId, taskId, versionId);
    }
    return {
        loadProcess: loadProcess,
        openBpmnViewerModal: openBpmnViewerModal

    }
}();
