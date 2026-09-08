var ErrorEventManager = {
    ConstGetUri: 'BpmnError/List',
    ConstAddUri: 'BpmnError/Save',
    ConstDeleteUri: 'BpmnError/Delete',

    getTree: function () {
        return $.get('/' + this.ConstGetUri);
    },

    getNamespaces: function () {
        $.get('/MetaDesign/App/api/namespaces').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#error-namespaceId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
        $.get('/MetaDesign/Entities').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#error-entityId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
    },

    showRelatedEntities: function () {
        //var selectedNamespace = $('#error-namespaceId option:selected').val();
        //$("#error-entityId").empty();
        $.get('/MetaDesign/Entities').then(function (data) {
            var relatedEntities = data.filter(e => e.namespaceId == "ProcessEntities");
            for (var i = 0; i < relatedEntities.length; i++) {
                $("#error-entityId")
                    .append($('<option></option>').val(relatedEntities[i].id).text(relatedEntities[i].name));
            }
        });
    },

    updateTree: function(formData, id, isUpdate) {
        var $tree = $("#error-tree");
        if (isUpdate) {
            var node = $tree.jstree(true).get_node(id);
            node.data = {
                'errorCode': formData.errorCode,
                'namespaceId': formData.namespaceId,
                'entityId': formData.entityId
            };
            $tree.jstree('rename_node', id, formData.name);
        } else {
            var nodes = $tree.jstree(true).get_json($tree, {'flat': true});
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i]["id"] === formData.id) return;
            }
            $tree.jstree('create_node',
                "#",
                {
                    "id": id,
                    "parent": "#",
                    "text": formData.name,
                    "type": 'error',
                    "data": {
                        'errorCode': formData.errorCode,
                        'namespaceId': formData.namespaceId,
                        'entityId': formData.entityId
                    }
                },
                "first");
        }
        $tree.jstree("open_all");
    },

    deleteNode: function (id, cb) {
        var result = confirm("Are you sure?");
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

    acquireFormData: function () {
        var $modal = $("#error-modal");
        var result = {};
        result.id = $modal.find('#error-id').val();
        result.name = $modal.find('#error-name').val();
        result.errorCode = $modal.find('#error-code').val();
        result.namespaceId = $modal.find('#error-namespaceId').val();
        result.entityId = $modal.find('#error-entityId').val();
        return result;
    },

    submitForm: function () {
        var $modal = $("#error-modal");
        var formData = ErrorEventManager.acquireFormData();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + this.ConstAddUri,
            data: formData,
           headers: window.AddAntiForgeryToken(),
            success: function (res) {
                ErrorEventManager.updateTree(formData, res.id, res.id == formData.id);
                $modal.modal('hide');

            },
            error: function () {
                alert('Something went wrong unexpectedly.!');
                $modal.modal('hide');
            }
        });
    },

    fillTheForm: function (data) {
        var $modal = $("#error-modal");
        $modal.find('#error-id').val(data.id);
        $modal.find('#error-name').val(data.name);
        $modal.find('#error-code').val(data.errorCode);
        $modal.find('#error-namespaceId').val(data.namespaceId);
        $modal.find('#error-entityId').val(data.entityId);
    },

    settingModal: function (node) {
        var $modal = $("#error-modal");
        var data = {
            'id': node.id,
            'name': node.text,
            'errorCode': node.data.errorCode,
            'namespaceId': node.data.namespaceId,
            'entityId': node.data.entityId
        };

        ErrorEventManager.fillTheForm(data);
        $modal.modal('show');
    },

    showModal: function () {
        var $modal = $("#error-modal");
        var data = {
            'id': '',
            'name': '',
            'errorCode': '',
            'namespaceId': '',
            'entityId': ''
        };

        ErrorEventManager.fillTheForm(data);
        $modal.modal('show');
    }

};
var MessageEventManager = {
    ConstGetUri: 'BpmnMessage/List',
    ConstAddUri: 'BpmnMessage/Save',
    ConstDeleteUri: 'BpmnMessage/Delete',

    getTree: function () {
        return $.get('/' + this.ConstGetUri);
    },

    getNamespaces: function () {
        $.get('/MetaDesign/App/api/namespaces').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#message-namespaceId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
        $.get('/MetaDesign/Entities').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#message-entityId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
    },

    showRelatedEntities: function () {
        //var selectedNamespace = $('#message-namespaceId option:selected').val();
        //$("#message-entityId").empty();
        $.get('/MetaDesign/Entities').then(function (data) {
            var relatedEntities = data.filter(e => e.namespaceId == "ProcessEntities");
            for (var i = 0; i < relatedEntities.length; i++) {
                $("#message-entityId")
                    .append($('<option></option>').val(relatedEntities[i].id).text(relatedEntities[i].name));
            }
        });

    },

    updateTree: function(formData, id, isUpdate) {
        var $tree = $("#message-tree");
        if (isUpdate) {
            var node = $tree.jstree(true).get_node(id);
            node.data = {
                'namespaceId': formData.namespaceId,
                'entityId': formData.entityId
            };
            $tree.jstree('rename_node', id, formData.name);
        } else {
            var nodes = $tree.jstree(true).get_json($tree, {'flat': true});
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i]["id"] === formData.id) return;
            }
            $tree.jstree('create_node',
                '#',
                {
                    "id": id,
                    "parent": '#',
                    "text": formData.name,
                    "type": "message",
                    "data": {
                        'namespaceId': formData.namespaceId,
                        'entityId': formData.entityId
                    }
                },
                "first");
        }
        $tree.jstree("open_all");
    },

    deleteNode: function (id, cb) {
        var result = confirm("Are you sure?");
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

    acquireFormData: function () {
        var $modal = $("#message-modal");
        var result = {};
        result.id = $modal.find('#message-id').val();
        result.name = $modal.find('#message-name').val();
        result.namespaceId = $modal.find('#message-namespaceId').val();
        result.entityId = $modal.find('#message-entityId').val();
        return result;
    },

    submitForm: function () {
        var $modal = $("#message-modal");
        var formData = MessageEventManager.acquireFormData();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + this.ConstAddUri,
            data: formData,
           headers: window.AddAntiForgeryToken(),
            success: function (res) {
                MessageEventManager.updateTree(formData, res.id, res.id == formData.id);

                $modal.modal('hide');

            },
            error: function () {
                alert('Something went wrong unexpectedly.!');
                $modal.modal('hide');
            }
        });
    },

    fillTheForm: function (data) {
        var $modal = $("#message-modal");
        $modal.find('#message-id').val(data.id);
        $modal.find('#message-name').val(data.name);
        $modal.find('#message-namespaceId').val(data.namespaceId);
        $modal.find('#message-entityId').val(data.entityId);
    },

    settingModal: function (node) {
        var $modal = $("#message-modal");
        var savedData = {
            'id': node.id,
            'name': node.text,
            'namespaceId': node.data.namespaceId,
            'entityId': node.data.entityId
        };

        MessageEventManager.fillTheForm(savedData);
        $modal.modal('show');
    },

    showModal: function () {
        var $modal = $("#message-modal");
        var data = {
            'id': '',
            'name': '',
            'namespaceId': '',
            'entityId': ''
        };
        MessageEventManager.fillTheForm(data);
        $modal.modal('show');
    }

};
var EscalateEventManager = {
    ConstGetUri: 'BpmnEscalate/List',
    ConstAddUri: 'BpmnEscalate/Save',
    ConstDeleteUri: 'BpmnEscalate/Delete',

    getTree: function () {
        return $.get('/' + this.ConstGetUri);
    },

    getNamespaces: function () {
        $.get('/MetaDesign/App/api/namespaces').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#escalate-namespaceId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
        $.get('/MetaDesign/Entities').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#escalate-entityId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
    },

    showRelatedEntities: function () {
        //var selectedNamespace = $('#escalate-namespaceId option:selected').val();
        //$("#escalate-entityId").empty();
        $.get('/MetaDesign/Entities').then(function (data) {
            var relatedEntities = data.filter(e => e.namespaceId == "ProcessEntities");
            for (var i = 0; i < relatedEntities.length; i++) {
                $("#escalate-entityId")
                    .append($('<option></option>').val(relatedEntities[i].id).text(relatedEntities[i].name));
            }
        });
    },

    updateTree: function(formData, id, isUpdate) {
        var $tree = $("#escalate-tree");
        if (isUpdate) {
            var node = $tree.jstree(true).get_node(id);
            node.data = {
                'escalationCode': formData.escalationCode,
                'namespaceId': formData.namespaceId,
                'entityId': formData.entityId
            };
            $tree.jstree('rename_node', id, formData.name);
        } else {
            var nodes = $tree.jstree(true).get_json($tree, {'flat': true});
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i]["id"] === formData.id) return;
            }
            $tree.jstree('create_node',
                "#",
                {
                    "id": id,
                    "parent": "#",
                    "text": formData.name,
                    "type": 'escalate',
                    "data": {
                        'escalationCode': formData.escalationCode,
                        'namespaceId': formData.namespaceId,
                        'entityId': formData.entityId
                    }
                },
                "first");
        }
        $tree.jstree("open_all");
    },

    deleteNode: function (id, cb) {
        var result = confirm("Are you sure?");
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

    acquireFormData: function () {
        var $modal = $("#escalate-modal");
        var result = {};

        result.id = $modal.find('#escalate-id').val();
        result.name = $modal.find('#escalate-name').val();
        result.escalationCode = $modal.find('#escalation-code').val();
        result.namespaceId = $modal.find('#escalate-namespaceId').val();
        result.entityId = $modal.find('#escalate-entityId').val();
        return result;
    },

    submitForm: function () {
        var $modal = $("#escalate-modal");
        var formData = EscalateEventManager.acquireFormData();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + this.ConstAddUri,
            data: formData,
           headers: window.AddAntiForgeryToken(),
            success: function (res) {
                EscalateEventManager.updateTree(formData, res.id, res.id == formData.id);
                $modal.modal('hide');

            },
            error: function () {
                alert('Something went wrong unexpectedly.!');
                $modal.modal('hide');
            }
        });
    },

    fillTheForm: function (data) {
        var $modal = $("#escalate-modal");
        $modal.find('#escalate-id').val(data.id);
        $modal.find('#escalate-name').val(data.name);
        $modal.find('#escalation-code').val(data.escalationCode);
        $modal.find('#escalate-namespaceId').val(data.namespaceId);
        $modal.find('#escalate-entityId').val(data.entityId);
    },

    settingModal: function (node) {
        var $modal = $("#escalate-modal");
        var data = {
            'id': node.id,
            'name': node.text,
            'escalationCode': node.data.escalationCode,
            'namespaceId': node.data.namespaceId,
            'entityId': node.data.entityId
        };

        EscalateEventManager.fillTheForm(data);
        $modal.modal('show');
    },

    showModal: function () {
        var $modal = $("#escalate-modal");
        var data = {
            'id': '',
            'name': '',
            'escalationCode': '',
            'namespaceId': '',
            'entityId': ''
        };

        EscalateEventManager.fillTheForm(data);
        $modal.modal('show');
    }

};
var SignalEventManager = {
    ConstGetUri: 'BpmnSignal/List',
    ConstAddUri: 'BpmnSignal/Save',
    ConstDeleteUri: 'BpmnSignal/Delete',

    getTree: function () {
        return $.get('/' + this.ConstGetUri);
    },

    getNamespaces: function () {
        $.get('/MetaDesign/App/api/namespaces').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#signal-namespaceId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
        $.get('/MetaDesign/Entities').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#signal-entityId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
    },

    showRelatedEntities: function () {
        //var selectedNamespace = $('#signal-namespaceId option:selected').val();
        //$("#signal-entityId").empty();
        $.get('/MetaDesign/Entities').then(function (data) {
            var relatedEntities = data.filter(e => e.namespaceId == "ProcessEntities");
            for (var i = 0; i < relatedEntities.length; i++) {
                $("#signal-entityId")
                    .append($('<option></option>').val(relatedEntities[i].id).text(relatedEntities[i].name));
            }
        });
    },

    updateTree: function(formData, id, isUpdate) {
        var $tree = $("#signal-tree");
        if (isUpdate) {
            var node = $tree.jstree(true).get_node(id);
            node.data = {
                'namespaceId': formData.namespaceId,
                'entityId': formData.entityId
            };
            $tree.jstree('rename_node', id, formData.name);
        } else {
            var nodes = $tree.jstree(true).get_json($tree, {'flat': true});
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i]["id"] === formData.id) return;
            }
            $tree.jstree('create_node',
                "#",
                {
                    "id": id,
                    "parent": "#",
                    "text": formData.name,
                    "type": "signal",
                    "data": {
                        'namespaceId': formData.namespaceId,
                        'entityId': formData.entityId
                    }
                },
                "first");
        }
        $tree.jstree("open_all");
    },

    deleteNode: function (id, cb) {
        var result = confirm("Are you sure?");
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

    acquireFormData: function () {
        var $modal = $("#signal-modal");
        var result = {};
        result.id = $modal.find('#signal-id').val();
        result.name = $modal.find('#signal-name').val();
        result.namespaceId = $modal.find('#signal-namespaceId').val();
        result.entityId = $modal.find('#signal-entityId').val();
        return result;
    },

    submitForm: function () {
        var $modal = $("#signal-modal");
        var formData = SignalEventManager.acquireFormData();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + this.ConstAddUri,
            data: formData,
           headers: window.AddAntiForgeryToken(),
            success: function (res) {
                SignalEventManager.updateTree(formData, res.id, res.id == formData.id);

                $modal.modal('hide');

            },
            error: function () {
                alert('Something went wrong unexpectedly.!');
                $modal.modal('hide');
            }
        });
    },

    fillTheForm: function (data) {
        var $modal = $("#signal-modal");
        $modal.find('#signal-id').val(data.id);
        $modal.find('#signal-name').val(data.name);
        $modal.find('#signal-namespaceId').val(data.namespaceId);
        $modal.find('#signal-entityId').val(data.entityId);
    },

    settingModal: function (node) {
        var $modal = $("#signal-modal");
        var data = {
            'id': node.id,
            'name': node.text,
            'namespaceId': node.data.namespaceId,
            'entityId': node.data.entityId
        };

        SignalEventManager.fillTheForm(data);
        $modal.modal('show');
    },

    showModal: function () {
        var $modal = $("#signal-modal");
        var data = {
            'id': '',
            'name': '',
            'namespaceId': '',
            'entityId': ''
        };

        SignalEventManager.fillTheForm(data);
        $modal.modal('show');
    }

};
var DataStoreManager = {
    ConstGetUri: 'BpmnDataStore/List',
    ConstAddUri: 'BpmnDataStore/Save',
    ConstDeleteUri: 'BpmnDataStore/Delete',

    getTree: function () {
        return $.get('/' + this.ConstGetUri);
    },

    getNamespaces: function () {
        $.get('/MetaDesign/App/api/namespaces').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#datastore-namespaceId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
        $.get('/MetaDesign/Entities').then(function (data) {
            for (var i = 0; i < data.length; i++) {
                $("#datastore-entityId").append($('<option></option>').val(data[i].id).text(data[i].name));
            }
        });
    },

    showRelatedEntities: function () {
        var selectedNamespace = $('#datastore-namespaceId option:selected').val();
        $("#datastore-entityId").empty();
        $.get('/MetaDesign/Entities').then(function (data) {
            var relatedEntities = data.filter(e => e.namespaceId == selectedNamespace);
            for (var i = 0; i < relatedEntities.length; i++) {
                $("#datastore-entityId").append(
                    $('<option></option>').val(relatedEntities[i].id).text(relatedEntities[i].name));
            }
        });
    },

    updateTree: function(formData, id, isUpdate) {
        var $tree = $("#datastore-tree");
        if (isUpdate) {
            var node = $tree.jstree(true).get_node(id);
            node.data = {
                'isUnLimited': formData.isUnLimited,
                'capacity': formData.capacity,
                'namespaceId': formData.namespaceId,
                'entityId': formData.entityId
            };
            $tree.jstree('rename_node', id, formData.name);
        } else {
            var nodes = $tree.jstree(true).get_json($tree, {'flat': true});
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i]["id"] === formData.id) return;
            }
            $tree.jstree('create_node',
                "#",
                {
                    "id": id,
                    "parent": "#",
                    "text": formData.name,
                    "type": 'datastore',
                    "data": {
                        'isUnLimited': formData.isUnLimited,
                        'capacity': formData.capacity,
                        'namespaceId': formData.namespaceId,
                        'entityId': formData.entityId
                    }
                },
                "first");
        }
        $tree.jstree("open_all");
    },

    deleteNode: function (id, cb) {
        var result = confirm("Are you sure");
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

    acquireFormData: function () {
        var $modal = $("#datastore-modal");
        var result = {};
        result.id = $modal.find('#datastore-id').val();
        result.name = $modal.find('#datastore-name').val();
        result.capacity = $modal.find('#datastore-capacity').val();
        result.isUnLimited = $modal.find('#datastore-isUnLimited').prop('checked');
        result.namespaceId = $modal.find('#datastore-namespaceId').val();
        result.entityId = $modal.find('#datastore-entityId').val();
        return result;
    },

    submitForm: function () {
        var $modal = $("#datastore-modal");
        var formData = DataStoreManager.acquireFormData();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + this.ConstAddUri,
            data: formData,
           headers: window.AddAntiForgeryToken(),
            success: function (res) {
                DataStoreManager.updateTree(formData, res.id, res.id == formData.id);
                $modal.modal('hide');

            },
            error: function () {
                alert('Something went wrong unexpectedly.!');
                $modal.modal('hide');
            }
        });
    },

    fillTheForm: function (data) {
        var $modal = $("#datastore-modal");
        $modal.find('#datastore-id').val(data.id);
        $modal.find('#datastore-name').val(data.name);
        $modal.find('#datastore-capacity').val(data.capacity);
        $modal.find('#datastore-isUnLimited').prop('checked', data.isUnLimited);
        $modal.find('#datastore-namespaceId').val(data.namespaceId);
        $modal.find('#datastore-entityId').val(data.entityId);
    },

    settingModal: function(node) {
        var $modal = $("#datastore-modal");
        var data = {
            'id': node.id,
            'name': node.text,
            'capacity': node.data.capacity,
            'isUnLimited': node.data.isUnLimited,
            'namespaceId': node.data.namespaceId,
            'entityId': node.data.entityId
        };

        DataStoreManager.fillTheForm(data);
        if ($modal.find('#datastore-isUnLimited').prop('checked'))
            $("#datastore-capacity-div").hide();
        else
            $("#datastore-capacity-div").show();
        $modal.modal('show');
    },

    showModal: function () {
        var $modal = $("#datastore-modal");
        var data = {
            'id': '',
            'name': '',
            'capacity': '',
            'isUnLimited': true,
            'namespaceId': '',
            'entityId': ''
        };

        DataStoreManager.fillTheForm(data);
        if ($modal.find('#datastore-isUnLimited').prop('checked'))
            $("#datastore-capacity-div").hide();
        else
            $("#datastore-capacity-div").show();
        $modal.modal('show');
    },

    showOrhideCapacity: function () {
        var $modal = $("#datastore-modal");
        if ($modal.find('#datastore-isUnLimited').prop('checked'))
            $("#datastore-capacity-div").hide();
        else
            $("#datastore-capacity-div").show();
    }
};
var InterfaceManager = {
    ConstGetUri: 'BpmnInterface/List',
    ConstAddUri: 'BpmnInterface/Save',
    ConstAddOperationUri: 'BpmnInterfaceOperations/Save',
    ConstDeleteOperationUri: 'BpmnInterfaceOperations/Delete',
    ConstDeleteUri: 'BpmnInterface/Delete',

    getTree: function () {
        return $.get('/' + this.ConstGetUri);
    },

    fillMessagesAndErrors: function (messages, errors) {
        if (messages != null) {
            for (var i = 0; i < messages.length; i++) {
                $("#inMessageRef").append($('<option></option>').val(messages[i].id).text(messages[i].name));
                $("#outMessageRef").append($('<option></option>').val(messages[i].id).text(messages[i].name));
            }
        }
        if (errors != null) {
            for (var j = 0; j < errors.length; j++) {
                $("#errorRef").append($('<option></option>').val(errors[j].id).text(errors[j].name));
            }
        }

    },

    updateTree: function(formData, id, type, isUpdate) {
        var $tree = $("#interface-tree");
        if (isUpdate) {
            var node = $tree.jstree(true).get_node(id);
            node.data = {
                implementationRef: formData.implementationRef,
                inMessageRef: formData.inMessageRef,
                outMessageRef: formData.outMessageRef,
                errorRef: formData.errorRef
            };
            $tree.jstree('rename_node', id, formData.name);
        } else {
            var nodes = $tree.jstree(true).get_json($tree, {'flat': true});
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i]["id"] === formData.id) return;
            }
            $tree.jstree('create_node',
                formData.interfaceId,
                {
                    "id": id,
                    "parent": formData.interfaceId,
                    "text": formData.name,
                    "type": type,
                    "data": {
                        implementationRef: formData.implementationRef,
                        inMessageRef: formData.inMessageRef,
                        outMessageRef: formData.outMessageRef,
                        errorRef: formData.errorRef
                    }
                },
                "first");
        }
        $tree.jstree("open_all");
    },

    deleteNode: function (id, cb) {
        var result = confirm("Are you sure?");
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

    deleteOperation: function (id, cb) {
        var result = confirm("Are you sure?");
        if (!result) return;
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + this.ConstDeleteOperationUri + '?Id=' + id,
            success: function () {
                if (cb)
                    cb();
            },
           headers: window.AddAntiForgeryToken()
        });
    },

    acquireFormData: function (type) {
        var $modal = $("#interface-modal");
        var result = {};
        if (type == 'interface') {
            result.interfaceId = $modal.find('#interface-parent').val();
            result.id = $modal.find('#interface-id').val();
            result.name = $modal.find('#interface-name').val();
            result.implementationRef = $modal.find('#implementationRef').val();
            result.operations = [];
        }
        if (type == 'operation') {
            $modal = $("#operations-modal");
            result.interfaceId = $modal.find('#operation-interfaceId').val();
            result.id = $modal.find('#operation-id').val();
            result.name = $modal.find('#operation-name').val();
            result.implementationRef = $modal.find('#implementationRef').val();
            result.inMessageRef = $modal.find('#inMessageRef').val();
            result.outMessageRef = $modal.find('#outMessageRef').val();
            result.errorRef = $modal.find("#error-table").tabulator("getData");
        }
        return result;
    }, //todo items

    submitForm: function (type) {
        var $modal = $("#interface-modal");
        var formData = {};
        var url = '';
        if (type == 'interface') {
            formData = InterfaceManager.acquireFormData('interface');
            url = window.top.rootUrl + this.ConstAddUri;
        }
        if (type == 'operation') {
            $modal = $("#operations-modal");
            formData = InterfaceManager.acquireFormData('operation');
            url = window.top.rootUrl + this.ConstAddOperationUri;
        }

        $.ajax({
            type: "POST",
            url: url,
            data: formData,
           headers: window.AddAntiForgeryToken(),
            success: function (res) {
                InterfaceManager.updateTree(formData, res.id, type, res.id == formData.id);
                $modal.modal('hide');
            },
            error: function () {
                alert('Something went wrong unexpectedly.!');
                $modal.modal('hide');
            }
        });
    },

    fillTheForm: function (data, type) {
        var $modal = $("#interface-modal");
        if (type == 'interface') {

            $modal.find('#interface-parent').val(data.parent);
            $modal.find('#interface-id').val(data.id);
            $modal.find('#interface-name').val(data.name);
            $modal.find('#implementationRef').val(data.implementationRef);
        }
        if (type == 'operation') {
            $modal = $("#operations-modal");
            $modal.find('#operation-interfaceId').val(data.interfaceId);
            $modal.find('#operation-id').val(data.id);
            $modal.find('#operation-name').val(data.name);
            $modal.find('#implementationRef').val(data.implementationRef);
            $modal.find('#inMessageRef').val(data.inMessageRef);
            $modal.find('#outMessageRef').val(data.outMessageRef);
            $modal.find('#errorRef').val(data.errorRef);
        }
    },

    settingModal: function (node) {
        var $modal = $("#interface-modal");
        var data = {};
        if (node.type == 'interface') {
            data = {
                'parent': node.parent,
                'id': node.id,
                'name': node.text,
                'implementationRef': node.data.implementationRef
            };
            $modal.modal('show');
        }
        if (node.type == 'operation') {
            $modal = $("#operations-modal");
            data = {
                'interfaceId': node.parent,
                'id': node.id,
                'name': node.text,
                'implementationRef': node.data.implementationRef,
                'inMessageRef': node.data.inMessageRef,
                'outMessageRef': node.data.outMessageRef,
                'errorRef': node.data.errorRef
            };
            $modal.modal('show');
        }

        InterfaceManager.fillTheForm(data, node.type);
        InterfaceManager.instantiateTable();

        if (node.data.errorRef != null)
            $("#error-table").tabulator("setData", node.data.errorRef);
        else
            $("#error-table").tabulator("setData", []);

        $("#error-table").tabulator("redraw");
    },

    showModal: function (type, parent) {
        var $modal = $("#interface-modal");
        var data = {};
        if (type == 'interface') {
            data = {
                'parent': parent,
                'id': '',
                'name': '',
                'implementationRef': ''
            };
            $modal.modal('show');
        }
        if (type == 'operation') {
            $modal = $("#operations-modal");
            data = {
                'interfaceId': parent,
                'id': '',
                'name': '',
                'implementationRef': '',
                'inMessageRef': '',
                'outMessageRef': '',
                'errorRef': null
            };
            $modal.modal('show');
        }
        InterfaceManager.fillTheForm(data, type);
        InterfaceManager.instantiateTable();
        $("#error-table").tabulator("setData", []);
    },

    instantiateTable: function () {
        var deleteIcon = function () {
            return "<i class='fa fa-trash btn btn-danger'></i>";
        };
        $("#error-table")
            .tabulator({
                height: "200px",
                layout: "fitColumns",
                columns: [
                    {
                        title: "Name",
                        field: "name",
                        sorter: "string",
                        headerSort: false,
                        width: 150,
                        editable: true,
                        editor: "input"
                    },
                    {
                        title: "Error Code",
                        field: "errorCode",
                        sorter: "string",
                        headerSort: false,
                        width: 100,
                        editable: true,
                        editor: "input"
                    },
                    {
                        title: "Structure",
                        field: "entityId",
                        sorter: "string",
                        headerSort: false,
                        width: 200,
                        editable: true,
                        editor: "select",
                        editorParams: window.structures.map(function (en) {
                            return {
                                label: en.name,
                                value: en.value
                            }
                        })
                    },
                    {
                        title: "",
                        formatter: deleteIcon,
                        headerSort: false,
                        width: 80,
                        cellClick: function (e, cell) {
                            cell.getRow().delete();
                        }
                    }
                ]
            });
    },

    addNewError: function () {
        $("#error-table").tabulator("addRow",
            {
                id: '',
                name: 'name',
                errorCode: 'error code',
                namespaceId: 'ProcessEntities',
                entityId: ''
            });
    },

    addExistingErrorToTable: function () {
        var errors = [];
        $.get('/BpmnError/List')
            .then(function (data) {
                errors = data;
           var error = errors.find(e => e.id == $('#errorRef').val());
                $("#error-table").tabulator("addRow",
                    {
                        id: error.id,
                        name: error.name,
                        errorCode: error.errorCode,
                        namespaceId: error.namespaceId,
                        entityId: error.entityId
                    });
            });
    }
};

