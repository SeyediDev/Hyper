var pollingInterval = null;
var alreadyIsPolling = false;
var $migrationStartBtn = $('#migration-start-btn').ladda();

function startMigration() {
    if (alreadyIsPolling)
        return;
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + "MigrationActions/Migrate",
        data: $('#options-form').serialize(),
        headers: AddAntiForgeryToken(),
        success: function(data) {
            if (data.WasBusy) {
                stopPolling();
                window.toast.error(window.tetaI18n.t('AnotherSynchronizationGoingOn'), window.tetaI18n.t('Failed'));
                return;
            }
            showMessages(data.Messages);
            try {
                showCommands(data.Commands);
                showErrors(data.Errors);
            } catch (e) {
                console.error(e);
            } finally {
                stopPolling();
            }
            //todo make sure it never gets timed out
        },
        error: function() {
            stopPolling();
            //todo handle intervals and isPolling
            window.toast.error(window.tetaI18n.t('Error Occured'), window.tetaI18n.t('Failed'));
        }
    });

    clearPreviousResults();
    $migrationStartBtn.ladda('start');
    pollingInterval = setInterval(function() {
            pollStatus(showMessages);
        },
        2000);
    alreadyIsPolling = true;
}
function startSetEnumerationItems() {
    if (alreadyIsPolling)
        return;
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + "MigrationActions/SetEnumerationItems",
        data: $('#options-form').serialize(),
        headers: AddAntiForgeryToken(),
        success: function (data) {
            if (data.WasBusy) {
                stopPolling();
                window.toast.error(window.tetaI18n.t('AnotherSynchronizationGoingOn'), window.tetaI18n.t('Failed'));
                return;
            }
            showMessages(data.Messages);
            try {
                showCommands(data.Commands);
                showErrors(data.Errors);
            } catch (e) {
                console.error(e);
            } finally {
                stopPolling();
            }
            //todo make sure it never gets timed out
        },
        error: function () {
            stopPolling();
            //todo handle intervals and isPolling
            window.toast.error(window.tetaI18n.t('Error Occured'), window.tetaI18n.t('Failed'));
        }
    });

    clearPreviousResults();
    $migrationStartBtn.ladda('start');
    pollingInterval = setInterval(function () {
        pollStatus(showMessages);
    },
        2000);
    alreadyIsPolling = true;
}
function stopPolling() {
    if (pollingInterval) {
        $migrationStartBtn.ladda('stop');
        clearInterval(pollingInterval);
        alreadyIsPolling = false;
        HideStopbtn();
    }
}

function pollStatus(useMessagesCallback) {
    $.get(window.top.rootUrl + "MigrationActions/MigrationStatus",
        {},
        function(data) {
            useMessagesCallback(data.Messages);
        }
    );
}

function clearPreviousResults() {
    $('#migration-status').html("");
    $('#migration-commands').html("");
    $('#migration-errors').html("");
}

function showMessages(messages) {
    $('#migration-status').append(messages);
    var scrollElement = document.getElementById("migration-status");
    scrollElement.scrollTop = scrollElement.scrollHeight - scrollElement.clientHeight;
}

function showCommands(t) {
    if (0 !== t.length) {
        var o = "<hr/><h4>" + window.tetaI18n.t("Commands") + '</h4><br/><ul class="list-group">'; t.forEach(function (t) { o += '<li class="list-group-item" dir="ltr">' + t + "<button onclick=\"copyToClipboard(this)\" class=\"copy-btn\" data-text=\"کپی\"><span>کپی</span><svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M16 1H4C2.9 1 2 1.9 2 3V17H4V3H16V1ZM19 5H8C6.9 5 6 5.9 6 7V21C6 22.1 6.9 23 8 23H19C20.1 23 21 22.1 21 21V7C21 5.9 20.1 5 19 5ZM19 21H8V7H19V21Z\" fill=\"white\" /></svg></button><li/>" }), o += "</ul>", $("#migration-commands").append(o)
    }
}
function copyToClipboard(button) {
    // پیدا کردن والد <li>
    var li = button.closest("li");
    if (!li) return "";

    // متن داخل li رو می‌گیریم و متن دکمه رو حذف می‌کنیم
    let text = li.cloneNode(true);
    // حذف دکمه
    let btn = text.querySelector("button");
    if (btn) btn.remove();
    var textToCopy = text.textContent.trim();
    try {
        // استفاده از Clipboard API برای کپی کردن متن
        navigator.clipboard.writeText(textToCopy);

        // نمایش پیام موفقیت
        showNotification('متن با موفقیت کپی شد!');
    } catch (err) {
        // استفاده از روش قدیمی اگر Clipboard API پشتیبانی نشود
        fallbackCopyText(textToCopy);
    }
}
function fallbackCopyText(text) {
    var textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.top = 0;
    textArea.style.left = 0;
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        var successful = document.execCommand('copy');
        if (successful) {
            showNotification('متن با موفقیت کپی شد!');
        } else {
            showNotification('خطا در کپی کردن متن', true);
        }
    } catch (err) {
        showNotification('خطا در کپی کردن متن: ' + err, true);
    }

    document.body.removeChild(textArea);
}
function showNotification(message, isError = false) { if (!isError) alert(message); else showErrors([message]); }

