(function () {

    var disableStickyOnEdge = function () {
        if (/Edge\/\d./i.test(navigator.userAgent)) {
            $('.t-table-wrapper .sticky-row th').css({ position: 'relative' });
        }
    };
    $(disableStickyOnEdge);

    var rootUrl = $('meta[name="root"]').prop("content");
    if (!rootUrl)
        return;
    if (rootUrl.slice(-1) !== "/") {
        rootUrl += "/";
    }
    window.rootUrl = rootUrl;
})();

window.handleAvatarError = function(img) {
	 var $img = $(img);
	 if ($img.attr('src').indexOf('login.png') === -1)
		 $img.attr('src', window.top.rootUrl + 'Content/common-assets-includes/images/global/login.png');
}

window.MainPartLoadingManager = function() {
    var shouldWait = false;

    var showMain = function() {
        $("#loadingPart").addClass("hidden");
        $("#mainPartOfTheForm").removeClass("hidden");
    };

    var showLoader = function() {
        $("#loadingPart").removeClass("hidden");
    };

    var setShouldWait = function(b) {
        shouldWait = b;
    };

    var showMainIfReady = function() {
        if (!shouldWait)
            showMain();
    }

    return {
        showMain: showMain,
        showLoader: showLoader,
        setShouldWait: setShouldWait,
        showMainIfReady: showMainIfReady
    };
}();

$(document).ready(function() {
    $(function() {
        window.MainPartLoadingManager.showMainIfReady();
        if (typeof window.instantiateMenu === 'function')
		    window.instantiateMenu();
    });
});

window.PageAddressManager = function () {
    _address = {};

    var checkAddressType = function (address) {
        var mandatoryFields = ['namespaceId', 'entityId', 'pageType', 'pageId'];
        for (var i = 0; i < mandatoryFields.length; i++) {
            if (!address[mandatoryFields[i]])
                throw new Error('No ' + mandatoryFields[i] + ' in address');
        }
    }

    var setPageAddress = function(address) {
        checkAddressType(address);
        _address = address;
    };

    var getNamespaceId = function() {
        return _address.namespaceId ||
            $('input[name="NamespaceId"]').val();
    };
    var getEntityId = function() {
        return _address.entityId ||
            $('input[name="EntityId"]').val();
    };
    var getFormSubjectId = function () {
        return _address.formSubjectId || $('input[name="FormSubjectId"]').val();
    };
    var getPageId = function () {
        return _address.pageId ||
            $('input[name="FormId"]').val() ||
            $('input[name="ReportId"]').val() ||
            $('input[name="DashboardId"]').val();
    };
    var getPageType = function() {
        return _address.pageType;
    };

    var getPageParameters = function() {
        switch (getPageType()) {
            case 'Form':
                return 'FormId=' + getPageId() + '&FormSubjectId=' + getFormSubjectId();
            case 'Report':
                return 'ReportId=' + getPageId();
            case 'Dashboard':
                return 'DashboardId=' + getPageId();
        }
        throw new Error('Page type not identified: ' + getPageType());
    }

    var getQueryParameters = function () {
        return 'NamespaceId=' +
            getNamespaceId() +
            '&EntityId=' +
            getEntityId() +
            '&' +
            getPageParameters();
	 };

    var getPageCode = function () {
        return getNamespaceId() +
            '-' +
            getEntityId() +
            '-' +
            getPageId();
	 };

    var navigateTo = function(url, isAlreadyAbsolute, newTab) {
		 var absoluteUrl = isAlreadyAbsolute ? url : window.top.rootUrl + url;
        if (newTab || (window.event && window.event.ctrlKey))
		    window.open(absoluteUrl, '_blank', 'noopener');
	    else
		    location.href = absoluteUrl;
    };

    var getFormUrl = function (address) {
        return 'Form' + (address.type ? ('/' + address.type) : '') + '?' +
            'NamespaceId=' +
            address.namespace  +
            '&EntityId=' +
            address.entity +
            (address.formId ? ('&FormId=' + address.formId) : '') +
            (address.ids ? ('&Ids=' + address.ids) : '') +
            (address.subject ? ('&FormSubjectId=' + address.subject) : '');
    };
    
    var navigateToForm = function (address, newTab) {
        var url = getFormUrl(address);
        navigateTo(url, false, newTab);
    };

    return {
        setPageAddress: setPageAddress,
        getNamespaceId: getNamespaceId,
        getEntityId: getEntityId,
        getFormSubjectId: getFormSubjectId,
        getPageId: getPageId,
        getPageType: getPageType,
        getQueryParameters: getQueryParameters,
        getPageCode: getPageCode,
        
        getFormUrl: getFormUrl,
        navigateTo: navigateTo,
        navigateToForm: navigateToForm
    };
}();

function checkboxChanged(element) {
	var $element = $(element);
	$element.val(element.checked);
	$element.attr("value", element.checked ? "true" : "false");
	var $hiddenField = $("input[name='" + $element.attr('associated-hidden-name') + "']");
	$hiddenField.val(element.checked ? true : false);
	$hiddenField.attr("value", element.checked ? true : false);
	$hiddenField.trigger('change');
}
