// ReSharper disable UseOfImplicitGlobalInFunctionScope
//It depends on ProcessesManager
var TetaBpmnDesignerUtils = function() {
	var getUrlVars = function() {
		var vars = [], pair;
		var pairs = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
		for (var i = 0; i < pairs.length; i++) {
			pair = pairs[i].split('=');
			vars.push(pair[0]);
			vars[pair[0]] = pair[1];
		}
		return vars;
	};

	return {
		getUrlVars: getUrlVars
	};
}();
var TetaBpmnDesignerManager = function() {
	var currentProcessId, currentVersionId;
	var tetaBpmnModule = window.TetaBpmnModule;
	var rootUrl = (window.top || {}).rootUrl || '/';
	var options = {
		onAddNewSubProcess: function(flowNodeId) {
			ProcessesManager.showModal(window.TreeManager.eNodeTypes.Process,
				null,
				true,
				flowNodeId);
		}
	};
	var isProcess = function(element) {
		return element.$type === 'bpmn:Process';
	};

	var openEntityDesignerLink = function() {
		if (!tetaBpmnModule.bpmnModeler) {
			alert('لطفا ابتدا یک فرآیند را انتخاب نمایید.');
			return;
		}
		var processModel = tetaBpmnModule.bpmnModeler._definitions.rootElements.find(isProcess);
		var entityId = processModel.entityId || processModel.$attrs.entityId;
		var namespaceId = processModel.namespaceId || processModel.$attrs.namespaceId;
		if (entityId && namespaceId) {
			var url = rootUrl + 'MetaDesign/App/entity?namespaceId=' + namespaceId + '&entityId=' + entityId;
			window.open(url, '_blank');
		} else {
			alert('لطفا ابتدا مقادیر فضای نامی و موجودیت فرآیند را مقداردهی نمایید.');
		}
	};

	var openProcessLink = function() {
//	    if (!tetaBpmnModule.bpmnModeler) {
//	        alert('لطفا ابتدا یک فرآیند را انتخاب نمایید.');
//	        return;
//	    }
		//	    var processId = tetaBpmnModule.bpmnModeler._definitions.rootElements.find(isProcess).id;
		if (!currentProcessId) {
			alert('لطفا ابتدا یک فرآیند را انتخاب نمایید.');
			return;
		}
		var url =
			rootUrl +
				'Process/WorkItems?__processId=' +
				currentProcessId +
				(currentVersionId ? ('&__versionId=' + currentVersionId) : '');
		window.open(url, '_blank');
	};

	var setGlobalThings = function() {
		$.get(rootUrl + 'MetaDesign/App/api/entities').then(res => {
			window.entities = res;
		});
		$.get(rootUrl + 'Process/AllBusinessRuleList').then(res => {
			window.businessRules = res;
		});
		$.get(rootUrl + 'MetaDesign/App/api/namespaces').then(res => {
			window.namespaces = res.map(n => {
				return {
					name: n.id,
					value: n.id
				}
			});
		});
		$.get(rootUrl + 'MetaDesign/App/api/MessageStructures').then(res => {
			window.structures = (res || []).map(e => {
				return {
					name: e.id,
					value: e.id
				}
			});
		});
		$.get(rootUrl + 'BpmnMessage/List').then(res => {
			window.messages = res.map(n => {
				return {
					name: n.name,
					value: n.id
				}
			});
		});
		$.get(rootUrl + 'BpmnError/List').then(res => {
			window.errors = res.map(n => {
				return {
					name: n.name,
					value: n.id
				}
			});
		});
		$.get(rootUrl + 'BpmnSignal/List').then(res => {
			window.signals = res.map(n => {
				return {
					name: n.name,
					value: n.id
				}
			});
		});
		$.get(rootUrl + 'BpmnEscalate/List').then(res => {
			window.escalations = res.map(n => {
				return {
					name: n.name,
					value: n.id
				}
			});
		});
		$.get(rootUrl + 'BpmnDataStore/List').then(res => {
			window.dataStores = res.map(n => {
				return {
					name: n.name,
					value: n.id
				}
			});
		});
		$.get(rootUrl + 'Process/AllProcessesList').then(res => {
			window.processes = res;
		});
		window.fetchMessageStructure = function(structureId) {
			return $.get(rootUrl + 'MetaDesign/App/api/MessageStructure?entityId=' + structureId);
		};
		window.lockProcessAdministrative = function(processId, versionId, isLock) {
			return $.get(rootUrl +
				'Process/processDesign/ProcessAdministratorLock?processId=' +
				processId +
				'&versionId=' +
				versionId +
				'&isLock=' +
				isLock);
		};
		window.checkInProcess = function(processId, versionId) {
			return $.get(rootUrl +
				'Process/processDesign/ProcessCheckedIn?processId=' +
				processId +
				'&versionId=' +
				versionId +
				'&check=true');
		};
		window.checkOutProcess = function(processId, versionId) {
			return $.get(rootUrl +
				'Process/processDesign/ProcessCheckedIn?processId=' +
				processId +
				'&versionId=' +
				versionId +
				'&check=false');
		};
		window.fetchEntityForms = function(namespaceId, entityId, onlyProcessForms, indexForms) {
			return $.get(rootUrl +
				'MetaDesign/App/api/Forms?namespaceId=' +
				namespaceId +
				'&entityId=' +
				entityId +
				'&onlyProcessForms=' +
				onlyProcessForms +
				'&onlyIndexForms=' +
				indexForms);
		};

		window.fetchEntityFields = function(namespaceId, entityId, onlyAssociations) {
			return $.get(rootUrl +
				'MetaDesign/App/api/EntityFields?namespaceId=' +
				namespaceId +
				'&entityId=' +
				entityId +
				'&onlyAssociations=' +
				onlyAssociations);
		};

		window.fetchEntityStates = function(namespaceId, entityId) {
			return $.get(rootUrl +
				'MetaDesign/App/api/EntityStates?namespaceId=' +
				namespaceId +
				'&entityId=' +
				entityId);
		};

		window.fetchOperation = function(interfaceId) {
			return $.get(rootUrl + 'MetaDesign/App/api/InterfaceOperations?interfaceId=' + interfaceId);
		};


	};

//    var showLinks = function() {
//        $('#links-dropdown').removeClass('hidden');
//    };

	var setHandlers = function() {
		$(".upload").click(function() {
			$(".hide").trigger("click");
		});

		tetaBpmnModule.setImportFileHandler('#btnOpenFileDialog');
		tetaBpmnModule.setExportFileHandlers('#js-download-diagram', '#js-download-svg');

//		$('#loadProcessButton').on('click', loadProcess);
		$('#saveProcessButton').on('click', saveProcess);
//		$('#js-create-diagram').on('click', tetaBpmnModule.createNewDiagram);
		$('#entityDesignerLink').on('click', openEntityDesignerLink);
		$('#processLink').on('click', openProcessLink);
	};

	var getErrorNotifyFunction = function(errorType) {
		if (!window.toast) return alert;
		if (errorType === undefined || errorType < 2) return window.toast.error;
		if (errorType < 5) return window.toast.warning;
		return window.toast.info;
	};

	var getSuccessNotifyFunction = function() {
		return window.toast ? window.toast.success : alert;
	}

	var showErrors = function(errors) {
		for (var i = 0; i < errors.length; i++) {
			var notify = getErrorNotifyFunction(errors[i].Type);
			notify(errors[i].Code + " " + errors[i].Text,
				errors[i].For + " " + errors[i].GeneralText,
				{
					"timeOut": "0",
					"extendedTimeOut": "0"
				});
		}
	};

	var loadProcess = function(processId, versionId) {
		//    var processId = $('#processIdInput').val();
		var downloadUrl = rootUrl +
			'Process/DownloadProcess?processId=' +
			processId +
			(versionId ? ('&versionId=' + versionId) : '');
		$.get(downloadUrl,
			function(result) {
				if (result.errors && result.errors.length) {
					showErrors(result.errors);
				}
				tetaBpmnModule.openDiagram(result.xml, options);
//		    showLinks();
			});
	};

	var loadCurrentProcess = function() {
		var urlVars = TetaBpmnDesignerUtils.getUrlVars();
		currentProcessId = urlVars['ProcessId'];
		currentVersionId = urlVars['VersionId'];
		if (!currentProcessId) return;
		loadProcess(currentProcessId, currentVersionId);
	};
	var setSubProcessId = function(flowNodeId, subProcessId) {
		tetaBpmnModule.setSubProcessId(flowNodeId, subProcessId);
	}
	var addToProcessesList = function(processName, processId) {
		window.processes.push({ name: processName, id: processId });
	}

	function saveProcess() {
		tetaBpmnModule.exportDiagram(function(err, xml) {
			$.ajax({
				type: "POST",
				url: rootUrl +
					'Process/SaveProcess?processId=' +
					currentProcessId +
					(currentVersionId ? ('&versionId=' + currentVersionId) : ''),
				data: {
					strXml: xml
				},
				headers: {
					"__RequestVerificationToken": $('input[name="__RequestVerificationToken').val()
				},
				success: function(result) {
					console.log(result);
					var notifySuccess = getSuccessNotifyFunction();
					var notifyFailure = getErrorNotifyFunction();
					if (result.success)
						notifySuccess('با موفقیت ذخیره شد.');
					else notifyFailure('ذخیره ی ناموفق.');
					if (!result.errors) return;
					showErrors(result.errors);
				}
			});
		});
	}

	return {
		setGlobalThings: setGlobalThings,
		loadCurrentProcess: loadCurrentProcess,
		setHandlers: setHandlers,
		setSubProcessId: setSubProcessId,
		addToProcessesList: addToProcessesList

	}
}();

TetaBpmnDesignerManager.setGlobalThings();
$(function() {
	TetaBpmnDesignerManager.setHandlers();
	TetaBpmnDesignerManager.loadCurrentProcess();
});