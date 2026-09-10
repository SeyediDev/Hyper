/* ================================================
 * Make use of Twitter Bootstrap's modal more monkey-friendly
 * 
 * For Bootstrap 3.
 * 
 * javanoob@hotmail.com
 * 
 * Licensed under The MIT License.
 * ================================================ */
var BootstrapDialog = null;
!function($) {
    "use strict";

    BootstrapDialog = function(options) {
        this.defaultOptions = {
            type: BootstrapDialog.TYPE_PRIMARY,
            size: BootstrapDialog.SIZE_NORMAL,
            title: null,
            message: null,
            buttons: [],
            closable: true,
            spinicon: BootstrapDialog.ICON_SPINNER,
            data: {},
            onshow: null,
            onhide: null,
            autodestroy: true
        };
        this.indexedButtons = {};
        this.realized = false;
        this.initOptions(options);
    };

    BootstrapDialog.NAMESPACE = 'bootstrap-dialog';

    BootstrapDialog.TYPE_DEFAULT = 'type-default';
    BootstrapDialog.TYPE_INFO = 'type-info';
    BootstrapDialog.TYPE_PRIMARY = 'type-primary';
    BootstrapDialog.TYPE_SUCCESS = 'type-success';
    BootstrapDialog.TYPE_WARNING = 'type-warning';
    BootstrapDialog.TYPE_DANGER = 'type-danger';

    BootstrapDialog.DEFAULT_TEXTS = {};
    BootstrapDialog.DEFAULT_TEXTS[BootstrapDialog.TYPE_DEFAULT] = '';
    BootstrapDialog.DEFAULT_TEXTS[BootstrapDialog.TYPE_INFO] = '';
    BootstrapDialog.DEFAULT_TEXTS[BootstrapDialog.TYPE_PRIMARY] = '';
    BootstrapDialog.DEFAULT_TEXTS[BootstrapDialog.TYPE_SUCCESS] = '';
    BootstrapDialog.DEFAULT_TEXTS[BootstrapDialog.TYPE_WARNING] = '';
    BootstrapDialog.DEFAULT_TEXTS[BootstrapDialog.TYPE_DANGER] = '';

    BootstrapDialog.SIZE_NORMAL = 'size-normal';
    BootstrapDialog.SIZE_LARGE = 'size-large';

    BootstrapDialog.BUTTON_SIZES = {};
    BootstrapDialog.BUTTON_SIZES[BootstrapDialog.SIZE_NORMAL] = '';
    BootstrapDialog.BUTTON_SIZES[BootstrapDialog.SIZE_LARGE] = 'btn-lg';

    BootstrapDialog.ICON_SPINNER = 'glyphicon glyphicon-asterisk';

    BootstrapDialog.prototype = {
        constructor: BootstrapDialog,
        initOptions: function(options) {
            this.options = $.extend(true, this.defaultOptions, options);

            return this;
        },
        initModalStuff: function() {
            this.setModal(this.createModal())
                    .setModalDialog(this.createModalDialog())
                    .setModalContent(this.createModalContent())
                    .setModalHeader(this.createModalHeader())
                    .setModalBody(this.createModalBody())
                    .setModalFooter(this.createModalFooter());

            this.getModal().append(this.getModalDialog());
            this.getModalDialog().append(this.getModalContent());
            this.getModalContent()
                    .append(this.getModalHeader())
                    .append(this.getModalBody())
                    .append(this.getModalFooter());

            return this;
        },
        createModal: function() {
            return $('<div class="modal fade" tabindex="-1"></div>');
        },
        getModal: function() {
            return this.$modal;
        },
        setModal: function($modal) {
            this.$modal = $modal;

            return this;
        },
        createModalDialog: function() {
            return $('<div class="modal-dialog modal-xl"></div>');
        },
        getModalDialog: function() {
            return this.$modalDialog;
        },
        setModalDialog: function($modalDialog) {
            this.$modalDialog = $modalDialog;

            return this;
        },
        createModalContent: function() {
            return $('<div class="modal-content"></div>');
        },
        getModalContent: function() {
            return this.$modalContent;
        },
        setModalContent: function($modalContent) {
            this.$modalContent = $modalContent;

            return this;
        },
        createModalHeader: function() {
            return $('<div class="modal-header"></div>');
        },
        getModalHeader: function() {
            return this.$modalHeader;
        },
        setModalHeader: function($modalHeader) {
            this.$modalHeader = $modalHeader;

            return this;
        },
        createModalBody: function() {
            return $('<div class="modal-body"></div>');
        },
        getModalBody: function() {
            return this.$modalBody;
        },
        setModalBody: function($modalBody) {
            this.$modalBody = $modalBody;

            return this;
        },
        createModalFooter: function() {
            return $('<div class="modal-footer"></div>');
        },
        getModalFooter: function() {
            return this.$modaFooter;
        },
        setModalFooter: function($modaFooter) {
            this.$modaFooter = $modaFooter;

            return this;
        },
        createDynamicContent: function(rawContent) {
            var contentType = typeof rawContent;
            if (contentType === 'function') {
                return rawContent.call(rawContent, this);
            }

            return rawContent;
        },
        setData: function(key, value) {
            this.options.data[key] = value;

            return this;
        },
        getData: function(key) {
            return this.options.data[key];
        },
        getType: function() {
            return this.options.type;
        },
        setType: function(type) {
            this.options.type = type;

            return this;
        },
        getSize: function() {
            return this.options.size;
        },
        setSize: function(size) {
            this.options.size = size;

            return this;
        },
        getTitle: function() {
            return this.options.title;
        },
        setTitle: function(title) {
            this.options.title = title;

            return this;
        },
        getMessage: function() {
            return this.options.message;
        },
        setMessage: function(message) {
            this.options.message = message;

            return this;
        },
        isClosable: function() {
            return this.options.closable;
        },
        setClosable: function(closable) {
            this.options.closable = closable;
            this.updateClosable();

            return this;
        },
        getSpinicon: function() {
            return this.options.spinicon;
        },
        setSpinicon: function(spinicon) {
            this.options.spinicon = spinicon;

            return this;
        },
        addButton: function(button) {
            this.options.buttons.push(button);

            return this;
        },
        addButtons: function(buttons) {
            var that = this;

            $.each(buttons, function(index, button) {
                that.addButton(button);
            });

            return this;
        },
        getButtons: function() {
            return this.options.buttons;
        },
        setButtons: function(buttons) {
            this.options.buttons = buttons;

            return this;
        },
        /**
         * If there is id provided for a button option, it will be in dialog.indexedButtons list.
         * 
         * In that case you can use dialog.getButton(id) to find the button.
         * 
         * @param {type} id
         * @returns {undefined}
         */
        getButton: function(id) {
            if (typeof this.indexedButtons[id] !== 'undefined') {
                return this.indexedButtons[id];
            }

            return null;
        },
        getButtonSize: function() {
            if (typeof BootstrapDialog.BUTTON_SIZES[this.getSize()] !== 'undefined') {
                return BootstrapDialog.BUTTON_SIZES[this.getSize()];
            }

            return '';
        },
        isAutodestroy: function() {
            return this.options.autodestroy;
        },
        setAutodestroy: function(autodestroy) {
            this.options.autodestroy = autodestroy;
        },
        getDefaultText: function() {
            return BootstrapDialog.DEFAULT_TEXTS[this.getType()];
        },
        getNamespace: function(name) {
            return BootstrapDialog.NAMESPACE + '-' + name;
        },
        createHeaderContent: function() {
            var $container = $('<div></div>');
            $container.addClass(this.getNamespace('header'));

            // title
            $container.append(this.createTitleContent());

            // Close button
            if (this.isClosable()) {
                $container.append(this.createCloseButton());
            }

            return $container;
        },
        createTitleContent: function() {
            var $title = $('<div></div>');
            $title.addClass(this.getNamespace('title'));
            $title.append(this.getTitle() !== null ? this.createDynamicContent(this.getTitle()) : this.getDefaultText());

            return $title;
        },
        createCloseButton: function() {
            var $container = $('<div></div>');
            $container.addClass(this.getNamespace('close-button'));
            var $icon = $('<button class="close">×</button>');
            $container.append($icon);
            $container.on('click', {dialog: this}, function(event) {
                event.data.dialog.close();
            });

            return $container;
        },
        createBodyContent: function() {
            var $container = $('<div></div>');
            $container.addClass(this.getNamespace('body'));

            // Message
            $container.append(this.createMessageContent());

            return $container;
        },
        createMessageContent: function() {
            var $message = $('<div></div>');
            $message.addClass(this.getNamespace('message'));
            $message.append(this.createDynamicContent(this.getMessage()));

            return $message;
        },
        createFooterContent: function() {
            var $container = $('<div></div>');
            $container.addClass(this.getNamespace('footer'));

            // Buttons
            $container.append(this.createFooterButtons());

            return $container;
        },
        createFooterButtons: function() {
            var that = this;
            var $container = $('<div></div>');
            $container.addClass(this.getNamespace('footer-buttons'));
            this.indexedButtons = {};
            $.each(this.options.buttons, function(index, button) {
                var $button = that.createButton(button);
                if (typeof button.id !== 'undefined') {
                    that.indexedButtons[button.id] = $button;
                }
                $container.append($button);
            });

            return $container;
        },
        createButton: function(button) {
            var $button = $('<button class="btn"></button>');
            $button.addClass(this.getButtonSize());

            // Icon
            if (typeof button.icon !== undefined && $.trim(button.icon) !== '') {
                $button.append(this.createButtonIcon(button.icon));
            }

            // Label
            if (typeof button.label !== undefined) {
                $button.append(button.label);
            }

            // Css class
            if (typeof button.cssClass !== undefined && $.trim(button.cssClass) !== '') {
                $button.addClass(button.cssClass);
            } else {
                $button.addClass('btn-default');
            }

            // Button on click
            $button.on('click', {dialog: this, button: button}, function(event) {
                var dialog = event.data.dialog;
                var button = event.data.button;
                if (typeof button.action === 'function') {
                    button.action.call(this, dialog);
                }

                if (button.autospin) {
                    var $button = $(this);
                    $button.find('.' + dialog.getNamespace('button-icon')).remove();
                    $button.prepend(dialog.createButtonIcon(dialog.getSpinicon()).addClass('icon-spin'));
                }
            });

            return $button;
        },
        createButtonIcon: function(icon) {
            var $icon = $('<span></span>');
            $icon.addClass(this.getNamespace('button-icon')).addClass(icon);

            return $icon;
        },
        /**
         * Invoke this only after the dialog is realized.
         * 
         * @param {type} enable
         * @returns {undefined}
         */
        enableButtons: function(enable) {
            var $buttons = this.getModalFooter().find('.btn');
            $buttons.prop("disabled", !enable).toggleClass('disabled', !enable);

            return this;
        },
        /**
         * Invoke this only after the dialog is realized.
         * 
         * @param {type} enable
         * @returns {undefined}
         */
        updateClosable: function() {
            if (this.isRealized()) {
                // Backdrop, I did't find a way to change bs3 backdrop option after the dialog is poped up, so here's a new wheel.
                var $theBigMask = this.getModal();
                $theBigMask.off('click').on('click', {dialog: this}, function(event) {
                    event.target === this && event.data.dialog.isClosable() && event.data.dialog.close();
                });

                // Close button
                this.getModalHeader().find('.' + this.getNamespace('close-button')).toggle(this.isClosable());

                // ESC key support
                $theBigMask.off('keyup').on('keyup', {dialog: this}, function(event) {
                    event.which === 27 && event.data.dialog.isClosable() && event.data.dialog.close();
                });
            }

            return this;
        },
        /**
         * Set handler for modal event 'show'.
         * This is a setter!
         * 
         * @param {type} onopen
         * @returns {_L9.BootstrapDialog.prototype}
         */
        onShow: function(onshow) {
            this.options.onshow = onshow;

            return this;
        },
        /**
         * Set handler for modal event 'hide'.
         * This is a setter!
         * 
         * @param {type} onclose
         * @returns {_L9.BootstrapDialog.prototype}
         */
        onHide: function(onhide) {
            this.options.onhide = onhide;

            return this;
        },
        isRealized: function() {
            return this.realized;
        },
        setRealized: function(realized) {
            this.realized = realized;

            return this;
        },
        handleModalEvents: function() {
            this.getModal().on('show.bs.modal', {dialog: this}, function(event) {
                var dialog = event.data.dialog;
                typeof dialog.options.onshow === 'function' && dialog.options.onshow(dialog);
            });
            this.getModal().on('hide.bs.modal', {dialog: this}, function(event) {
                var dialog = event.data.dialog;
                typeof dialog.options.onhide === 'function' && dialog.options.onhide(dialog);
            });
            this.getModal().on('hidden.bs.modal', {dialog: this}, function(event) {
                var dialog = event.data.dialog;
                dialog.isAutodestroy() && $(this).remove();
            });

            return this;
        },
        realize: function() {
            this.initModalStuff();
            this.getModal().addClass(BootstrapDialog.NAMESPACE)
                    .addClass(this.getType())
                    .addClass(this.getSize());
            this.getModalHeader().append(this.createHeaderContent());
            this.getModalBody().append(this.createBodyContent());
            this.getModalFooter().append(this.createFooterContent());
            this.getModal().modal({
                backdrop: 'static',
                keyboard: false
            });
            this.handleModalEvents();
            this.setRealized(true);

            return this;
        },
        open: function() {
            !this.isRealized() && this.realize();
            this.updateClosable();
            this.getModal().modal('show');

            return this;
        },
        close: function() {
            this.getModal().modal('hide');

            return this;
        }
    };

    /* ================================================
     * For lazy people
     * ================================================ */

    /**
     * Shortcut function: show
     * 
     * @param {type} options
     * @returns {undefined}
     */
    BootstrapDialog.show = function(options) {
        new BootstrapDialog(options).open();
    };

    /**
     * Alert window
     * 
     * @param {type} message
     * @param {type} callback
     * @returns {undefined}
     */
    BootstrapDialog.alert = function(message, callback) {
        new BootstrapDialog({
            message: message,
            data: {
                'callback': callback
            },
            closable: false,
            buttons: [{
                    label: 'OK',
                    action: function(dialog) {
                        typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(true);
                        dialog.close();
                    }
                }]
        }).open();
    };

    /**
     * Confirm window
     * 
     * @param {type} message
     * @param {type} callback
     * @returns {undefined}
     */
    BootstrapDialog.confirm = function(message, callback) {
        new BootstrapDialog({
            title: 'Confirmation',
            message: message,
            closable: false,
            data: {
                'callback': callback
            },
            buttons: [{
                    label: 'Cancel',
                    action: function(dialog) {
                        typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(false);
                        dialog.close();
                    }
                }, {
                    label: 'OK',
                    cssClass: 'btn-primary',
                    action: function(dialog) {
                        typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(true);
                        dialog.close();
                    }
                }]
        }).open();
    };
}(window.jQuery);

