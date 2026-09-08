window.toggleDesignMode = function () {
    $(".designTool").toggleClass("designMode");
};

window.ExcelManager = function (i18n) {

    var $importSaveButton;

    var eExportType = {
        Create: 0,
        Edit: 1,
        Delete: 2
    };
    var eImportType = {
        Create: 0,
        Edit: 1,
        Delete: 2
    };

    var downloadExportFile = function () {
        window.location = window.top.rootUrl +
            'Form/ExportToExcel?' +
            $('#filter-form').serialize() +
            '&' +
            window.PageAddressManager.getQueryParameters();
    };

    var downloadTemplate = function () {
        window.location = window.top.rootUrl +
            'Form/GenerateExcelTemplate?' +
            $('#filter-form').serialize() +
            '&' +
            window.PageAddressManager.getQueryParameters() +
            "&exportType=" + eImportType.Create;
    };
    var downloadTemplateWithData = function () {
	    window.location = window.top.rootUrl +
		    'Form/GenerateExcelTemplate?' +
		    $('#filter-form').serialize() +
		    '&' +
		    window.PageAddressManager.getQueryParameters() +
		    "&exportType=" + eImportType.Edit;
    };
// #region	operation result
    var displayOverviewResults = function (overviewResults) {
        return $('<div/>')
            .append($('</h6>', {text: i18n.t('OverviewResultsTitle')}))
            .append($.map(overviewResults, function (sheetInfo) {
                var $sheet = $('<div/>', {
                    'text': sheetInfo.SheetName,
                    'class': 'd-inline-block border border-success p-1'
                }).append($('<span/>', {
                    'class': 'badge badge-primary m-1',
                    'title': i18n.t('RowsCount'),
                    'text': sheetInfo.TotalCount
                }));
                if (sheetInfo.ErrorCount > 0) {
                    $sheet.append($('<span/>',
                        {
                            'class': 'badge badge-danger m-1',
                            'title': i18n.t('ErrorsCount'),
                            'text': sheetInfo.ErrorCount
                        }));
                }
                return $sheet;
            }));
    };

    var displayReadErrors = function (readErrors) {
        if ((readErrors || []).length === 0) return null;
//		return $('<table/>', { 'class': 'table' }).append(
//			$('<thead><tr>' +
//					'<th>' + i18n.t('') + '</th>' +
//				'<th>' + i18n.t('') + '</th>' +
//                '</tr></thead>')
//		);
        return $('<div/>')
            .append($('</h6>', {text: i18n.t('ReadErrors')}))
            .append($.map(readErrors, function (error) {
                return $('<p/>',
                    {
                        'text': i18n.t('ReadErrorTemplate', error.Row, error.HeaderName, error.SheetName, error.ErrorText),
                        'class': 'text-danger'
                    });
            }));
    };

    var displayApplyErrors = function (applyErrors) {
        if ((applyErrors || []).length === 0) return null;
        return $('<div/>')
            .append($('</h6>', {text: i18n.t('ApplyErrors')}))
            .append($.map(applyErrors, function (error) {
                return $('<p/>',
                    {
                        'text': error.Message,
                        'class': 'text-danger'
                    });
            }));
    };
    var displayFatalError = function (error) {
        if (!error) return null;
        return $('<p/>', {'text': error, 'class': 'text-danger'});
    };

    var displayOperationResult = function (result) {
        var $fatalError = displayFatalError(result.Fatal);
        var $overviewResults = displayOverviewResults(result.OverviewResultPerSheet);
        var $readErrors = displayReadErrors(result.ReadErrorList);
        var $applyErrors = displayApplyErrors(result.ApplyErrors);
        $('#operation-result')
            .html('')
            .append($fatalError)
            .append($overviewResults)
            .append($readErrors)
            .append($applyErrors);
    };

// #endregion
    var importFile = function () {
        $importSaveButton.ladda('start');

        var formData = new FormData();
        var fileInput = document.getElementById('excel-import-file');
        if (!fileInput.files || fileInput.files.length === 0) {
            window.toast.warning('لطفا ابتدا فایل را انتخاب نمایید.');
            $importSaveButton.ladda('stop');
            return;
        }
        formData.append(fileInput.files[0].name, fileInput.files[0]);
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + 'Form/BulkImportFromExcelFile?' + window.PageAddressManager.getQueryParameters(),
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            headers: {
                "__RequestVerificationToken": $('input[name="__RequestVerificationToken').val()
            },
            success: function (result) {
                $importSaveButton.ladda('stop');
                console.log(result);
                displayOperationResult(result);
            },
            error: function (jqXhr) {
                $importSaveButton.ladda('stop');
                window.toast.error(jqXhr.responseText);
            }
        });
    };

    var showImportModal = function () {
        $("#excel-import-modal").modal('show');
    };

    var clearImportModal = function () {
        $('#operation-result').html('');
        var eif = document.getElementById('excel-import-file');
        eif.value = '';
        eif.dispatchEvent(new Event('change'));
    };

    var initModal = function () {
        $('#excel-import-modal').on('hidden.bs.modal', clearImportModal);
        window.bsCustomFileInput.init();
        $importSaveButton = $('#import-save-button').ladda();
    }

    $(function () {
        initModal();
    });

    return {
        showImportModal: showImportModal,
        downloadExportFile: downloadExportFile,
        importFile: importFile,
        downloadTemplate: downloadTemplate,
        downloadTemplateWithData: downloadTemplateWithData
    };
}(window.tetaI18n);

