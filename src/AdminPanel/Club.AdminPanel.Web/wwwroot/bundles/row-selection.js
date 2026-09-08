window.TimeSpanManager = function() {
    var ticksToStructuredTime = function(ticks) {
        return {
            days: Math.floor(ticks / (24 * 60 * 60 * 10e6)),
            hours: Math.floor(ticks / (60 * 60 * 10e6)) % 24,
            minutes: Math.floor(ticks / (60 * 10e6)) % 60,
            seconds: Math.floor(ticks / 10e6) % 60,
            milliseconds: ticks % 10e6
        };
    };

    var structureToText = function (timespan, options) {
        options = options || { displayMilliseconds: true };
        return (timespan.days * 24) +
            timespan.hours +
            ":" +
            timespan.minutes +
            ":" +
            timespan.seconds +
            (options.displayMilliseconds && timespan.milliseconds > 0 ?
                "." + timespan.milliseconds : "");
    };

    var toDisplayText = function(ticks, options) {
        var timeSpan = ticksToStructuredTime(ticks);
        return structureToText(timeSpan, options);
    };

    return {
        toDisplayText: toDisplayText,
        ticksToStructuredTime: ticksToStructuredTime
    };
}();
var SummationManager = function() {    
    var sumNumbers = function(numbers) {
        return numbers.reduce(function(sum, i) { return sum + i; }, 0);
    };

    var toDisplayText = function(value, method) {
        switch (method) {
        case 'number':
//            return Math.abs(value - Math.floor(value)) < .001 ? value.toFixed(0) : value.toFixed(2);
            return parseFloat(value.toFixed(2));
        case 'duration':
            return window.TimeSpanManager.toDisplayText(value);
        default:
            throw "Argument out of range method " + method;
        }
    };

//    var getSumInNumber = function(values, method) {
//        switch (method) {
//            case 'number':
//                return sumNumbers(values);
//            case 'duration':
//                return sumDurations(values);
//            default:
//                throw Error("Argument out of range method " + method);
//        }
//    };


    var extractColumnObject = function($th) {
        return {
            name: $th.data('column-name'),
            method: $th.data('summation-method'),
            index: $th.index(),
            sum: 0
        };
    };

    //    example output: {
//        columns: [
//                {
//                    name: 'Duration',
//                    method: 'duration',
//                    index: 4
//                }
//            ],
//            rows: [{ 'Duration': 1000 }]
    //    };
    var extractTableArithmeticInfo = function(getNeededRows) {
        var $table = $('table#tblRecords');
        var info = { columns: [], rows: [] };
        $table.find('th[data-summable]').each(function(columnIdx, th) {
            var $th = $(th);
            var columnObject = extractColumnObject($th);
            info.columns.push(columnObject);
            if (getNeededRows === undefined)
                return;
            $.each(getNeededRows(),
                function (rowIdx, tr) {
                    var $tr = $(tr);
                    var $td = $tr.find('td:nth-child(' + (columnObject.index + 1) + ')'); // considered no colspan
                    var rowObject = (info.rows[rowIdx] || {});
                    rowObject[columnObject.name] = Number($td.data('arithmetic-value'));
                    info.rows[rowIdx] = rowObject;
                });
        });
        return info;
    };

    var getColumnNumbers = function(rows, column) {
        return rows.map(function(row) { return row[column.name]; });
    };

    var sumSelecteds = function(getAlreadySum, getSelectedRows, getPageAlreadySelecteds) {
        var info = extractTableArithmeticInfo(getSelectedRows);
        if (info.columns.length === 0) return info;
        var sumsObject = getAlreadySum();
        var subtractionsInfo = extractTableArithmeticInfo(getPageAlreadySelecteds);
        $.each(info.columns,
            function(idx, column) {
                var otherPagesSum = (sumsObject[column.name] || 0) -
                    sumNumbers(getColumnNumbers(subtractionsInfo.rows, column));
                var numbers = getColumnNumbers(info.rows, column).concat(otherPagesSum);
                var sum = sumNumbers(numbers);
                column.sum = sum;
            });
        return info;
    };

    var getSummaryRow = function () {
        return $('#summary-row');
    }

    var displayResults = function (summationInfo) {
        if (summationInfo.columns.length > 0 &&
            summationInfo.columns.some(function (column) { return column.sum > 0 })) {
            var $summaryRow = getSummaryRow();
            if ($summaryRow.length === 0) {
                var $lastTr = $('table#tblRecords tr:last');
                var columnsCount = $lastTr.find('td').length;
                $summaryRow = $('<tr>', { 'id': 'summary-row', title: 'مجموع انتخاب شده‌ها' });
                $summaryRow.append($('<td><i class="fa fa-plus"></i></td>'));
                $summaryRow.append($('<td>'.repeat(columnsCount - 1)));
                $lastTr.after($summaryRow);
            }
            $.each(summationInfo.columns,
                function (idx, column) {
                    $summaryRow.find('td:nth-child(' + (column.index + 1) + ')')
                        .text(toDisplayText(column.sum, column.method));
                });
        } else {
            getSummaryRow().remove();
        }
    };

    var displaySums = function(getAlreadySum, getSelectedRows, getPageAlreadySelecteds) {
        var summationInfo = sumSelecteds(getAlreadySum, getSelectedRows, getPageAlreadySelecteds);
        displayResults(summationInfo);
    };
    var getSumsObject = function(getAlreadySum, getSelectedRows, getPageAlreadySelecteds) {
        var sumsObject = {};
        $.each(sumSelecteds(getAlreadySum, getSelectedRows, getPageAlreadySelecteds).columns,
            function(idx, column) {
                sumsObject[column.name] = column.sum;
            });
        return sumsObject;
    };

    var loading = function () {
        getSummaryRow().css({ 'opacity': 0.2 });
    };
    var loaded = function () {
        getSummaryRow().css({ 'opacity': 1 });
    };

    var getArithmeticUrl = function () {
        return window.rootUrl + 'Form/DoArithmetic?'
            + window.PageAddressManager.getQueryParameters() + '&'
            + $('#filter-form').serialize();
    }

    var displayAllPagesSum = function () {
        var info = extractTableArithmeticInfo();
        if (info.columns.length === 0) return;
        loading();
        $.get(getArithmeticUrl()).then(function (res) {
                $.each(info.columns, function (idx, column) {
                    column.sum = res[column.name];
                });
                displayResults(info);
                loaded();
            });
    };

    return {
        displaySums: displaySums,
        displayAllPagesSum: displayAllPagesSum,
        getSumsObject: getSumsObject
    };
}();

