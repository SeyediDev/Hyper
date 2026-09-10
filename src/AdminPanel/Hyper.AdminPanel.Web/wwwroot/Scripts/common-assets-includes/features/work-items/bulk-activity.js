var BulkActivityManager = function () {
    var eBulkQueryType = {
        list: 'List',
        query: 'Query'
    };
    
    var goToBulkForm = function (mainPart, queryString) {
        var checkBoxesState = SelectionManager.findOutCheckBoxesState();
        if (checkBoxesState === SelectionManager.eCheckBoxesState.nothingChecked) {
            window.toast.info('لطفا ابتدا آیتم‌هایی را انتخاب نمایید.', '');
            return;
        }

        var serializedForm = $('#filter-form').serialize();
        queryString += serializedForm;

        var queryType;
        if (checkBoxesState === SelectionManager.eCheckBoxesState.allPagesChecked) {
            queryType = eBulkQueryType.query;
        } else {
            queryType = eBulkQueryType.list;

            $.each(SelectionManager.getSelectedIdentifiers(), function (idx, aiId) {                
                queryString += '&aiList=' + aiId;
            });
        }
        
        queryString += '&queryType=' + queryType;

        window.PageAddressManager.navigateTo(mainPart + queryString);
    };

    var goToForm = function (formId, caller) {
        var bulkEditUrl = 'Form/BulkWorkItem';
        var queryString = '?' + 'formId=' + formId + '&caller=' + caller + '&';

        goToBulkForm(bulkEditUrl, queryString);
    };

    return {
        goToForm: goToForm
    };
}();