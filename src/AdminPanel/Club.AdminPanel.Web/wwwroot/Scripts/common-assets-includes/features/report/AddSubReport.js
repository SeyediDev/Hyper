var SubReportManager = function () {
	var openAddSubReportFrame = function (namespaceId, entityId, reportId, configId, type) {
		$("#SubReportModal .modal-body").html(
			"<iframe src='" +
			window.top.rootUrl +
			"Report/AddSubReport?EntityId=" +
			entityId +
			"&ReportId=" +
			reportId +
			"&NamespaceId=" +
			namespaceId +
			"&ConfigId=" +
			configId +
			"&type=" + type + "' style='width:100%; height:100%;' frameborder='0' allowtransparency='true'></iframe>");
	};
	var addInline = function (namespaceId, entityId, reportId, configId) {
		openAddSubReportFrame(namespaceId, entityId, reportId, configId, 2);
	};
	var addLinked = function (namespaceId, entityId, reportId, configId) {
		openAddSubReportFrame(namespaceId, entityId, reportId, configId, 1);
	};

	return {
		addInline: addInline,
		addLinked: addLinked
	};
}();
