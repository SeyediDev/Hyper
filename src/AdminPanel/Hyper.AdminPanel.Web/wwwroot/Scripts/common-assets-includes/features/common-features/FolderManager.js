var FolderManager = function() {
    var constSubmitUri = 'Folder/SaveFolderConfig';
    var constDeleteUri = 'Folder/DeleteFolderConfig';

    var fillTheForm = function (data) {
        var $configFolderModal = $("#new-folder-modal");
        $configFolderModal.find('#config-folder-id').val(data.Id);
        $configFolderModal.find('#config-folder-parent-id').val(data.ParentFolderId);
        $configFolderModal.find('#folder-isForConfig').val(Boolean(data.IsForConfig));
        $configFolderModal.find('input[name="newFolderName"]').val(data.Name);
        if ($configFolderModal.find('input[name=newFolderAccess]').length > 0)
            $configFolderModal.find('input[name=newFolderAccess][value="' + data.IsPublic + '"]').prop('checked', true);
    };

    var acquireSubmitDataObj = function () {
        var result = {};
        result["NamespaceId"] = window.PageAddressManager.getNamespaceId();
        result["EntityId"] = window.PageAddressManager.getEntityId();
        var pageType = window.PageAddressManager.getPageType();
        switch (pageType) {
            case "Form":
                result["FormId"] = window.PageAddressManager.getPageId();
                break;
            case "Report":
                result["ReportId"] = window.PageAddressManager.getPageId();
                break;
            case "Dashboard":
                result["DashboardId"] = window.PageAddressManager.getPageId();
                break;
        }
        result["FolderId"] = $('#new-folder-modal #config-folder-id').val();
        result["Name"] = $('#new-folder-modal input[name="newFolderName"]').val();
        //        result.Id = $('#new-folder-modal #config-folder-id').val();
        result["ParentFolderId"] = $('#new-folder-modal #config-folder-parent-id').val();
        if (!result["ParentFolderId"]) result["ParentFolderId"] = null;
        result["IsPublic"] = $('#new-folder-modal input[name="newFolderAccess"]:checked').val() === 'true';
        result["IsForConfig"] = $('#new-folder-modal #folder-isForConfig').val();
        return result;
    };

    var showModal = function (isForConfig, parentId, folderInfo) {
        var $modal = $('#new-folder-modal');

        if (folderInfo === undefined) {
            folderInfo = {
                "Id": 0,
                "Name": window.tetaI18n.t('New Folder'),
                "IsForConfig": isForConfig,
                "IsPublic": false,
                "ParentFolderId": parentId ? parentId : null
            };
        }
        fillTheForm(folderInfo);

        $modal.modal('show');
    };

    var submissionCallback = null;

    var registerSubmissionCallback = function (cb) {
        if (submissionCallback !== null && cb !== submissionCallback)
            throw new Error('Concurrent folder submission is not supported');
        submissionCallback = cb;
    };

    var submitForm = function () {
        var saveFolderConfigObj = acquireSubmitDataObj();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + constSubmitUri,
            data: saveFolderConfigObj,
            headers: AddAntiForgeryToken(),
            success: function (res) {
                submissionCallback(res.Id, res.ParentFolderId, res.Name, saveFolderConfigObj.FolderId === res.Id);
                submissionCallback = null;
                $('#new-folder-modal').modal('hide');
            },
            error: function (res) {
                console.error(res);
                window.toast.error(window.tetaI18n.t('Error Occured'));
                $('#new-folder-modal').modal('hide');
            }
        });
    };    

    var deleteFolder = function (folderId, cb) {
        var result = confirm(window.tetaI18n.t('SureToDeleteFolder'));
        if (!result) return;
        $.ajax({
            type: "POST",
            url: window.top.rootUrl +
                constDeleteUri +
                '?folderId=' +
                folderId,
            success: function () {
                if (cb)
                    cb();
            },
            headers: AddAntiForgeryToken()
        });
    };

    return {
        showModal: showModal,
        submitForm: submitForm,
        deleteFolder: deleteFolder,
        registerSubmissionCallback: registerSubmissionCallback
    };
}();