function showErrors(messages) {
    if (messages.length === 0)
        return;
    var html = "<hr/><h4>" + window.tetaI18n.t('ErrorsAndMessages') + "</h4><br/><ul class=\"list-group\">";
    var oldCode = "";
    messages.forEach(function(el) {
        if (oldCode !== el.code) {
            html += "<span>" + el.code + "</span>";
            oldCode = el.code;
        }
        html += "<li class=\"list-group-item list-group-item-" +
            el.className +
            "\"  dir=\"ltr\">" +
            el.comment +
            "</li>";
    });
    html += "</ul>";
    $('#migration-errors').append(html);
}

//	function uncheckDropForeignKeyIndex() {
//		uncheckCheckbox('#DropForeignKeyIndex', '#CreateForeignKeyIndex');
//	}
//	function uncheckCreateForeignKeyIndex() {
//		uncheckCheckbox('#CreateForeignKeyIndex', '#DropForeignKeyIndex');
//	}
function disableCheckBox() {
    dependCheckbox('#DoDropUndefinedTables', '#DoRenameUndefinedTables');
    dependCheckbox('#DropUndefinedViews', '#RenameUndefinedViews');
    dependCheckbox('#DropUndefinedField', '#RenameUndefinedField');
    //		dependCheckbox('#CreateForeignKeyIndex', '#CheckIndexes');
    //		dependCheckbox('#DropForeignKeyIndex', '#CheckIndexes');
    dependCheckbox('#DropExtraIndex', '#CheckIndexes');
    //		dependCheckbox('#RebuildIndex', '#CheckIndexes');
}

function dependCheckbox(depndent, dependedOn) {
    var $dependent = $(depndent);
    var $dependedOn = $(dependedOn);

    if ($dependedOn.prop('checked') === false) {
        $dependent.prop('checked', false);
        $dependent.prop('disabled', true);
    } else {
        $dependent.prop('disabled', false);
    }
}

function uncheckCheckbox(depndent, dependedOn) {
    var $dependent = $(depndent);
    var $dependedOn = $(dependedOn);

    if ($dependedOn.prop('checked') === true) {
        $dependent.prop('checked', false);
    }
}

function ShowStopbtn() {
    var stopbtn = $("#migration-stop-btn");
    var startbtn = $("#migration-start-btn");
    startbtn.removeClass("btn-block").addClass("col-sm-9");
    stopbtn.show();
    //        stopbtn.style.display = "block";
}

function HideStopbtn() {
    var stopbtn = $("#migration-stop-btn");
    var startbtn = $("#migration-start-btn");
    stopbtn.hide();
    startbtn.removeClass("col-sm-9").addClass("btn-block");
    //            if (startbtn.class.contains === "btn-danger") {
    startbtn.removeClass("btn-danger").addClass("btn-primary");
    //        }
}

function Stop() {
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + "MigrationActions/Stop",
        headers: AddAntiForgeryToken(),
    });

    var stopbtn = $("#migration-stop-btn");
    var startbtn = $("#migration-start-btn");
    stopbtn.hide();
    startbtn.removeClass("btn-primary").removeClass("col-sm-9").addClass("btn-danger").addClass("btn-block");
}