var ConfigConfig = function () {
	var constSubmitUri = 'Report/SetConfig';
	var cloneUri = 'Report/CloneConfig';
	var constChangeParentUri = 'Folder/ChangeConfigParent';

	var eConfigAccessLevel = {
		'private': '0',
		'public': '1',
		'userGroup': '2'
	};

	var acquireSubmittableObject = function () {
		var result = {};
		result["NamespaceId"] = window.PageAddressManager.getNamespaceId();
		result["EntityId"] = window.PageAddressManager.getEntityId();
		result["ReportId"] = window.PageAddressManager.getPageId();
		result["ConfigId"] = $('#config-config-modal input[name="ConfiggingConfigId"]').val();
		var acccessLevel = $('#config-config-modal input[name="access"]:checked').val();
		result["IsPublic"] = acccessLevel !== eConfigAccessLevel['private'];
		//       result["FolderId"] = $('#config-config-modal select[name="folder"]').val(); //todo
		result["IsDefault"] = $('#config-config-modal input[name="isdefault"]').is(':checked');
		result["UserGroupId"] = acccessLevel === eConfigAccessLevel['userGroup'] ? $('#config-config-modal select[name="userGroup"]').val() : 0;
		result["Name"] = $('#config-config-modal input[name="alias"]').val();
		return result;
	};

	var showOrHideUserGroupsCombo = function () {
		if ($('#config-config-modal input[name=access][value=' + eConfigAccessLevel['userGroup'] + ']')
			.prop('checked')) {
			$('#config-config-modal .user-group-div').removeClass('hidden');
		} else {
			$('#config-config-modal .user-group-div').addClass('hidden');
		}
	};

	var showModal = function (configId) {
		var $modal = $('#config-config-modal');
		var prevConfig = window.configsJson[configId];

		if (!prevConfig) {
			console.error('Config with id ' + configId + ' not found!');
			return;
		}

		$modal.find('input[name="ConfiggingConfigId"]').val(configId);
		$modal.find('input[name="alias"]').val(prevConfig.alias);
		$modal.find('select[name="folder"]').val(prevConfig.folderId).trigger('change');//trigger change is for select2
		$modal.find('input[name="isdefault"]').prop('checked', prevConfig.isdefault);

		if (window.jsCanPublishConfigs) {
			$modal.find('input[name=access][value=' + prevConfig.access + ']').prop('checked', true);
			$modal.find('select[name="userGroup"]').val(prevConfig.userGroup).trigger('change');
			showOrHideUserGroupsCombo();
		}

		$modal.modal('show');
	};

	var changeParent = function (nodeId, parentId, isFolder) {
		var changeParentModel = {
			newFolderId: parentId,
			configType: window.PageAddressManager.getPageType()
		};
		changeParentModel[isFolder ? 'folderId' : 'configId'] = nodeId;
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + constChangeParentUri,
			data: changeParentModel,
			headers: AddAntiForgeryToken(),
			success: function () {
				console.debug('success');
			},
			error: function () {
				window.toast.error(window.tetaI18n.t('Error Occured'));
			}
		});
	};

	var submitForm = function () {
		var configObject = acquireSubmittableObject();

		$.ajax({
			type: "POST",
			url: window.top.rootUrl + constSubmitUri,
			data: configObject,
			headers: AddAntiForgeryToken(),
			success: function (res) {
				$('#configs-tree').jstree('rename_node', res, configObject.Name);
				$('#config-config-modal').modal('hide');
			},
			error: function () {
				window.toast.error(window.tetaI18n.t('Error Occured'));
			}
		});
		//todo refresh? dismiss modal? tell it saved successfully?
	};
	var clone = function (configId, cb) {
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + cloneUri + '?' + window.PageAddressManager.getQueryParameters(),
			data: {
				configId: configId
			},
			headers: AddAntiForgeryToken(),
			success: function (res) {
				if (cb)
					cb(res);
			},
			error: function () {
				window.toast.error(window.tetaI18n.t('Error Occured'));
			}
		});
	};
	return {
		showModal: showModal,
		changeParent: changeParent,
		submitForm: submitForm,
		showOrHideUserGroupsCombo: showOrHideUserGroupsCombo,
		clone: clone
	};
}();