var FilterConfig = function () {
    var constSubmitUri = "Filter/SaveFilterConfig";
    var constLoadUri = "Filter/OpenFilter";
    var constSaveUri = "Filter/SaveFilterValues";
    var constDeleteUri = "Filter/DeleteFilterConfig";
    var constChangeParentUri = "Filter/ChangeParent";

    var navigateTo = function (filterId) {
        var configId = $('#ConfigId').val();
        window.PageAddressManager.navigateTo(
            constLoadUri +
            '?filterId=' +
            filterId +
            (configId ? ('&configId=' + configId) : '')
        );
    };

    var fillTheForm = function (data) {
        var $filterModal = $("#filters-modal");
        $filterModal.find('input[name="filterName"]').val(data.Name);
        $filterModal.find('input[name="isDefault"]').prop('checked', data.IsDefault);
        $filterModal.find('input[name="isForThisConfig"]').prop('checked', !!data.ConfigId);
        if ($filterModal.find('input[name=IsPublic]').length > 0)
            $filterModal.find('input[name=IsPublic][value="' + data.IsPublic + '"]').prop('checked', true);
        $filterModal.find('input[name="filterId"]').val(data.FilterId);
        //$filterModal.find('input[name="ConfigId"]').val(ConfigId);
    };

    var showFiltersModal = function (filterId) {
        var $modal = $('#filters-modal');
        if (filterId === undefined) {
            fillTheForm({
                "FilterId": 0,
                // "ConfigId": 0,
                "Name": "",
                "IsDefault": false,
                "IsPublic": false
            });
        } else {
            var $tree = $("#filters-tree");
            var nodes = $tree.jstree(true).get_json('#', { 'flat': true });

            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].data.filterId === filterId) {
                    fillTheForm({
                        "FilterId": nodes[i].data.filterId,
                        "Name": nodes[i].text,
                        "IsDefault": nodes[i].data.isDefault,
                        "IsPublic": nodes[i].data.isPublic,
                        "ConfigId": nodes[i].data.configId
                    });
                    break;
                }
            }
        }
        $modal.modal('show');
    };

    var acquireFilter = function () {
        var result = [];
        $('#filterDiv :input').each(function (idx, item) {
            var $item = $(item);
            var name = $item.attr('name');
            if ($item.is(':radio') && !$item.is(':checked'))
                return;
            var value = $item.val();
            if (name && !$item.prop('disabled'))
                result.push({ FieldId: name, Value: !value ? value : value.toString() });
        });
        return result;
    };

    var acquireFilterDataObj = function () {
        var result = {};

        result["NamespaceId"] = window.PageAddressManager.getNamespaceId();
        result["EntityId"] = window.PageAddressManager.getEntityId();
        var pageType = window.PageAddressManager.getPageType();
        switch (pageType) {
            case "Form":
                result["FormId"] = window.PageAddressManager.getPageId();
                break;
            case "Report":
                result["ReportId"] = window.PageAddressManager.getPageId();
                break;
            case "Dashboard":
                result["DashboardId"] = window.PageAddressManager.getPageId();
                break;
            default:
                throw new Error('Invalid pageType');
        }
        result["FolderId"] = $('#filters-modal #folder-id').val();
        result["ParentFolderId"] = $('#filters-modal #parent-folder-id').val();
        if (!result["ParentFolderId"]) result["ParentFolderId"] = null;

        result["FilterValues"] = acquireFilter();

        result["Name"] = $('#filters-modal input[name="filterName"]').val();
        result["IsPublic"] = $('#filters-modal input[name="IsPublic"]:checked').val() === 'true';
        result["IsDefault"] = $('#filters-modal input[name="isDefault"]').is(':checked');
        result["FilterId"] = $('#filters-modal input[name="filterId"]').val();
        if ($('#filters-modal input[name="isForThisConfig"]').is(':checked'))
            result["ConfigId"] = $('#ConfigId').val();

        return result;
    };

    var submitForm = function () {
        var saveFilterObj = acquireFilterDataObj();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + constSubmitUri,
            data: saveFilterObj,
            headers: AddAntiForgeryToken(),
            success: function (res) {
                if (Number(res) > 0) {
                   FilterTreeManager.createFilterNode(res, saveFilterObj.Name, res == saveFilterObj.FilterId); //todo
                   navigateTo(res);
                } else {
                    console.error('Unable to "save" filter', res);
                    window.toast.error(window.tetaI18n.t('Error Occured'));
                }
                $('#filters-modal').modal('hide');
            },
            error: function (res) {
                if (!res)
                    res = window.tetaI18n.t('Error Occured');
                window.toast.error(res);
                $('#filters-modal').modal('hide');
            }
        });
    };

    var saveFilterValues = function (filterId) {
        var saveFilterObj = {
            filterValues: acquireFilter(),
            filterId: filterId
        };
        $.ajax({
            type: "POST",
            url: window.top.rootUrl +
                constSaveUri,
            //            +
            //                '?filterId=' +
            //                filterId,
            data: JSON.stringify(saveFilterObj),
            headers: AddAntiForgeryToken(),
            contentType: 'application/json; charset=utf-8',
            success: function () {
                window.toast.success(window.tetaI18n.t('Saved!'));
//                navigateTo(filterId);
            }
        });
    };    

    var changeParent = function (nodeId, parentId, isFolder) {
        var changeParentModel = { newFolderId: parentId };
        changeParentModel[isFolder ? 'folderId' : 'filterId'] = nodeId;
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + constChangeParentUri,
            data: changeParentModel,
            headers: AddAntiForgeryToken(),
            success: function () {
            },
            error: function () {
                window.toast.error(window.tetaI18n.t('FolderChangeError'));
            }
        });
    };

    var deleteFilter = function (filterId, cb) {
        var result = confirm(window.tetaI18n.t('SureToDeleteFilter'));
        if (!result) return;
        $.ajax({
            type: "POST",
            url: window.top.rootUrl +
                constDeleteUri +
                '?filterId=' + filterId,
            success: function () {
                if (cb)
                    cb();
            },
            headers: AddAntiForgeryToken()
        });
    };

    var getCurrentPageFilterId = function () {
        return window.currentFilterId;
    };

    return {
        deleteFilter: deleteFilter,
        changeParent: changeParent,
        showFiltersModal: showFiltersModal,
        submitForm: submitForm,
        saveFilterValues: saveFilterValues,
        getCurrentPageFilterId: getCurrentPageFilterId,
        acquireFilter: acquireFilter,
        navigateTo: navigateTo
    };
}();

