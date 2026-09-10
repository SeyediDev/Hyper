var ConfigsTreeManager = function () {

	var selectCurrentNode = function (event, data) {
		var configId = $('#ConfigId').val();
		var $configsTree = $('#configs-tree');
		$configsTree.jstree("deselect_all");
		$configsTree.jstree('select_node', configId);
		data.instance._open_to(configId);
	};

    var goToConfig = function (configId) {
        var currentFilterId = FilterConfig.getCurrentPageFilterId();
		window.PageAddressManager.navigateTo('Report?' +
			window.PageAddressManager.getQueryParameters() +
			'&ConfigId=' +
            configId +
            (currentFilterId ? ('&FilterId=' + currentFilterId) : ''));
	};

	var addRemoveToContextMenu = function(items, node) {
		if (node.data.isMeta)
			return;
		items['remove'] = {
			'icon': "fa fa-times",
			'label': window.tetaI18n.t('Remove'),
			'action': function () {
				if (node.type === "default")
					FolderManager.deleteFolder(node.id, function () {
						$('#configs-tree').jstree('delete_node', node.id);
					});
				else
					deleteConfigId(node.id, function () {
						$('#configs-tree').jstree('delete_node', node.id);
					});
			}
		};
	};

	var addCloneToContextMenu = function(items, node) {
		if (node.type !== "default") {
			items['clone'] = {
				'icon': "fa fa-copy",
				'label': window.tetaI18n.t('Copy'),
				'action': function() {
					ConfigConfig.clone(node.id, goToConfig);
				}
			};
		}
	};

	var addSettingsToContextMenu = function(items, node) {
		if (node.data.isMeta)
			return;
		items['settings'] = {
			'icon': "fa fa-cogs",
			'label': window.tetaI18n.t('Settings'),
			'action': function() {
				if (node.type === "default")
					FolderManager.showModal(true,
						node.parent,
						{
							"Id": node.id,
							"Name": node.text,
							"IsForConfig": node.data.isForConfig, //todo
							"IsPublic": node.data.isPublic,
							"ParentFolderId": node.data.parentFolderId
						});
				else
					ConfigConfig.showModal(node.id);
			}
		};
	};

	var newFolder = function (parentId) {
		FolderManager.registerSubmissionCallback(createFolderNode);
		FolderManager.showModal(true, parentId);
	};
	
	var addNewFolderToContextMenu = function(items, node) {
		if (window.jsCanDesign && node.type === "default") {
			items['newFolder'] = {
				'icon': "fa fa-folder-open",
				'label': window.tetaI18n.t('New Folder'),
				'action': function() {
					newFolder(node.id);
				}
			};
		}
	};

	var contextMenu = function (node) {
		var items = {};
		addNewFolderToContextMenu(items, node);
		if (window.jsCanDesign && (!node.data.isPublic || window.jsCanPublishConfigs)) {
			addSettingsToContextMenu(items, node);
			addCloneToContextMenu(items, node);
			addRemoveToContextMenu(items, node);
		}
		return items;
	};

	var types = {
		"default": { //folder
			"icon": window.top.rootUrl + "Content/common-assets-includes/icons/folder.svg",
			"valid_children": ["default", "Chart", "ReportList", "GroupByList"]
		},
		"Chart": {
			"icon": "fa fa-bar-chart",
			"valid_children": []
		},
		"ReportList": {
			"icon": "fa fa-list",
			"valid_children": []
		},
		"GroupByList": {
			"icon": "fa fa-table",
			"valid_children": []
		}
	};

	var nodeMoved = function (e, data) {
		if (data['old_parent'] !== data['parent'])
			ConfigConfig.changeParent(data.node.id, data.parent, data.node.type === 'default');
		//            console.warn('Must save ' + data.node.id + '\'s parent as ' + data['parent']);
	};

	var instantiateTree = function () {
		$('#configs-tree')
			.jstree({
				'core': {
					'data': window.configsTree,
					"check_callback": true
				},
				"contextmenu": {
					"select_node": false,
					'items': contextMenu
				},
				"types": types,
				"state": {
					"key": 'configs-' + window.PageAddressManager.getPageId()
				},
				"search": {
					"fuzzy": true,
					"show_only_matches": true,
					"show_only_matches_children": true
				},
				"conditionalselect": function (node, event) {
					if (node.type === 'default')
						return false;
					goToConfig(node.id);
					return false;
				},
				"plugins": [
					"dnd", "contextmenu", "types", "conditionalselect", "wholerow",
					"search", "state"
				]
			})
			.on('move_node.jstree', nodeMoved)
			.on('ready.jstree', selectCurrentNode)
			.on('search.jstree before_open.jstree', function (e, data) {
				if (data.instance.settings.search.show_only_matches) {
					data.instance._data.search.dom.find('.jstree-node')
						.show().filter('.jstree-last').filter(function () {
							return this.nextSibling;
						}).removeClass('jstree-last')
						.end().end().end().find(".jstree-children").each(function () {
							$(this).children(".jstree-node:visible").eq(-1).addClass("jstree-last");
						});
				}
			})
			.off('keydown');
		$("#configs-search-input").keyup(function () {
			$('#configs-tree').jstree('search', $(this).val());
		});
	};

	var createFolderNode = function (nodeId, parentNodeId, text, isUpdate) {
		if (isUpdate) {
			$('#configs-tree').jstree('rename_node', nodeId, text);
			//todo data should also change
		}
		else
			$('#configs-tree')
				.jstree('create_node',
					parentNodeId ? parentNodeId.toString() : '#',
					{
						"id": nodeId,
						"text": text,
						//				    "type": "default",
						"data": {
							"isPublic": /*parentNode ? parentNode.data.isPublic :*/ false,
							"parentFolderId": parentNodeId ? parentNodeId : null
						}
					},
					"first");
	};

	$(instantiateTree);
	return {
		newFolder: newFolder,
		goToConfig: goToConfig,
		getContextMenu: contextMenu
	};
}();
