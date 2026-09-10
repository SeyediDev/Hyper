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
			.show();
	}
}