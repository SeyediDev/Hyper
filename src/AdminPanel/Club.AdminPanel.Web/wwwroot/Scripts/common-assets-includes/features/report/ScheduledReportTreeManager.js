var ScheduledReportTreeManager = function () {    
    var contextMenu = function (node) {
        if (node.type === 'default') return {};
        var items = {
            'execute': {
                'icon': "fa fa-play-circle-o",
                'label': 'اجرا',
                'action': function () {
                    ScheduledReport.takeReport(node.id, node.data.configId);
                }
            },
            'reportLog': {
                'icon': "fa fa-history",
                'label': 'سابقه گزارش گیری',
                'action': function () {
                    ScheduledReport.scheduledReportLog(node.id);
                }
            },
            'remove': {
                'icon': "fa fa-times",
                'label': 'حذف',
                'action': function () {
                    ScheduledReport.deleteSchedule(node.id, node.data.configId, function () {
                        $('#scheduledReport-tree').jstree('delete_node', node.id);
                    });
                }
            }
        };
        if (node.data.configId === $('#ConfigId').val())
            items['settings'] = {
                'icon': "fa fa-cogs",
                'label': 'تنظیمات',
                'action': function () {
                    ScheduledReport.showModal(node.id);
                }
            }
        return items;
    };

    var types = {
        "default": {
            "icon": window.top.rootUrl + "Content/common-assets-includes/icons/folder.svg",
            "valid_children": ["ScheduledReport"]
        },
        "ScheduledReport": {
            "icon": "fa fa-clock-o",
            "valid_children": []
        }
    };

    var createScheduleNode = function (id, text) {
        var $tree = $("#scheduledReport-tree");
        var nodes = $tree.jstree(true).get_json('#', {'flat': true});
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i]["id"] === id) return;
        }
        $tree
            .jstree('create_node',
                'thisConfig',
                {
                    "id": id,
                    "text": text,
                    "data": {"configId": $('#ConfigId').val()},
                    "type": 'ScheduledReport'
                },
                "last");
    };
//    nodeMoved: function (e, data) {
//        if (data['old_parent'] !== data['parent'])
//            console.warn('Must save ' + data.node.id + '\'s parent as ' + data['parent']); //todo
//    },

    var goToSchedule = function (id, configId) {
        window.location = window.top
            .rootUrl +
            'Report/LoadScheduled?configId=' +
            configId +
            '&scheduleId=' +
            id;
    };

    var instantiateTree = function () {
        $('#scheduledReport-tree')
            .jstree({
                'core': {
                    'data': window.scheduledReportsTree,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': contextMenu
                },
                "types": types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (node, event) {
                    if (node.type === 'default')
                        return false;
                    goToSchedule(node.id, node.data.configId);
                    return false;
                },
                "plugins": ["contextmenu", "types", "conditionalselect", "wholerow"]
            })
            //.on('move_node.jstree', ScheduledReportTreeManager.nodeMoved)
            .on('ready.jstree', function () {
                $("#scheduledReport-tree").jstree("open_all");
                $("#scheduledReport-tree").jstree("select_node",
                    ScheduledReport.acquireCurrentPageScheduleId());
            })
            //            .on('search.jstree before_open.jstree', function (e, data) {
            //                if (data.instance.settings.search.show_only_matches) {
            //                    data.instance._data.search.dom.find('.jstree-node')
            //                        .show().filter('.jstree-last').filter(function () {
            //                        return this.nextSibling;
            //                    }).removeClass('jstree-last')
            //                        .end().end().end().find(".jstree-children").each(function () {
            //                        $(this).children(".jstree-node:visible").eq(-1).addClass("jstree-last");
            //                    });
            //                }
            //            })
            .off('keydown');
    };

    $(instantiateTree);
    return {
        createScheduleNode: createScheduleNode
    };
}();