function clearOutDrillDowns() {
	$("#myDrillDownMenu")
		.each(function(index, el) {
			$(el).slideUp();
		});
}


window.onclick = function() {
	clearOutDrillDowns();
};

function FetchParentReportIds(ids) {
	var prevParentReportIds = $('input[name="ParentReportIds"]').val();
	var parentReportIdsSuffix = prevParentReportIds ? ("," + prevParentReportIds) : "";
	return ids + parentReportIdsSuffix;
}
function OpenDrillDownModal(nameSpaceId, entityId, reportId, configId, ids, title, parentFilterValues) {
	var e = window.event;
	try {
		e.stopImmediatePropagation();
		e.preventDefault();
	} catch (e) {
	}
	clearOutDrillDowns();
	var param = [];
	param.push("NamespaceId=" + nameSpaceId);
	param.push("EntityId=" + entityId);
	param.push("ReportId=" + reportId);
	param.push("ConfigId=" + configId);
	param.push("ParentReportIds=" + FetchParentReportIds(ids));
	param.push("Page=1");
	param.push("DrillDown=1");
	parentFilterValues = encodeURIComponent(JSON.stringify(parentFilterValues)).replace(/'/g, '%27');
	param.push("ParentFilterValues=" + parentFilterValues);
	var paramStr = param.join("&");
	var $textAndPic = $('<div style="width:99%;height:100%;"></div>');
	$textAndPic.append("<iframe src='" +
		window.top.rootUrl +
		"Report/Index?" +
		paramStr +
		"' style='width:100%; height:100%;' frameborder='0' allowTransparency='true'></iframe>");

	var dialog = new window.top.BootstrapDialog({
		title: title,
		message: $textAndPic
	});
	dialog.realize();
	dialog.getModalBody().css("width", "100%");
	var dialogHeight = screen.availHeight * 0.7;
	dialog.getModalBody().css("height", dialogHeight + "px");
	dialog.getModalDialog().css("width", "90%");
	dialog.getModalBody().find(".bootstrap-dialog-body").css("width", "100%");
	dialog.getModalBody().find(".bootstrap-dialog-body").css("height", "100%");
	dialog.getModalBody().find(".bootstrap-dialog-message").css("width", "100%");
	dialog.getModalBody().find(".bootstrap-dialog-message").css("height", "100%");
	dialog.open();
}

function openSubReportFrame(tr, e) {
	var sReportCount = $(tr).attr("sReportCount");
	var evt = e || window.event;
	try {
		evt.stopImmediatePropagation();
		evt.preventDefault();
	} catch (e) {
	}
	var ids = $(tr).attr("Ids");
	var reportKey = $(tr).closest("table").attr("key");
	var parentFilterValues = window.top.reportInfo[reportKey]["parentFilter"];
	openSubReportFrame2(sReportCount, ids, reportKey, parentFilterValues, tr, evt);
}

function getDashboardHref(subReportNamespaceId,
	subReportEntityId,
	parentReportId,
	parentReportConfigId,
	dashboardId,
	dashboardConfigId,
	ids,
	parentFilterValues
) {
	return "dashboard?NamespaceId=" +
		subReportNamespaceId +
		"&EntityId=" +
		subReportEntityId +
		"&DashboardId=" +
		dashboardId +
		"&ConfigId=" +
		dashboardConfigId +
		"&ParentReportId=" +
		parentReportId +
		"&ParentReportConfigId=" +
		parentReportConfigId +
		"&ParentReportIds=" +
		FetchParentReportIds(ids) +
		"&ParentFilterValues=" +
		parentFilterValues;
}

var bindOpenDrillDownClickEvent = function($a,
	subConfigName,
	namespaceId,
	entityId,
	subReportReportId,
	subConfigId,
	ids,
	parentFilterValues) {
	$a.on('click',
		function() {
			var ii = $(this).attr('index');
			OpenDrillDownModal(
				namespaceId,
				entityId,
				subReportReportId[ii],
				subConfigId[ii],
				ids,
				subConfigName,
				parentFilterValues);
		});
};

function openSubReportFrame2(sReportCount, ids, reportKey, parentFilterValues, obj, evt) {
	var reportInfo = window.top.reportInfo[reportKey];
	var namespaceId = reportInfo["NamespaceId"];
	var entityId = reportInfo["EntityId"];

	var subReportNamespaceId = window.top.subReportNamespaceIds[reportKey];
	var subReportEntityId = window.top.subReportEntityIds[reportKey];
	var subReportReportId = window.top.subReportReportIds[reportKey];
	var subConfigId = window.top.subConfigIds[reportKey];
	var reportViewType = reportInfo["ReportViewType"];
	var dashboardId = reportInfo["DashboardId"];
	var dashboardConfigId = reportInfo["DashboardConfigId"];
	if (+sReportCount === 1) {
		if (reportViewType === 4 || reportViewType === "Dashboard") {
			window.PageAddressManager.navigateTo(getDashboardHref(subReportNamespaceId,
				subReportEntityId,
				subReportReportId,
				subConfigId,
				dashboardId,
				dashboardConfigId,
				ids,
				parentFilterValues));
		} else {
			OpenDrillDownModal(subReportNamespaceId,
				subReportEntityId,
				subReportReportId,
				subConfigId,
				ids,
				window.top.subReportConfigNames[reportKey],
				parentFilterValues);
		}
	} else if (+sReportCount > 1) {
		subReportNamespaceId = subReportNamespaceId.split(",");
		subReportEntityId = subReportEntityId.split(",");
		subReportReportId = subReportReportId.split(",");
		subConfigId = subConfigId.split(",");

		reportViewType = reportViewType.split(",");
		dashboardId = dashboardId.split(",");
		dashboardConfigId = dashboardConfigId.split(",");
		var subConfigNames = window.top.subReportConfigNames[reportKey].split(",");
		$("#myDrillDownMenu").html("");
		for (var i = 0, len = subConfigId.length; i < len; i++) {
			if (reportViewType[i] === "Dashboard") {
				var hrf = getDashboardHref(subReportNamespaceId[i],
					subReportEntityId[i],
					subReportReportId[i],
					subConfigId[i],
					dashboardId[i],
					dashboardConfigId[i],
					ids,
					parentFilterValues);
				$("#myDrillDownMenu")
					.append("<a class='drill-item' " +
						"href='" +
						hrf +
						"'>" +
						subConfigNames[i] +
						"</a>");
			} else {
				var $a = $("<a class='drill-item' index='" + i + "'>")
					.text(subConfigNames[i]);
				bindOpenDrillDownClickEvent($a,
					subConfigNames[i],
					namespaceId,
					entityId,
					subReportReportId,
					subConfigId,
					ids,
					parentFilterValues);
				$a.appendTo("#myDrillDownMenu");
			}
		}
		//var y = $(obj).position();
		var posX = obj.offsetLeft;
		var posY = obj.offsetTop;
		var co = 1;
		while (obj.offsetParent) {
			posX = posX + obj.offsetParent.offsetLeft;
			//if (co == 6 || co == 7)
			//    posY = posY;
			//else
			posY = posY + obj.offsetParent.offsetTop;
			if (obj === document.getElementsByTagName("body")[0]) {
				break;
			} else {
				obj = obj.offsetParent;
			}
			++co;
		}

		$("#myDrillDownMenu")
			.css("top", ((evt && evt.pageY) ? evt.pageY : posY) + 20)
			.css("left", ((evt && evt.pageX) ? evt.pageX : posX))
            .css("z-index", 9999999999)
            .css("display", "flex")
			.show();
	}
}