//It depends on TetaBpmnDesignerManager

var ProcessesManager = {
	ConstGetUri: 'Tree/ProcessFramework',
	ConstSaveUri: 'Tree/Save',
	ConstDeleteUri: 'Tree/Delete',
	ConstDragUri: 'Tree/Drag',
	isOpen: true,

	addingSubprocessState: {
		flowNodeId: '',
		isSubProcess: false
	},

	processId: function () {
		var url = window.location.href;
		if (url.indexOf('/Design') !== -1)
			return url.substring(url.indexOf('nodeId=') + 'nodeId='.length);
		return undefined;
	},

	getTree: function () {
		return $.get('/' + this.ConstGetUri);
	},

	createNode: function (formData, id, isUpdate) {
		var $tree = $("#process-tree");
		if (isUpdate) {
			$tree.jstree('rename_node', id, formData.Name);
		} else {
			var nodes = $tree.jstree(true).get_json($tree, { 'flat': true });
			for (var i = 0; i < nodes.length; i++) {
				if (nodes[i]["id"] === formData.Id) return;
			}
			$tree.jstree('create_node',
				formData.ParentId,
				{
					"id": id,
					"parent": formData.ParentId,
					"text": formData.Name,
					"type": parseInt(formData.NodeTypeId),
					"data": {
						"orderId": formData.OrderId,
						"code": formData.StandardCode
						//,"businessRuleType": formData.BusinessRuleType
					}
				});
			if (formData.NodeTypeId == TreeManager.eNodeTypes.Process) {
				$tree.jstree('create_node',
					id,
					{
						"id": Math.random().toString(36).substring(7),
						"parent": id,
						"text": 'version 1.0',
						"type": TreeManager.eNodeTypes.ProcessVersion,
						"data": {
							"orderId": formData.OrderId,
							"code": '1.0'
						}
					});
			}
			if (formData.NodeTypeId == TreeManager.eNodeTypes.BusinessRule) {
				$tree.jstree('create_node',
					id,
					{
						"id": Math.random().toString(36).substring(7),
						"parent": id,
						"text": 'version 1.0',
						"type": TreeManager.eNodeTypes.BusinessRuleVersion,
						"data": {
							"orderId": formData.OrderId,
							"code": '1.0'
						}
					});
			}
		}
		$tree.jstree("open_all");
	},

	deleteProcess: function (id, cb) {
		var result = confirm("آیا از حذف این فرآیند مطمئنید؟");
		if (!result) return;
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + this.ConstDeleteUri + '?Id=' + id,
			success: function () {
				if (cb)
					cb();
			},
			headers: window.AddAntiForgeryToken()
		});
	},

	submitForm: function (modalId) {
		var $modal = $(modalId);
		var processData;
		if (modalId === '#process-modal')
			processData = ProcessesManager.acquireFormData();
		if (modalId === '#business-rule-modal')
			processData = ProcessesManager.acquireFormDataBusinessRule();
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + this.ConstSaveUri,
			data: processData,
			headers: window.AddAntiForgeryToken(),
			success: function (generatedId) {
				ProcessesManager.createNode(processData, generatedId, generatedId === processData.Id);
				if (processData.NodeTypeId === TreeManager.eNodeTypes.Process) {
					if (!ProcessesManager.addingSubprocessState.isSubProcess) {
						window.location.href = window.top.rootUrl +
							'Process/Design?ProcessId=' +
							processData.StandardCode +
							'&VersionId=1.0';
					} else {
						window.TetaBpmnDesignerManager.addToProcessesList(processData.Name, processData.StandardCode);
						window.TetaBpmnDesignerManager.setSubProcessId(
							ProcessesManager.addingSubprocessState.flowNodeId,
							processData.StandardCode);
						ProcessesManager.addingSubprocessState.isSubProcess = false;
						ProcessesManager.addingSubprocessState.flowNodeId = null;
						window.open(window.top.rootUrl +
							'Process/Design?ProcessId=' +
							processData.StandardCode +
							'&VersionId=1.0',
							'_blank');
					}
				}
				// window.location.reload();

				$modal.modal('hide');

			},
			error: function (err) {
				//alert(err.responseText);
				window.toast.error(err.responseText);
				//$modal.modal('hide');
			}
		});
	},

	acquireFormData: function () {
		var $modal = $("#process-modal");
		var result = {};

		result.Id = $modal.find('#process-id').val();
		result.NodeTypeId = $modal.find('#type').val();
		result.StandardCode = $modal.find('#standard-code').val();
		result.Name = result.NodeTypeId == 4 ? 'Version ' + result.StandardCode : $modal.find('#process-name').val();
		result.ParentId = $modal.find('#parent-id').val();
		result.OrderId = $modal.find('#order-id').val();
		result.Purpose = $modal.find('#business-rule-purpose').val();
		result.Description = $modal.find('#business-rule-description').val();
		return result;
	},
	acquireFormDataBusinessRule: function () {
		var $modal = $("#business-rule-modal");
		var result = {};

		result.Id = $modal.find('#business-rule-id').val();
		result.NodeTypeId = $modal.find('#business-rule-node-type').val();
		result.StandardCode = $modal.find('#business-rule-standard-code').val();
		result.Name = $modal.find('#business-rule-name').val();
		result.ParentId = $modal.find('#business-rule-parent-id').val();
		result.OrderId = $modal.find('#business-rule-order-id').val();
		result.Purpose = $modal.find('#business-rule-purpose').val();
		result.Description = $modal.find('#business-rule-description').val();
		result.BusinessRuleTypeId = $modal.find('#business-rule-type').val();
		return result;
	},

	goToBpmnEditor: function (node) {
		var parent = TreeManager.getNodeById(node.parent);

		window.top.PageAddressManager.navigateTo(
			'Process/BpmnDesign?ProcessId=' +
			parent.data.code +
			'&VersionId=' +
			node.data.code +
			'&nodeId=' +
			node.id);
	},
	goToBusinessRuleEditor: function (node) {
		var parent = TreeManager.getNodeById(node.parent);
		window.top.PageAddressManager.navigateTo(
			'Process/BusinessRuleDesign?businessRuleCode=' +
			parent.data.code +
			'&versionId=' +
			node.data.code +
			'&nodeId=' +
			node.id);
	},

	fillTheForm: function (data) {
		var $modal = $("#process-modal");
		$modal.find('#process-id').val(data.Id);
		$modal.find('#process-name').val(data.Name);
		$modal.find('#parent-id').val(data.ParentId);
		$modal.find('#type').val(data.NodeTypeId);
		$modal.find('#order-id').val(data.OrderId);
		$modal.find('#standard-code').val(data.StandardCode);
	},

	fillTheFormBusinessRule: function (data) {
		var $modal = $("#business-rule-modal");
		$modal.find('#business-rule-id').val(data.Id);
		$modal.find('#business-rule-name').val(data.Name);
		$modal.find('#business-rule-parent-id').val(data.ParentId);
		$modal.find('#business-rule-node-type').val(data.NodeTypeId);
		$modal.find('#business-rule-order-id').val(data.OrderId);
		$modal.find('#business-rule-standard-code').val(data.StandardCode);
	},
	settingModal: function (node) {
		var $modal = $("#process-modal");
		var data = {
			'Id': node.id,
			'Name': node.text,
			'ParentId': node.parent,
			'NodeTypeId': node.type,
			'StandardCode': node.data.code
		};

		(node.type === TreeManager.eNodeTypes.ProcessVersion ? $("#process-name-div").hide() : $("#process-name-div").show());
		(node.type === TreeManager.eNodeTypes.ProcessVersion || node.type === TreeManager.eNodeTypes.Process ? $("#node-type-div").hide() : $("#node-type-div").show());

		ProcessesManager.fillTheForm(data);
		$modal.modal('show');
	},

	showModal: function (type, parentId, isSubProcess, flowNodeId) {
		ProcessesManager.addingSubprocessState.isSubProcess = isSubProcess;
		ProcessesManager.addingSubprocessState.flowNodeId = flowNodeId;
		if (isSubProcess) {
			parentId = TreeManager.getGrandParentIdOfSelectedNode();
		}
		var $modal = $("#process-modal");
		var data = {
			'Id': 0,
			'Name': '',
			'ParentId': parentId,
			'NodeTypeId': type,
			'StandardCode': ''
		};

		(type === TreeManager.eNodeTypes.ProcessVersion ? $("#process-name-div").hide() : $("#process-name-div").show());
		(type === TreeManager.eNodeTypes.ProcessVersion || type === TreeManager.eNodeTypes.Process ? $("#node-type-div").hide() : $("#node-type-div").show());

		ProcessesManager.fillTheForm(data);
		$modal.modal('show');
	},

	showBusinessRuleModal: function (type, parentId, isSubProcess, flowNodeId) {
		ProcessesManager.addingSubprocessState.isSubProcess = isSubProcess;
		ProcessesManager.addingSubprocessState.flowNodeId = flowNodeId;
		if (isSubProcess) {
			parentId = TreeManager.getGrandParentIdOfSelectedNode();
		}
		var $modal = $("#business-rule-modal");
		var data = {
			'Id': 0,
			'Name': TreeManager.getNodeName(parentId),
			'ParentId': parentId,
			'NodeTypeId': type,
			'StandardCode': ''
		};
		(type === TreeManager.eNodeTypes.BusinessRuleVersion ? $("#business-rule-name-div").hide() : $("#business-rule-name-div").show());
		(type === TreeManager.eNodeTypes.BusinessRuleVersion ? $("#business-rule-purpose-div").hide() : $("#business-rule-purpose-div").show());
		(type === TreeManager.eNodeTypes.BusinessRuleVersion ? $("#business-rule-description-div").hide() : $("#business-rule-description-div").show());

		ProcessesManager.fillTheFormBusinessRule(data);
		$modal.modal('show');
	},

	nodeMoved: function (e, data) {
		var $tree = $("#process-tree");
		var changeParent = {
			nodeTypeId: data.node.type,
			id: data.node.id,
			parentId: data.node.parent
		};
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + ProcessesManager.ConstDragUri,
			data: changeParent,
			headers: window.AddAntiForgeryToken(),
			success: function (res) {
				if (res === 0)
					alert('0');

				$('#process-tree').jstree("open_all");
			},
			error: function () {
				alert('خطا');
			}
		});
	},

	closeAllNodes: function () {
		if (ProcessesManager.isOpen) {
			$('#process-tree').jstree("close_all");
			ProcessesManager.isOpen = !ProcessesManager.isOpen;
		} else {
			$('#process-tree').jstree("open_all");
			ProcessesManager.isOpen = !ProcessesManager.isOpen;
		}
	}
};

