var FilterManager = function () {
    var toCreationTime, fromCreationTime;
    var altFieldFormatter =
        function (unixDate) {
            var date = new Date(unixDate);
            return date.getFullYear() +
                '/' +
                ("0" + (date.getMonth() + 1)).slice(-2) +
                '/' +
                ("0" + date.getDate()).slice(-2);
        };

    var initializeDatePickers = function () {
        if (!$.fn.pDatepicker) return;
        toCreationTime = $("#to-creation-date").persianDatepicker({
            altField: '#alt-to-creation-date',
            altFieldFormatter: altFieldFormatter,
            initialValue: !!$("#to-creation-date").attr('value'),
            initialValueType: 'gregorian',
            format: 'dddd DD MMMM YYYY',
            calendarType: 'persian',
            calendar: {
                persian: {
                    showHint: true,
                    locale: 'fa'
                },
                gregorian: {
                    showHint: true
                }
            },
            observer: true,
            autoClose: true,
            //                maxDate: new persianDate().unix(),
            onSelect: function (unix) {
                toCreationTime.touched = true;
                if (fromCreationTime && fromCreationTime.options && fromCreationTime.options.maxDate != unix) {
                    var cachedValue = fromCreationTime.getState().selected.unixDate;
                    fromCreationTime.options = { maxDate: unix };
                    if (fromCreationTime.touched) {
                        fromCreationTime.setDate(cachedValue);
                    }
                }
            }
        });
        $("#to-creation-date").on('change', function () {
            var $this = $(this);
            if (!$this.val())
                $('#alt-to-creation-date').val('');
        });
        fromCreationTime = $("#from-creation-date").persianDatepicker({
            altField: '#alt-from-creation-date',
            altFieldFormatter: altFieldFormatter,
            calendarType: 'persian',
            calendar: {
                persian: {
                    showHint: true,
                    locale: 'fa'
                },
                gregorian: {
                    showHint: true
                }
            },
            format: 'dddd DD MMMM YYYY',
            observer: true,
            autoClose: true,
            initialValue: !!$("#from-creation-date").attr('value'),
            initialValueType: 'gregorian',
            //                minDate: new persianDate().unix(),
            onSelect: function (unix) {
                fromCreationTime.touched = true;
                if (toCreationTime && toCreationTime.options && toCreationTime.options.minDate != unix) {
                    var cachedValue = toCreationTime.getState().selected.unixDate;
                    toCreationTime.options = { minDate: unix };
                    if (toCreationTime.touched) {
                        toCreationTime.setDate(cachedValue);
                    }
                }
            }
        });
        $("#from-creation-date").on('change', function () {
            var $this = $(this);
            if (!$this.val())
                $('#alt-from-creation-date').val('');
        });
    };

    var initializeSelectBoxes = function () {
        if (!$('select').select2) return;
        $('select').select2();
    };

    var initialize = function () {
        initializeDatePickers();
        initializeSelectBoxes();
    };

    var loadProcessActivities = function(select) {
        var jsTasks = {};
        jsTasks.versionNo = $(select).find(":selected").attr("versionNo");
        jsTasks.processId = $(select).val();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + 'Process/GetActivityList',
            data: JSON.stringify(jsTasks),
            contentType: 'application/json; charset=utf-8',
            headers: AddAntiForgeryToken(),
            success: function(data) {
                $("select[name='__Activity']")[0].blur();
                $("select[name='__Activity']").html('<option></option>');
                if (data && data.Rows) {
                    var r = data.Rows;
                    for (var i = 0, len = r.length; i < len; i++) {
                        $("select[name='__Activity']")
                            .append('<option value=' + r[i].Ids + '>' + r[i].DisplayValue + '</option>');
                    }
                }
            },
            error: function(xhr) {
                Error(xhr);
                console.log('کاری در فهرست کارها وجود ندارد.');
            }
        });
    };

    var submitWithRestorableTasks = function(val) {
        $('#just-restorable-tasks').val(val);
        window.submitFilter();
    };

    var bringRestorableTasks = function () {
        submitWithRestorableTasks('True');
    };

    var backToNormalTasks = function () {
        submitWithRestorableTasks('False');
    };

    return {
        initialize: initialize,
        initializeDatePickers: initializeDatePickers,
        loadProcessActivities: loadProcessActivities,
        bringRestorableTasks: bringRestorableTasks,
        backToNormalTasks: backToNormalTasks
    };
}();