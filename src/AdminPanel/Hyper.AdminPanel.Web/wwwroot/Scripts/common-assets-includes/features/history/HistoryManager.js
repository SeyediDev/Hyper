var SessionStorageStack = function () {

	var push = function (stackName, item) {
		var existingArr = sessionStorage.getItem(stackName);
		var array = !existingArr ? [] : JSON.parse(existingArr);
		array.push(item);
		sessionStorage.setItem(stackName, JSON.stringify(array));
	};

	var pop = function (stackName) {
		var sessionStorageItem = sessionStorage.getItem(stackName);
		if (!sessionStorageItem) return undefined;
		var array = JSON.parse(sessionStorageItem);
		if (!Array.isArray(array) || array.length === 0)
			return null;
		var poped = array.pop();
		sessionStorage.setItem(stackName, JSON.stringify(array));
		return poped;
	};

	var top = function (stackName) {
		var existingArr = JSON.parse(sessionStorage.getItem(stackName));
		if (!Array.isArray(existingArr) || existingArr.length === 0)
			return null;
		return existingArr[existingArr.length - 1];
	};

	return {
		push: push,
		pop: pop,
		top: top
	};
}();

var HistoryManager = function () {
	var previousPageObj = null;

	var isForm = function (url) {
		return url && url.toLowerCase().indexOf('/form/') !== -1; 
			// && (url.toLowerCase().indexOf('iframe') === -1)
			// && (url.toLowerCase().indexOf('/Index') === -1)
    };

	var formsAreEqual = function (a, b) {
		return a.Main === b.Main &&
			a.queryString.NamespaceId === b.queryString.NamespaceId &&
			a.queryString.EntityId === b.queryString.EntityId &&
			a.queryString.FormId === b.queryString.FormId &&
			a.queryString.ids === b.queryString.ids &&
			a.queryString.__parentNamespaceId === b.queryString.__parentNamespaceId &&
			a.queryString.__parentEntityId === b.queryString.__parentEntityId &&
			a.queryString.__parentFormSubjectId === b.queryString.__parentFormSubjectId &&
			a.queryString.__subTableAssociationFieldId === b.queryString.__subTableAssociationFieldId &&
			a.queryString.__parentIds === b.queryString.__parentIds &&
			a.queryString.pid === b.queryString.pid &&
			a.queryString.wid === b.queryString.wid &&
			a.queryString.TaskId === b.queryString.TaskId &&
			a.queryString.ProcessId === b.queryString.ProcessId &&
			a.queryString.Caller === b.queryString.Caller;
	};

	var pagesAreEqual = function (a, b) {
		return a.Main === b.Main;
	};

	var acquireCurrentFormObject = function () {
		return {
			NamespaceId: window.PageAddressManager.getNamespaceId(),
			EntityId: window.PageAddressManager.getEntityId(),
			FormId: window.PageAddressManager.getPageId(),
			ids: $('input[name="ids"]').val(),
			Id: $('input[name="ids"]').val(),
			__parentNamespaceId: $('input[name="__parentNamespaceId"]').val(),
			__parentEntityId: $('input[name="__parentEntityId"]').val(),
			__parentFormSubjectId: $('input[name="__parentFormSubjectId"]').val(),
			__subTableAssociationFieldId: $('input[name="__subTableAssociationFieldId"]').val(),
			__parentIds: $('input[name="__parentIds"]').val(),
			pid: $('input[name="pid"]').val(),
			wid: $('input[name="wid"]').val(),
			TaskId: $('input[name="TaskId"]').val(),
			ProcessId: $('input[name="ProcessId"]').val(),
			Caller: $('input[name="Caller"]').val()
		};
	};

	var acquireCurrentPageObject = function () {
		var search = location.search.substring(1);
		var obj = {};
		if (!search)
			return obj;
		try {
			obj = JSON.parse('{"' + decodeURI(search).replace(/"/g, '\\"').replace(/&/g, '","').replace(/=/g, '":"') + '"}');
		} finally {
			return obj;
        }
    };

	var perceivePageUrl = function () {
		var currentUrl = window.location.href;
		var mainUrl = currentUrl.split('?');
		var pageObject = {
			Main: mainUrl[0],
			queryString: isForm(currentUrl) ? acquireCurrentFormObject() : acquireCurrentPageObject()
		};
		var previousPage = SessionStorageStack.top(window.name + 'TetaHistory');
		if (previousPage !== null &&
			(
			    (isForm(currentUrl) && formsAreEqual(pageObject, previousPage)) || 
				(!isForm(currentUrl) && pagesAreEqual(pageObject, previousPage))
			)) {
			SessionStorageStack.pop(window.name + 'TetaHistory');
        }
		SessionStorageStack.push(window.name + 'TetaHistory', pageObject);
    };

	var getBackUrl = function () {
		SessionStorageStack.pop(window.name + 'TetaHistory');
		previousPageObj = SessionStorageStack.pop(window.name + 'TetaHistory');
		if (!previousPageObj)
			return null;
		var previousUrl = previousPageObj.Main + '?' + $.param(previousPageObj.queryString);
		return previousUrl;
	};

	var goBack = function (btn) {
		window.goBackCalled = true;
		if (window.LaddaManager)
			window.LaddaManager.startLadda(btn);
		var backUrl = getBackUrl();
		if (!backUrl)
			window.close();
		else
			window.location.href = backUrl;
	};

	var revertBack = function () {
		if (previousPageObj)
			SessionStorageStack.push(window.name + 'TetaHistory', previousPageObj);
		perceivePageUrl();
		if (window.LaddaManager)
			window.LaddaManager.stopLadda();
	};

	return {
		perceivePageUrl: perceivePageUrl,
		getBackUrl: getBackUrl,
		goBack: goBack,
		revertBack: revertBack
	};
}();

$(HistoryManager.perceivePageUrl);