var TreeManager = function () {
	var constTreeSelector = '#process-tree';

	var eNodeTypes = {
		'ProcessCategory': 1,
		'ProcessGroup': 2,
		'Process': 3,
		'ProcessVersion': 4,
		'MainProcess': 15,
		'SubProcess': 16,
		'System': 17,
		'SubSystem': 18,
		'BusinessSegment': 19,
		'BusinessUnit': 20,
		'Department': 21,
		'WorkPlace': 22,
		'BusinessRule': 23,
		'BusinessRuleVersion': 24,
		'BusinessDomain': 25,
		'BusinessPolicy': 26,
		'BusinessPolicyGroup': 27,
		'RoleGroup': 28,
		'Metrics': 29
	};

	var eBusinessRuleTypes = {
		'DMN': 1,
		'PRR': 2
	};

	var getValidChildren = function (typeId) {
		switch (typeId) {
			case 3:
				return [4];
			case 4:
				return [];
			case 23:
				return [24];
			case 24:
				return [];
			default:
				return [1, 2, 3, 15, 16, 17, 18, 19, 20, 21, 22, 23, 25, 26, 27, 28, 29];
		}
	};

	var getTypes = function () {
		return {
			1: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(1)
			},
			2: {
				"icon": "/Content/common-assets-includes/icons/layer.svg",
				"valid_children": getValidChildren(2)
			},
			3: {
				"icon": "/Content/common-assets-includes/icons/flow-chart.svg",
				"valid_children": getValidChildren(3)
			},
			4: {
				"icon": "/Content/common-assets-includes/icons/flow-chart4.svg",
				"valid_children": getValidChildren(4)
			},
			15: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(15)
			},
			16: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(16)
			},
			17: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(17)
			},
			18: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(18)
			},
			19: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(19)
			},
			20: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(20)
			},
			21: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(21)
			},
			22: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(22)
			},
			23: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(23)
			},
			24: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(24)
			},
			25: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(25)
			},
			26: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(26)
			},
			27: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(27)
			},
			28: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(28)
			},
			29: {
				"icon": "/Content/common-assets-includes/icons/layers.svg",
				"valid_children": getValidChildren(29)
			}
		}
	};

	var contextMenu = function (data) {
		var items = {
			'addNewGroup': {
				'icon': "/Content/common-assets-includes/icons/plus.svg",
				'label': 'New Folder',
				'action': function () {
					ProcessesManager.showModal(data.type, data.id);
				}
			},
			'addNewProcess': {
				'icon': "/Content/common-assets-includes/icons/plus.svg",
				'label': 'New Process',
				'action': function () {
					ProcessesManager.showModal(3, data.id);
				}
			},
			'addBusinessRule': {
				'icon': "/Content/common-assets-includes/icons/plus.svg",
				'label': 'New Business Process',
				'action': function () {
					ProcessesManager.showBusinessRuleModal(23, data.id);
				}
			},
			'addBusinessRuleVersion': {
				'icon': "/Content/common-assets-includes/icons/plus.svg",
				'label': 'New Business Process Version',
				'action': function () {
					ProcessesManager.showBusinessRuleModal(24, data.id);
				}
			},
			'addNewVersion': {
				'icon': "/Content/common-assets-includes/icons/plus.svg",
				'label': 'New Process Version',
				'action': function () {
					ProcessesManager.showModal(4, data.id);
				}
			},
			'setting': {
				'icon': "/Content/common-assets-includes/icons/edit.svg",
				'label': 'Edit',
				'action': function () {
					ProcessesManager.settingModal(data);
				}
			},
			'remove': {
				'icon': "/Content/common-assets-includes/icons/delete.svg",
				'label': 'Delete',
				'action': function () {
					ProcessesManager.deleteProcess(data.id,
						function () {
							$('#process-tree').jstree('delete_node', data.id);
						});
				}
			},
			'analysis': {
				'icon': "/Content/common-assets-includes/icons/analysis.svg",
				'label': 'Analysis',
				'action': function () {
					window.open(window.top.rootUrl +
						'Dashboard?NamespaceId=ProcessData&EntityId=ActivityInstanceRecord' +
						'&DashboardId=ProcessDashboard&Process=' +
						data.data.dbId,
						'_blank');
				}
			}
		};

		switch (data.type) {
			case 3:
				delete items.addNewGroup;
				delete items.addNewProcess;
				delete items.addBusinessRule;
				delete items.addBusinessRuleVersion;
				break;
			case 4:
				delete items.addNewGroup;
				delete items.addNewVersion;
				delete items.addNewProcess;
				delete items.analysis;
				delete items.addBusinessRule;
				delete items.addBusinessRuleVersion;
				break;
			case 23:
				delete items.addNewGroup;
				delete items.addBusinessRule;
				delete items.addNewProcess;
				delete items.addNewVersion;
				break;
			case 24:
				delete items.addNewGroup;
				delete items.addNewVersion;
				delete items.addNewProcess;
				delete items.analysis;
				delete items.addBusinessRule;
				delete items.addBusinessRuleVersion;
				break;
			default:
				delete items.addNewVersion;
				delete items.analysis;
				delete items.addBusinessRuleVersion;
		}
		return items;
	};

	var instantiateTree = function (data) {
		$(constTreeSelector)
			.jstree({
				'core': {
					'data': data,
					"check_callback": true
				},
				"contextmenu": {
					"select_node": false,
					'items': contextMenu
				},
				"types": getTypes(),
				"search": {
					//                    "fuzzy": true,
					"show_only_matches": true,
					"show_only_matches_children": true
				},
				"conditionalselect": function (data) {
					switch (data.type) {
						case eNodeTypes.ProcessVersion:
							ProcessesManager.goToBpmnEditor(data);
							break;
						case eNodeTypes.BusinessRuleVersion:
							ProcessesManager.goToBusinessRuleEditor(data);
							break;
						default:
							return false;
					}

					return false; //unreachable
				},
				"plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
			})
			.on('ready.jstree',
				function () {
					$('#process-tree').jstree("open_all");
					$("#process-tree").jstree("select_node", ProcessesManager.processId());

				})
			.on('move_node.jstree', ProcessesManager.nodeMoved)
			.on('search.jstree before_open.jstree',
				function (e, data) {
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

		$("#process-search-input").keyup(function () {
			$('#process-tree').jstree('search', $(this).val());
		});
	};

	var getGrandParentIdOfSelectedNode = function () {
		return $(constTreeSelector).jstree("get_selected", true)[0].parents[1];
	}

	var getNodeById = function (id) {
		return $(constTreeSelector).jstree("get_node", id);
	};

	var getNodeName = function (id) {
		return getNodeById(id).text;
	};

	return {
		instantiateTree: instantiateTree,
		eNodeTypes: eNodeTypes,
		getNodeName: getNodeName,
		getNodeById: getNodeById,
		getGrandParentIdOfSelectedNode: getGrandParentIdOfSelectedNode
	};
}();

$(function () {
	ProcessesManager.getTree().then(function (data) {
		if (!Array.isArray(data)) return;
		var treeData = data.map(function (item) {
			return {
				id: item.Id,
				parent: item.ParentId,
				text: item.Name,
				type: item.NodeTypeId,
				data: {
					code: item.StandardCode,
					order: item.OrderId,
					dbId: item.DbId
					//                    ,
					//                    businessRuleType: item.BusinessRuleType
				}
			}
		});
		TreeManager.instantiateTree(treeData);
	});
});