var SelectionManager = function() {
    var eCheckBoxesState = {
        nothingChecked: 0,
        someChecked: 1,
        thisPageChecked: 2,
        allPagesChecked: 3
    };
    var defaultGetIdentifier = function($row) {
        return $row.data('ids');
    };

    var getIdentifier = defaultGetIdentifier;

    var getSelectingRowsByIdentifiers = function(identifiers) {
        var rows = [];
        $('.selectable-table-row')
            .each(function() {
                if (identifiers.indexOf(getIdentifier($(this))) >= 0) {
                    rows.push(this);
                }
            });
        return rows;
    };

    var getSelectedRows = function() {
        var rows = [];
        $('.row-checkbox:checked').each(function(idx, elem) {
            rows.push(elem.closest('tr'));
        });
        return rows;
    };

    var getAllRows = function() {
        var rows = [];
        $('.row-checkbox').each(function(idx, elem) {
            rows.push(elem.closest('tr'));
        });
        return rows;
    };

    var getSelectedCheckBoxes = function() {
        var rows = [];
        $('.row-checkbox:checked').each(function(idx, elem) {
            rows.push(elem);
        });
        return rows;
    };

    var setFunctionOfGetIdentifier = function(fn) {
        getIdentifier = fn;
    };

    var getEmptySelectedsObject = function() {
        return {
            "records": [],
            "sums": {}
        };
    };
    var getAlreadySelectedsObject = function() {
        var json = $('#alreadySelectedsJson').val();
        if (!json)
            return getEmptySelectedsObject();
        return JSON.parse(json);
    };

    var setHiddenJsonInput = function(obj) {
        $('#alreadySelectedsJson').val(JSON.stringify(obj));
    };

    var findOutCheckBoxesState = function () {
        if ($('ul#pages-info-ul li.page-number.checked-page').length > 0)
            return eCheckBoxesState.allPagesChecked;
        var nothing = true, all = true;
        $('.row-checkbox')
            .each(function () {
                if ($(this).prop('checked')) {
                    nothing = false;
                } else {
                    all = false;
                }
                if (!nothing && !all) return null;
            });
        if (nothing) {
            // todo specific state for other pages checked?
            if (getAlreadySelectedsObject().records.length === 0)
                return eCheckBoxesState.nothingChecked;
        }
        if (all)
            return eCheckBoxesState.thisPageChecked;
        return eCheckBoxesState.someChecked;
    };

    var getAlreadySums = function() {
        return getAlreadySelectedsObject().sums;
    };

    var getSelectingRows = function() {
        return getSelectingRowsByIdentifiers(getAlreadySelectedsObject().records);
    };

//    var getSummableRows = function () {
//        var alreadySelectedRows = getSelectingRows();
//        return getSelectedRows().filter(function (item) { return alreadySelectedRows.indexOf(item) < 0 });
//    };

    var displaySums = function (forAllPages) {
        if (forAllPages)
            SummationManager.displayAllPagesSum();
        else
            SummationManager.displaySums(getAlreadySums, getSelectedRows, getSelectingRows);
    };

    var getSumsObject = function() {
        return SummationManager.getSumsObject(getAlreadySums, getSelectedRows, getSelectingRows);
    };

    var resetAlreadySelections = function() {
        setHiddenJsonInput(getEmptySelectedsObject());
        displaySums();
        $('#reset-alredies').hide();
    };

    var getPageSelectedIdentifiers = function() {
        return getSelectedRows().map($).map(getIdentifier);
    };

    var getPageIdentifiers = function() {
        return getAllRows().map($).map(getIdentifier);
    };

    var getOtherPagesRecords = function(alreadyRecords, allPageRecords) {
        return alreadyRecords.filter(function(item) {
            return allPageRecords.indexOf(item) < 0;
        });
    };

    var mergeRecords = function(alreadyRecords, selectedRecords, allPageRecords) {
        return selectedRecords.concat(getOtherPagesRecords(alreadyRecords, allPageRecords));
    };

    var setThisPageCheckBoxesTo = function(b) {
        $('.row-checkbox')
            .each(function() {
                $(this).prop('checked', b);
            });
    };

    var setAllPagesCheckBoxesTo = function(b) {
        $('#check-all').toggleClass("all-checked");
        $('.row-checkbox').prop("disabled", b);
        $('ul#pages-info-ul li.page-number').toggleClass('checked-page');
    };

    var toggleCheckAllRows = function() {
        var checkBoxesState = findOutCheckBoxesState();
        switch (checkBoxesState) {
        case eCheckBoxesState.nothingChecked:
        case eCheckBoxesState.someChecked:
            setThisPageCheckBoxesTo(true);
            displaySums();
            break;
        case eCheckBoxesState.thisPageChecked:
            setAllPagesCheckBoxesTo(true);
            displaySums(true);
            break;
        case eCheckBoxesState.allPagesChecked:
            setThisPageCheckBoxesTo(false);
            setAllPagesCheckBoxesTo(false);
            displaySums();
            break;
        default:
            console.error('Unexpected checkbox state!', checkBoxesState);
        }
    };

    var getOtherPagesIdentifiers = function() {
        var alreadyObj = getAlreadySelectedsObject();
        var pageIdentifiers = getPageIdentifiers();
        var selectingRows = getSelectingRowsByIdentifiers(getAlreadySelectedsObject().records);
        SummationManager.getSubtractedSelectingRowsSummations(selectingRows);

    };

    var setAlreadySelections = function() {
        var alreadyObj = getAlreadySelectedsObject();
        alreadyObj.records = mergeRecords(alreadyObj.records,
            getPageSelectedIdentifiers(),
            getPageIdentifiers());
        alreadyObj.sums = getSumsObject();
        setHiddenJsonInput(alreadyObj);
    };

    var displayOtherPagesSelectionInfo = function(alreadyRecords) {
        var othersCount = getOtherPagesRecords(alreadyRecords, getPageIdentifiers()).length;
        $('#checkboxes-column').prepend($('<i>',
            {
                'class': "fa fa-exclamation mouse-pointer text-warning",
                'onclick': "SelectionManager.resetAlreadySelections()",
                'id': "reset-alredies",
                'title': othersCount + ' رکورد از صفحات دیگر انتخاب شده است. برای صرف‌نظر از آنها کلیک کنید.'
            }));
    };

    var checkAlreadyCheckboxes = function() {
        var alreadyRecords = getAlreadySelectedsObject().records;
        if (alreadyRecords.length === 0) return;
        displayOtherPagesSelectionInfo(alreadyRecords);
        $.each(getSelectingRowsByIdentifiers(alreadyRecords),
            function(idx, row) {
                $(row).find('.row-checkbox').prop('checked', true);
            });
        displaySums();
    };

    var getSelectedIdentifiers = function() {
        return mergeRecords(getAlreadySelectedsObject().records, getPageSelectedIdentifiers(), getPageIdentifiers());
    };

    

    var handleRowSelection = function(e) {
        var tagName = e.target.tagName.toLowerCase();
        if (tagName !== 'tr' && tagName !== 'td') return;
        var checkbox = $(this).find('.row-checkbox');
        if (checkbox.prop('disabled')) return;
        checkbox.prop('checked') ? checkbox.prop('checked', false) : checkbox.prop('checked', true);
        displaySums();
    };

    $('.selectable-table-row').on('click', handleRowSelection);
    $('.row-checkbox').on('change', displaySums);

    return {
        toggleCheckAllRows: toggleCheckAllRows,
        getSelectedCheckBoxes: getSelectedCheckBoxes,
        getSelectedRows: getSelectedRows,
        findOutCheckBoxesState: findOutCheckBoxesState,
        eCheckBoxesState: eCheckBoxesState,
        setFunctionOfGetIdentifier: setFunctionOfGetIdentifier,
        resetAlreadySelections: resetAlreadySelections,
        setAlreadySelections: setAlreadySelections,
        checkAlreadyCheckboxes: checkAlreadyCheckboxes,
        getSelectedIdentifiers: getSelectedIdentifiers
    };
}();