var ErrorTreeManager = {
    instantiateTree: function (data) {
        $('#error-tree')
            .jstree({
                'core': {
                    'data': data,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': ErrorTreeManager.contextMenu
                },
                "types": ErrorTreeManager.types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (data) {
                    ErrorEventManager.settingModal(data);
                    return false; //unreachable
                },
                "plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
            })
            .on('ready.jstree',
                function () {
                    $('#error-tree').jstree("open_all");
                })
            .on('search.jstree before_open.jstree',
                function(e, data) {
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

        $("#error-search-input").keyup(function () {
            $('#error-tree').jstree('search', $(this).val());
        });
    },

    contextMenu: function (data) {
        var items = {
            'Add': {
                'icon': "/Content/common-assets-includes/icons/plus.svg",
                'label': 'Add',
                'action': function () {
                    ErrorEventManager.showModal();
                }
            },
            'Delete': {
                'icon': "/Content/common-assets-includes/icons/delete.svg",
                'label': 'Delete',
                'action': function () {
                    ErrorEventManager.deleteNode(data.id,
                        function() {
                        $('#error-tree').jstree('delete_node', data.id);
                    });
                }
            }
        };
        return items;
    },

    types: {
        'error': {
            "icon": " bpmn-icon-intermediate-event-catch-error",
            "valid_children": []
        }
    }
};
var MessageTreeManager = {
    instantiateTree: function (data) {
        $('#message-tree')
            .jstree({
                'core': {
                    'data': data,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': MessageTreeManager.contextMenu
                },
                "types": MessageTreeManager.types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (data) {
                    MessageEventManager.settingModal(data);
                    return false; //unreachable
                },
                "plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
            })
            .on('ready.jstree',
                function () {
                    $('#message-tree').jstree("open_all");
                })
            .on('search.jstree before_open.jstree',
                function(e, data) {
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

        $("#message-search-input").keyup(function () {
            $('#message-tree').jstree('search', $(this).val());
        });
    },

    contextMenu: function (data) {
        var items = {
            'Add': {
                'icon': "/Content/common-assets-includes/icons/plus.svg",
                'label': 'Add',
                'action': function () {
                    MessageEventManager.showModal();
                }
            },
            'Delete': {
                'icon': "/Content/common-assets-includes/icons/delete.svg",
                'label': 'Delete',
                'action': function () {
                    MessageEventManager.deleteNode(data.id,
                        function () {
                            $('#message-tree').jstree('delete_node', data.id);
                        });
                }
            }
        };
        return items;

    },

    types: {
        'message': {
            "icon": "bpmn-icon-intermediate-event-throw-message",
            "valid_children": []
        }
    }
};
var EscalateTreeManager = {
    instantiateTree: function (data) {
        $('#escalate-tree')
            .jstree({
                'core': {
                    'data': data,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': EscalateTreeManager.contextMenu
                },
                "types": EscalateTreeManager.types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (data) {
                    EscalateEventManager.settingModal(data);
                    return false; //unreachable
                },
                "plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
            })
            .on('ready.jstree',
                function () {
                    $('#escalate-tree').jstree("open_all");
                })
            .on('search.jstree before_open.jstree',
                function(e, data) {
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

        $("#escalate-search-input").keyup(function () {
            $('#escalate-tree').jstree('search', $(this).val());
        });
    },

    contextMenu: function (data) {
        var items = {
            'Add': {
                'icon': "/Content/common-assets-includes/iconsplus.svg",
                'label': 'Add',
                'action': function () {
                    EscalateEventManager.showModal();
                }
            },
            'Delete': {
                'icon': "/Content/common-assets-includes/icons/delete.svg",
                'label': 'Delete',
                'action': function () {
                    EscalateEventManager.deleteNode(data.id,
                        function() {
                        $('#escalate-tree').jstree('delete_node', data.id);
                    });
                }
            }
        };
        return items;

    },

    types: {
        'escalate': {
            "icon": "bpmn-icon-intermediate-event-throw-escalation",
            "valid_children": []
        }
    }
};
var SignalTreeManager = {
    instantiateTree: function (data) {
        $('#signal-tree')
            .jstree({
                'core': {
                    'data': data,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': SignalTreeManager.contextMenu
                },
                "types": SignalTreeManager.types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (data) {
                    SignalEventManager.settingModal(data);
                    return false; //unreachable
                },
                "plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
            })
            .on('ready.jstree',
                function () {
                    $('#signal-tree').jstree("open_all");

                })
            .on('search.jstree before_open.jstree',
                function(e, data) {
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

        $("#signal-search-input").keyup(function () {
            $('#signal-tree').jstree('search', $(this).val());
        });
    },

    contextMenu: function (data) {
        var items = {
            'Add': {
                'icon': "/Content/common-assets-includes/icons/plus.svg",
                'label': 'Add',
                'action': function () {
                    SignalEventManager.showModal();
                }
            },
            'Delete': {
                'icon': "/Content/common-assets-includes/icons/delete.svg",
                'label': 'Delete',
                'action': function () {
                    SignalEventManager.deleteNode(data.id,
                        function () {
                            $('#signal-tree').jstree('delete_node', data.id);
                        });
                }
            }
        };
        return items;

    },

    types: {
        'signal': {
            "icon": "bpmn-icon-intermediate-event-throw-signal",
            "valid_children": []
        }
    }
};
var DataStoreTreeManager = {
    instantiateTree: function (data) {
        $('#datastore-tree')
            .jstree({
                'core': {
                    'data': data,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': DataStoreTreeManager.contextMenu
                },
                "types": DataStoreTreeManager.types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (data) {
                    DataStoreManager.settingModal(data);
                    return false; //unreachable
                },
                "plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
            })
            .on('ready.jstree',
                function() {
                $('#datastore-tree').jstree("open_all");
            })
            .on('search.jstree before_open.jstree',
                function(e, data) {
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

        $("#datastore-search-input").keyup(function () {
            $('#datastore-tree').jstree('search', $(this).val());
        });
    },

    contextMenu: function (data) {
        var items = {
            'Add': {
                'icon': "/Content/common-assets-includes/icons/plus.svg",
                'label': 'Add',
                'action': function () {
                    DataStoreManager.showModal();
                }
            },
            'Delete': {
                'icon': "/Content/common-assets-includes/icons/delete.svg",
                'label': 'Delete',
                'action': function () {
                    DataStoreManager.deleteNode(data.id,
                        function () {
                            $('#datastore-tree').jstree('delete_node', data.id);
                        });
                }
            }
        };
        return items;

    },

    types: {
        'datastore': {
            "icon": "bpmn-icon-data-store",
            "valid_children": []
        }
    }
};
var InterfaceTreeManager = {
    instantiateTree: function (data) {
        $('#interface-tree')
            .jstree({
                'core': {
                    'data': data,
                    "check_callback": true
                },
                "contextmenu": {
                    "select_node": false,
                    'items': InterfaceTreeManager.contextMenu
                },
                "types": InterfaceTreeManager.types,
                "search": {
                    "fuzzy": true,
                    "show_only_matches": true,
                    "show_only_matches_children": true
                },
                "conditionalselect": function (data) {
                    InterfaceManager.settingModal(data);
                    return false; //unreachable
                },
                "plugins": ["contextmenu", "types", "search", "conditionalselect", "wholerow", "dnd"]
            })
            .on('ready.jstree',
                function () {
                    $('#interface-tree').jstree("open_all");
                })
            .on('search.jstree before_open.jstree',
                function(e, data) {
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

        $("#interface-search-input").keyup(function () {
            $('#interface-tree').jstree('search', $(this).val());
        });
    },

    contextMenu: function (data) {
        var items = {
            //'NewInterface': {
            //    'icon': "/Content/common-assets-includes/icons/plus.svg",
            //    'label': 'New Interface',
            //    'action': function () {
            //        InterfaceManager.showModal('interface', data.parent);
            //    }
            //},
            'NewOperation': {
                'icon': "/Content/common-assets-includes/icons/plus.svg",
                'label': 'New Operation',
                'action': function () {
                    InterfaceManager.showModal('operation', data.id);
                }
            },
            'Delete': {
                'icon': "/Content/common-assets-includes/icons/delete.svg",
                'label': 'Delete',
                'action': function () {
                    if (data.type == 'interface') {
                        InterfaceManager.deleteNode(data.id,
                            function() {
                            $('#interface-tree').jstree('delete_node', data.id);
                        });
                    }
                    if (data.type == 'operation') {
                        InterfaceManager.deleteOperation(data.id,
                            function() {
                            $('#interface-tree').jstree('delete_node', data.id);
                        });

                    }

                }
            }
        };
        if (data.type == 'operation') {
            delete items.NewOperation;
            //   delete items.NewInterface;
        }
        return items;

    },

    types: {
        'interface': {
            "icon": "/Content/common-assets-includes/icons/interface.svg",
            "valid_children": ['operation']
        },
        'operation': {
            "icon": "/Content/common-assets-includes/icons/man-machine-circuit-symbol.svg",
            "valid_children": []
        }
    }
};

$(function () {
    ErrorEventManager.getTree()
        .then(function (data) {
            if (!Array.isArray(data)) return;
            var treeData = data.map(function (item) {
                return {
                    id: item.id,
                    parent: "#",
                    text: item.name,
                    type: 'error',
                    data: {
                        errorCode: item.errorCode,
                        namespaceId: item.namespaceId,
                        entityId: item.entityId
                    }
                }
            });
            ErrorTreeManager.instantiateTree(treeData);
            //ErrorEventManager.getNamespaces();
            ErrorEventManager.showRelatedEntities();
            InterfaceManager.fillMessagesAndErrors(null, data);
        });
    MessageEventManager.getTree()
        .then(function (data) {
            if (!Array.isArray(data)) return;
            var treeData = data.map(function (item) {
                return {
                    id: item.id,
                    parent: "#",
                    text: item.name,
                    type: 'message',
                    data: {
                        namespaceId: item.namespaceId,
                        entityId: item.entityId
                    }
                }
            });
            MessageTreeManager.instantiateTree(treeData);
            MessageEventManager.showRelatedEntities();
            //MessageEventManager.getNamespaces();
            InterfaceManager.fillMessagesAndErrors(data, null);

        });
    EscalateEventManager.getTree()
        .then(function (data) {
            if (!Array.isArray(data)) return;
            var treeData = data.map(function (item) {
                return {
                    id: item.id,
                    parent: "#",
                    text: item.name,
                    type: 'escalate',
                    data: {
                        escalationCode: item.escalationCode,
                        namespaceId: item.namespaceId,
                        entityId: item.entityId
                    }
                }
            });
            EscalateTreeManager.instantiateTree(treeData);
            //EscalateEventManager.getNamespaces();
            EscalateEventManager.showRelatedEntities();
        });
    SignalEventManager.getTree()
        .then(function (data) {
            if (!Array.isArray(data)) return;
            var treeData = data.map(function (item) {
                return {
                    id: item.id,
                    parent: "#",
                    text: item.name,
                    type: 'signal',
                    data: {
                        namespaceId: item.namespaceId,
                        entityId: item.entityId
                    }
                }
            });
            SignalTreeManager.instantiateTree(treeData);
            //SignalEventManager.getNamespaces();
            SignalEventManager.showRelatedEntities();
        });
    DataStoreManager.getTree()
        .then(function (data) {
            if (!Array.isArray(data)) return;
            var treeData = data.map(function (item) {
                return {
                    id: item.id,
                    parent: "#",
                    text: item.name,
                    type: 'datastore',
                    data: {
                        capacity: item.capacity,
                        isUnLimited: item.isUnLimited,
                        namespaceId: item.namespaceId,
                        entityId: item.entityId
                    }
                }
            });
            DataStoreTreeManager.instantiateTree(treeData);
            DataStoreManager.getNamespaces();
        });
    InterfaceManager.getTree()
        .then(function (data) {
            var operation = [];
            if (!Array.isArray(data)) return;
            var treeData = data.map(function (item) {
                return {
                    id: item.id,
                    parent: "#",
                    text: item.name,
                    type: 'interface',
                    data: {
                        operations: item.operations,
                        implementationRef: item.implementationRef
                    }
                }
            });
            for (var i = 0; i < data.length; i++) {
                if (data[i].operations != null) {
                    operation = operation.concat(data[i].operations.map(function (op) {
                        return {
                            id: op.id,
                            parent: data[i].id,
                            text: op.name,
                            type: 'operation',
                            data: {
                                interfaceId: op.interfaceId,
                                implementationRef: op.implementationRef,
                                inMessageRef: op.inMessageRef,
                                outMessageRef: op.outMessageRef,
                                errorRef: op.errorRef
                            }
                        }
                    }));
                }
            }
            treeData = treeData.concat(operation);
            InterfaceTreeManager.instantiateTree(treeData);
        });
});