window.BulkOperationsManager = function () {
    var eBulkQueryType = {
        list: 'List',
        query: 'Query'
    };

    var readIndexCharacteristics = function () {
        return {
            namespaceId: window.PageAddressManager.getNamespaceId(),
            entityId: window.PageAddressManager.getEntityId(),
            indexFormId: window.PageAddressManager.getPageId(),
            formSubjectId: window.PageAddressManager.getFormSubjectId(),
            workItemFormId: "",
            wid: "",
            TaskId: "",
            ProcessId: "",
            Caller: "",
            indexFormPageNo: $('input[name="Page"]').val()
        };
    };

    var goToBulkForm = function (mainPart, queryString) {
        var checkBoxesState = SelectionManager.findOutCheckBoxesState();
        if (checkBoxesState === SelectionManager.eCheckBoxesState.nothingChecked) {
            window.toast.info('لطفا ابتدا آیتم‌هایی را انتخاب نمایید.', '');
            return;
        }

        if (checkBoxesState === SelectionManager.eCheckBoxesState.allPagesChecked) {
            var serializedForm = $('#filter-form').serialize();
            queryString += $.param(readIndexCharacteristics());
            queryString += '&queryType=' + eBulkQueryType.query + '&';
            queryString += serializedForm;
        } else {
            $.each(SelectionManager.getSelectedIdentifiers(),
                function (idx, identifier) {
                    queryString += 'recordsIds=' + identifier + '&';
                });

            queryString += $.param(readIndexCharacteristics());
            queryString += '&queryType=' + eBulkQueryType.list;
        }

        window.PageAddressManager.navigateTo(mainPart + queryString);
    };

    var bulkEdit = function (formId) {
        var bulkEditUrl = 'Form/BulkEdit';
        var queryString = '?' + 'FormId=' + formId + '&';

        goToBulkForm(bulkEditUrl, queryString);
    };

    var bulkCreateProcess = function (namespaceId,
                                      entityId,
                                      formId,
                                      taskId,
                                      processId,
                                      processVersion,
                                      associationFieldId) {
        var bulkCreateProcessUrl = 'Form/BulkCreateProcess';
        var queryString = '?' +
            'NamespaceId=' +
            namespaceId +
            '&EntityId=' +
            entityId +
            '&associationFieldId=' +
            associationFieldId +
            '&bulkFormId=' +
            formId +
            '&indexFormId=' +
            window.PageAddressManager.getPageId() +
            '&TaskId=' +
            taskId +
            '&ProcessId=' +
            processId +
            '&ProcessVersion=' +
            processVersion +
            '&';

        goToBulkForm(bulkCreateProcessUrl, queryString);
    };
    return {
        bulkEdit: bulkEdit,
        bulkCreateProcess: bulkCreateProcess
    };
}();