var FilterTreeManager = function () {
    var types = {
        "default": { //folder
            "icon": window.top.rootUrl + "Content/common-assets-includes/icons/folder.svg",
            "valid_children": ["default", "filter"]
        },
        "filter": {
            "icon": "fa fa-filter",
            "valid_children": []
        }
    };

    var newFolder = function (parentId) {
        FolderManager.registerSubmissionCallback(createFolderNode);
        FolderManager.showModal(false, parentId);
    };
    
    var getNode = function (id) {
        var $tree = $("#filters-tree");
        var nodes = $tree.jstree(true).get_json('#', { 'flat': true });
        for (var i = 0; i < nodes.length; i++) {            
            if (nodes[i]["id"] === id) {
                return nodes[i];
            }
        }
    };

    var contextMenu = function (node) {
        var items = {
            'newFolder': {
                'icon': "fa fa-folder-open",
                'label': window.tetaI18n.t('New Folder'),
                'action': function () {
                    newFolder(node.data.folderId);
                }
            },
            'settings': {
                'icon': "fa fa-cogs",
                'label': window.tetaI18n.t('Edit'),
                'action': function () {
                    if (node.type === "default") {
                        FolderManager.showModal(false, node.parent, {
										 "Id": node.data.folderId,
                                "Name": node.text,
                                "IsPublic": node.data.isPublic,
                                "ParentFolderId": node.data.parentFolderId
                            });
                    }
                    else {
							  FilterConfig.showFiltersModal(node.data.filterId);
                    }
                }
            },
            'filterValues': {
                'icon': "fa fa-save",
                'label': window.tetaI18n.t('SetFilterValues'),
                'action': function () {
                    FilterConfig.saveFilterValues(node.data.filterId);
                }
            },
            'remove': {
                'icon': "fa fa-times",
                'label': window.tetaI18n.t('Remove'),
                'action': function () {
                    if (node.type === "default")
							  FolderManager.deleteFolder(node.data.folderId, function () {
								  $('#filters-tree').jstree('delete_node', node.id);
                        });
                    else
							  FilterConfig.deleteFilter(node.data.filterId, function () {
								  $('#filters-tree').jstree('delete_node', node.id);
                        });
                }
            }
        };

        if (node.type === "default")
            delete items.filterValues;
        else
            delete items.newFolder;
        return items;
    };

	var nodeMoved = function (e, data) {
        var parentId = data.parent === '#' ? '#' : getNode(data.parent).data.folderId;
        if (data['old_parent'] !== data['parent'])
			  FilterConfig.changeParent(data.node.type === 'default' ? data.node.data.folderId :
				  data.node.data.filterId, parentId, data.node.type === 'default');
        //            console.warn('Must save ' + data.node.id + '\'s parent as ' + data['parent']);
    }

    var selectCurrentNode = function (event, data) {
        $('#filters-tree').jstree("deselect_all");
        var filterId = FilterConfig.getCurrentPageFilterId();
        if (!filterId) return;
        $('#filters-tree').jstree('select_node', 'filter-' + filterId);
        data.instance._open_to(filterId);
    };

    var instantiateTree = function () {
        $('#filters-tree')
            .jstree({
                'core': {
                    'data': window.filtersTree,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': contextMenu
                },
                "types": types,
                "state": {
                    "key": 'filters-' + window.PageAddressManager.getPageId()
                },
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (node, event) {
                    if (node.type === 'default')
                        return true;
                    FilterConfig.navigateTo(node.data.filterId);
                    return false;
                },
                "plugins": ["dnd", "contextmenu", "types", "conditionalselect", "wholerow",
                    "search", "state"]
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
        $("#filters-search-input").keyup(function () {
            $('#filters-tree').jstree('search', $(this).val());
        });
    };    

    var createFilterNode = function (id, text, isUpdate) {
        var $tree = $("#filters-tree");
        //        var nodes = $tree.jstree(true).get_json('#', { 'flat': true });
        //        for (var i = 0; i < nodes.length; i++) {
        //            if (nodes[i]["id"] === id) return;
        //        }
        if (isUpdate) {
			  $tree.jstree('rename_node', 'filter-' + id, text);
            //todo data should also change
        }
        else
            $tree
                .jstree('create_node',
                    '#',
                    {
                        "id": 'filter-' + id,
                        "text": text,
							  "data": { "filterId": id }, //todo other data?
                        "type": 'filter'
                    },
                    "last");
    };


    var createFolderNode = function (nodeId, parentNodeId, text, isUpdate) {
        if (isUpdate) {
            $('#filters-tree').jstree('rename_node', 'folder-' + nodeId, text);
            //todo data should also change
        }
        else
            $('#filters-tree')
                .jstree('create_node',
						 parentNodeId ? 'folder-' + parentNodeId.toString() : '#',
                    {
							  "id": 'folder-' + nodeId,
                        "text": text,
                        //				    "type": "default",
                        "data": {
                            "isPublic": false,
									"parentFolderId": parentNodeId ? parentNodeId : null,
									 "folderId": nodeId
                        }
                    },
                    "first");
    };

    $(instantiateTree);
    return {
        createFilterNode: createFilterNode,
        newFolder: newFolder
    };
}();
