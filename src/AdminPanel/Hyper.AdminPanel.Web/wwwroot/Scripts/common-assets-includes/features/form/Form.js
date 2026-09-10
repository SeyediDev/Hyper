window.submitCalled = false;
var alreadyBound = false;

var LaddaManager = function() {
	var $laddaButton = null;
	var startLadda = function(btn) {
		debugger;
		if (btn)
			$laddaButton = $(btn).ladda();
		if ($laddaButton)
			$laddaButton.ladda('start');
	};

	var stopLadda = function() {
		if ($laddaButton) {
			$laddaButton.ladda('stop');
		}
	};
	return {
		startLadda: startLadda,
		stopLadda: stopLadda
	}
}();

var MandatoryManager = function() {
	var passDoppleGangerRequireds = function() {
		$('table tr:nth-last-child(1)')
			.find('select, input')
			.each(function(idx, el) {
				if ($(el).closest('tr').attr('doppelganger')) {
					if ($(el).attr("required")) {
						$(el).removeAttr("required");
						$(el).attr("disabled", "disabled");
					}
				}
			});
	};
	var passHiddenRequireds = function() {
		$("div.dWrapper")
			.parent()
			.find("select, input")
			.each(function(idx, el) {
				if (!$(el).closest('tr').attr('doppelganger')) {
					if ($(el).attr("required") && $(el).closest("div.ShowHide").length > 0) {
						$(el).removeAttr("required");
						$(el).attr("disabled", "disabled");
					}
				}
			});
	};
	var passAllTableRequireds = function() {
		$("table")
			.find("select, input")
			.each(function(idx, el) {
				if (!$(el).closest('tr').attr('doppelganger')) {
					if ($(el).attr("required")) {
						$(el).removeAttr("required");
						//$(el).attr("disabled", "disabled");
					}
				}
			});
	};
	var pass = function(checkforEditableTable) {
		passDoppleGangerRequireds();
		passHiddenRequireds();
		if (checkforEditableTable) {
			passAllTableRequireds();
		}
		//    disabledPassCheckboxList();
	}

	return {
		pass: pass
	};
}();

var tableOp = (function() {
	var scrollDown = function($descendantElem) {
		var $container = $descendantElem.closest('.t-table-wrapper');
		$container.animate({ scrollTop: $container[0].scrollHeight });
	};

	var destroyInstantiatedControls = function($table) {
		var controls = $table.find("tbody tr:nth-last-child(1) :input");
		controls.each(function(idx, el) {
			var $el = $(el);
			if ($el.hasClass("select2-hidden-accessible")) {
				$el.select2('destroy');
			} else if ($el.hasClass("pwt-datepicker-input-element")) {
				el.parentElement.innerHTML = el.parentElement.innerHTML;
			}
		});
	};

	var addNewRow = function(span) {
		var tblName = $(span).attr("tablename");
		var table = $("table[name='" + tblName + "']");
		var itemIndex = $("table." + tblName + " tbody tr").length;
		console.log(itemIndex);
		destroyInstantiatedControls(table);
		var newTr = $(table).find("tbody tr:nth-last-child(1)").clone();
		var $newTr = $(newTr);
		$newTr.removeClass("ShowHide");
		$newTr.removeAttr("doppelganger");
		$newTr
			.find("td:last")
			.html(
				window.tetaI18n.t('NotSaved') +
				' | <span style="cursor:pointer;" onclick="tableOp.setDelColStatus(this)" class="text-danger icon-link form-link table-form-link" title="' +
				window.tetaI18n.t('Remove') +
				'"><i class="fa fa-times"></i></span>');
		$newTr
			.find("input, select, textarea")
			.each(function(idx, el) {
				var cellIndex = $(el).closest("td").index();
				var cName = $($(table).find("th")[cellIndex]).attr("fieldname");
				if (el.type == "hidden" && $(el).attr("removeStatus")) {
					cName = "__Deleted";
					$(el).attr("id", tblName + '_' + (itemIndex - 1) + cName);
				} else {
					if ($(el).attr("associated-hidden-name"))
						$(el).attr("id", 'field-' + tblName + '_' + (itemIndex - 1) + '__' + cName);
					else
						$(el).attr("id", tblName + '_' + (itemIndex - 1) + '__' + cName);
				}
				if ($(el).attr("name"))
					$(el).attr("name", tblName + '[' + (itemIndex - 1) + '].' + cName);
				if ($(el).attr("associated-hidden-name"))
					$(el).attr("associated-hidden-name", tblName + '[' + (itemIndex - 1) + '].' + cName);
			});
		$(table).find("tbody tr").eq(itemIndex - 1).before(newTr);

		$newTr
			.find('input.dateField')
			.each(function(idx, el) {
				PwtDatepickerBeneficiary.instantiatePwtDatepicker($(el));
			});
		$newTr.find('input.timeText')
			.each(function(idx, el) {
				Inputmask({ 'mask': '[99][:99][:99][:99][.999]', 'greedy': false }).mask(el);
			});
		if ($('select').select2) { //todo refactor it to use the same as logic.js.
			$newTr
				.find('select')
				.each(function(idx, el) {
					if ($(el).is('[data-isremote]')) {
						$(el)
							.select2({
								ajax: window.Select2Beneficiary.getAjaxObject(
									$(el).data('namespace'),
									$(el).data('entity'),
									$(el).data('column'),
									$(el).data('form'),
									Boolean($(this).attr("required")),
									$(el).data('remote-url'),
									$(el).attr('filter-formula')
								),
								templateResult: window.Select2Beneficiary.templateResult,
								templateSelection: window.Select2Beneficiary.templateSelection,
								width: '100%'
							});
					} else {
						$(el)
							.select2({ width: '100%' });
					}

				});
		}
		scrollDown(table);
	};

	var toggle = function(elem, tableName) {
		if (!tableName) return;
		$("table[name='" + tableName + "']").fadeToggle(400);
		if ($(elem).hasClass("fa-minus")) {
			$(elem).removeClass("fa-minus");
			$(elem).addClass("fa-plus");
		} else if ($(elem).hasClass("fa-plus")) {
			$(elem).removeClass("fa-plus");
			$(elem).addClass("fa-minus");
		}
	};

	var removeLastTrs = function() {
		$("table")
			.each(function(idx, el) {
				if ($(el).attr("editable")) {
					if ($(el).find("tbody tr:nth-last-child(1)").attr('doppelganger')) {
						$(el).find("tbody tr:nth-last-child(1)").remove();
					}
				}
			});
	};

	var setDelColStatus = function(span) {

		var $row = $(span).closest("tr");
		if (!$row.attr('identity')) {
			$row.remove();
			return;
		}

		var obj = $row.find("td.rowStatus").find("input");
		var val = obj.val();
		if (String(val) === "true") {
			val = "false";
			$row.css("backgroundColor", "");
		} else {
			val = "true";
			$row.css("backgroundColor", "#f5bbbb");
		}

		obj.val(val).trigger('change');


	};

	return {
		toggle: toggle,
		addNewRow: addNewRow,
		removeLastTrs: removeLastTrs,
		setDelColStatus: setDelColStatus
	};
})();


function addReturnUrlIfNeeded(type) {
	if (type && typeof type === 'string' && type.indexOf('AndReturn') !== -1 && !$('#returnUrl').val())
		$('#returnUrl').val(HistoryManager.getBackUrl());
}

function addAndRepeat(btn) {
	MandatoryManager.pass();
	causeSubmitForCreate(true, btn);
}

function ProcessCreateSave(btn) {
	var type = $(btn).data("ctype");
	MandatoryManager.pass(false);
	causeSubmitForCreate(false, btn, type);
}

function doSubmit(btn) {
	var type = null;
	if (btn) {
		type = $(btn).data("ctype");
		addReturnUrlIfNeeded(type);
	}
	MandatoryManager.pass(false);
	causeSubmitForCreate(false, btn, type ? type : null);
}

function apply(btn) {
	MandatoryManager.pass(false);
	causeSubmitnApply(btn);
}

function edit(btn) {
	var type = $(btn).data("etype");
	addReturnUrlIfNeeded(type);
	MandatoryManager.pass(true);
	$("input[name='isApply']").val("0");
	causeSubmit(btn, type);
	return true;
}

function causeSubmitnApply(btn) {
	$("input[name='isApply']").val("1");
	causeSubmit(btn);
}

function causeSubmitForCreate(addAgain, btn, type) {
	if (window.submitCalled)
		return;
	window.submitCalled = true;
	LaddaManager.startLadda(btn);
	tableOp.removeLastTrs();
	if (addAgain) {
		$("input[name='addAgain']").val(true);
	} else {
		$("input[name='addAgain']").val(false);
	}
	if (type) {
		$("input[name='CreateType']").val(type);
	}
	try {
		if (!$("input[name='addAgain']").closest("form")[0].checkValidity()) {
			// If the form is invalid, submit it. The form won't actually submit;
			// this will just cause the browser to display the native HTML5 error messages.
			window.submitCalled = false;
			LaddaManager.stopLadda();
			$("input[name='addAgain']").closest("form").find(':submit').click();
		} else {
			$("input[name='addAgain']").closest("form").find(':submit').click();
		}
	} catch (e) {
		$("input[name='addAgain']").closest("form").find(':submit').click();
	}
}

function causeSubmit(btn, type) {
	if (window.submitCalled)
		return;
	window.submitCalled = true;
	LaddaManager.startLadda(btn);
	if (type) {
		$("input[name='EditType']").val(type);
	}
	try {
		if (!$("input[name='EditType']").closest("form")[0].checkValidity()) {
			// If the form is invalid, submit it. The form won't actually submit;
			// this will just cause the browser to display the native HTML5 error messages.
			if (type == 'Save') { //Why just save Dear Jahangiri? important todo
				tableOp.removeLastTrs();
			}
			window.submitCalled = false;
			LaddaManager.stopLadda();
			$("input[name='EditType']").closest("form").find(':submit').click();
		} else {
			tableOp.removeLastTrs();
			$("input[name='EditType']").closest("form").find(':submit').click();
		}
	} catch (e) {
		$("input[name='EditType']").closest("form").find(':submit').click();
	}
}

var CreateProcess = function() {
	var applyAndGoTo = function(btn, page) {
		$('input[name="returnPage"]').val(page);
		apply(btn);
	};
	return {
		applyAndGoTo: applyAndGoTo
	};
}();

function del(btn) {
	var type = $(btn).data("dtype");
	addReturnUrlIfNeeded(type);
	MandatoryManager.pass(false);
	LaddaManager.startLadda(btn);
	window.submitCalled = true;
	$("input[name='DeleteType']").val(type);
	$("input[name='DeleteType']").closest("form").submit();
}

$('#edit-form').on('submit',
	function(e) {
		try {
			ControlBindingsManager.bindControls($(this));
		} catch (ex) {
			alert(ex);
			window.submitCalled = false;
			LaddaManager.stopLadda();
			alreadyBound = false;
			return false;
		}
		if (!submitCalled) {
			window.submitCalled = true;
			LaddaManager.startLadda();
			causeSubmit();
		}

		return true;
	});

$('#create-form').on('submit',
	function(e) {
		try {
			ControlBindingsManager.bindControls($(this));
		} catch (ex) {
			alert(ex);
			window.submitCalled = false;
			LaddaManager.stopLadda();
			alreadyBound = false;
			return false;
		}
		if (!submitCalled) {
			window.submitCalled = true;
			LaddaManager.startLadda();
			causeSubmitForCreate();
		}
		return true;
	});

window.ControlBindingsManager = function() {
	var bindableControlsOfThisForm = []; // list of JsBindingControls
	var
		tetaJsControls =
			{}; // Dictionary of controlTypes to objects of {obtainOutputData: function, provideInitialData: function}

	var getControl = function(controlId) {
		return $.grep(bindableControlsOfThisForm,
			function(control) {
				return control.id === controlId;
			})[0];
	};

	var getTableInputName = function(prefix, tableName, fieldName, idx) {
		return (prefix || tableName) + '[' + idx + ']' + '.' + fieldName;
	};

	var obtainDeletedRows = function(prefix, bindingInfo, data, initialData, result) {
		var deletedsCount = 0;
		$.each(initialData,
			function(idx, initialRow) {
				var i = 0;
				for (; i < data.length; i++) {
					if (initialRow.tetaId === data[i].tetaId)
						break;
				}
				if (i === data.length) {
					result.push({
						name: getTableInputName(prefix, bindingInfo.name, '__Ids', deletedsCount),
						value: initialRow.tetaId
					});
					result.push({
						name: getTableInputName(prefix, bindingInfo.name, '__Deleted', deletedsCount),
						value: true
					});
					deletedsCount++;
				}
			});
		return deletedsCount;
	}

	var getSubTableInitialData = function(initialData, tetaId, subTableDataField) {
		if (!initialData || !tetaId) return [];
		var arrayItem = initialData.find(function(item) { return item.tetaId === tetaId });
		if (!arrayItem) return [];
		return arrayItem[subTableDataField] || [];
	};

	var obtainTableNamesAndValues = function(prefix, bindingInfo, data, initialData) {
		if (!Array.isArray(data)) {
			console.error("only array data is bindable to a table");
			return [];
		}
		var result = [];
		var deletedsCount = obtainDeletedRows(prefix, bindingInfo, data, initialData, result);
		$.each(data,
			function(idx, dataArrayElement) {
				$.each(bindingInfo.tableInfo,
					function(unusedIndex, tableInfoItem) {
						var inputValue = dataArrayElement[tableInfoItem.relatedDataField];
						if (inputValue === undefined &&
							!(tableInfoItem.type === 'table' &&
							(getSubTableInitialData(initialData,
									dataArrayElement['tetaId'],
									tableInfoItem.relatedDataField).length >
								0))) {
							console.warn('The control doesn\'t provide ' +
								tableInfoItem.relatedDataField +
								' property for item ' +
								idx +
								'of table ' +
								bindingInfo.name);
							return;
						}
						var resultName =
							getTableInputName(prefix, bindingInfo.name, tableInfoItem.name, idx + deletedsCount);
						if (tableInfoItem.type === 'table') {
							var thisTableInitialData = getSubTableInitialData(initialData,
								dataArrayElement['tetaId'],
								tableInfoItem.relatedDataField);
							var thisTableResults = obtainTableNamesAndValues(resultName,
								tableInfoItem,
								inputValue || [],
								thisTableInitialData);
							result = result.concat(thisTableResults);
						} else {
							result.push({
								name: resultName,
								value: inputValue
							});
						}
					});
				if (dataArrayElement["tetaId"]) {
					result.push({
						name: getTableInputName(prefix, bindingInfo.name, '__Ids', idx + deletedsCount),
						value: dataArrayElement['tetaId']
					});
				}
			});
		return result;
	};

	var bindControls = function($form) { //todo hierarchical control output-data schema
		if (alreadyBound) return;
		else alreadyBound = true;
		$.each(bindableControlsOfThisForm,
			function(ctrlIdx, control) {
				var controlData = tetaJsControls[control.type].obtainOutputData(control.id);
				console.log('controlData', JSON.stringify(controlData));
				if (!controlData) return 'continue';
				$.each(control.bindingInfo,
					function(biIndex, bi) {
						switch (bi.type) {
						case "table":
							{
								var tableData;
								if (bi.relatedDataField === '')
									tableData = controlData;
								else
									tableData = controlData[bi.relatedDataField];
								if (tableData === undefined) {
									console.warn('The control doesn\'t provide ' + bi.relatedDataField + ' property.');
									return;
								}
								var tableNamesAndValues =
									obtainTableNamesAndValues('', bi, tableData, control.transformedInitialData);
								//todo initialData is too temporary. We should traverse to related table part in the InitialData                        
								$.each(tableNamesAndValues,
									function(tnvidx, tnv) {
										$form.append($("<input/>",
											{
												name: tnv.name,
												value: tnv.value,
												type: 'hidden'
											}));
									});
							}
							break;
						case "field":
							{
								var inputValue;
								if (bi.relatedDataField === '')
									inputValue = controlData;
								else
									inputValue = controlData[bi.relatedDataField];
								if (inputValue === undefined) {
									console.warn('The control doesn\'t provide ' + bi.relatedDataField + ' property.');
									return;
								}
								var alreadyExistingInput = $('input[name="' + bi.name + '"]');
								if (alreadyExistingInput.length)
									alreadyExistingInput.val(inputValue);
								else
									$form.append($("<input/>",
										{
											name: bi.name,
											value: inputValue,
											type: 'hidden'
										}));
							}
							break;
						case "file":
							console.error("Binding of file is not implemented.");
							break;
						default:
							console.error("Invalid binding type");
						}
					});
			});
	};


	var provideTableData = function(data, tableInfo) {
		var result = [];
		$.each(data,
			function(idx, rowJson) {
				var iterationResult = {};
				var row = typeof (rowJson) === 'string' ? JSON.parse(rowJson) : rowJson;
				$.each(tableInfo,
					function(tidx, col) {
						if (col.type === 'field') {
							iterationResult[col.relatedDataField] = row[col.name];
						} else if (col.type === 'table') {
							iterationResult[col.relatedDataField] = provideTableData(row[col.name], col.tableInfo);
						}
					});
				iterationResult['tetaId'] = row['Id'];
				iterationResult['id'] = row['Id']; //todo hardcoded id name
				result.push(iterationResult);
			});
		return result;
	};

	var provideBindingItemData = function(bindingInfo, control) {
		if (bindingInfo.type === "table") {
			return provideTableData(control.initialData[bindingInfo.name],
				bindingInfo.tableInfo);
		} else if (bindingInfo.type === "field") {
			return control.initialData[bindingInfo.name];
		}
		throw new Error('Not implemented bindingInfo.type ' + bindingInfo.type);
	};

	var provideInitialData = function(theControl) {
		var result;
		$.each(theControl.bindingInfo,
			function(idx, bi) {
				if (bi.relatedDataField === "") {
					if (result !== undefined) {
						throw new Error("Only one binding info makes sense when a relatedDataField is empty.");
					}
					result = provideBindingItemData(bi, theControl);
				} else {
					result = result || {};
					result[bi.relatedDataField] = provideBindingItemData(bi, theControl);
				}
			});
		return result;
	};

	var getInitialData = function(controlId) {
		return getControl(controlId).controlInput;
	};

	var registerJsControlIO = function(name, ioObject) {
		if (typeof (ioObject.obtainOutputData) !== 'function')
			throw new Error('ioObject should have obtainOutputData');
		if (typeof (ioObject.provideInitialData) !== 'function')
			throw new Error('ioObject should have provideInitialData');
		tetaJsControls[name] = ioObject;
	};

	var addJsControl = function(control) {
		control['transformedInitialData'] =
			tetaJsControls[control.type].provideInitialData(provideInitialData(control));
		control['controlInput'] =
			tetaJsControls[control.type].provideInitialData(provideInitialData(control));
		bindableControlsOfThisForm.push(control);
	};

	return {
		bindControls: bindControls,
		getInitialData: getInitialData,
		registerJsControlIO: registerJsControlIO,
		addJsControl: addJsControl
	};
}();

$('form').areYouSure({
	'message': window.tetaI18n.t('YourChangesNotSaved')
//    ,'addRemoveFieldsMarksDirty': true
});

function toggleFieldEnable(el, fieldName, hasAssociatedHidden) {
	var isChecked = $(el).prop('checked');
	$('[name="' + fieldName + '"]').prop('disabled', !isChecked);
	if (hasAssociatedHidden)
		$('[associated-hidden-name="' + fieldName + '"]').prop('disabled', !isChecked);
}

window.FormFileManager = function() {
	var eSubmitAction = { // Equivalent to FileSubmitAction enum in RunFileOperations.cs of Engine.Entities
		Nothing: 'Nothing',
		Move: 'Move',
		Upload: 'Upload',
		Remove: 'Remove'
	};

	var checkIfPass = function(files) {
		if (!files) return false;
		for (var i = 0; i < files.length; i++) {
			var file = files[i];
			var fileExt = file.name.split('.')[file.name.split('.').length - 1];
			var fileExtStr = fileExt.toLowerCase();
			if (fileExtStr === "bat" ||
				fileExtStr === "bin" ||
				fileExtStr === "cmd" ||
				fileExtStr === "com" ||
				fileExtStr === "cpl" ||
				fileExtStr === "exe" ||
				fileExtStr === "gadget" ||
				fileExtStr === "inf1" ||
				fileExtStr === "ins" ||
				fileExtStr === "inx" ||
				fileExtStr === "isu" ||
				fileExtStr === "job" ||
				fileExtStr === "jse" ||
				fileExtStr === "lnk" ||
				fileExtStr === "msc" ||
				fileExtStr === "msi" ||
				fileExtStr === "msp" ||
				fileExtStr === "mst" ||
				fileExtStr === "paf" ||
				fileExtStr === "pif" ||
				fileExtStr === "ps1" ||
				fileExtStr === "reg" ||
				fileExtStr === "rgs" ||
				fileExtStr === "sct" ||
				fileExtStr === "shb" ||
				fileExtStr === "shs" ||
				fileExtStr === "u3p" ||
				fileExtStr === "vb" ||
				fileExtStr === "vbe" ||
				fileExtStr === "vbs" ||
				fileExtStr === "vbscript" ||
				fileExtStr === "ws" ||
				fileExtStr === "wsf" ||
				file.type === "application/x-msdownload") {
				return false;
			}
		}
		return true;
	};

	//    var getActionSelector = function(fieldId) {
	//        return 'input[name="' + fieldId + '__Action"]';
	//    }
	//
	//    var getSubmitAction = function(fieldId) {
	//        $(getActionSelector(fieldId)).val();
	//    };

	var setSubmitAction = function(fieldId, action) {
		$('input[name="' + fieldId + '__Action"]').val(action);
	};

	var toggleRemoveButton = function(fieldId) {
		$('[id="' + fieldId + '-undo-btn"], [id="' + fieldId + '-remove-btn"]').toggleClass('hidden');
	};
	var toggleRemovingFileAppearance = function(fieldId) {
		$('[id="' + fieldId + '-file-content"]').toggleClass('removing-file');
	};

	var fileValueChanged = function(fileInput) {
		var files = fileInput.files;
		var pass = true;
		pass = checkIfPass(files);
		if (pass) {
			var reader = new FileReader();
			reader.readAsDataURL(files[0]);
			reader.onload = function(evt) {
				//            var index = evt.target.result.indexOf('base64,') + 7;
				var s = "";
				s = evt.target.result;
				if (s) {
					var ownerName = $(fileInput).attr("ownername");
					var owner = $("input[name='" + ownerName + "']");
					var fileNameObj = $(fileInput).val().split('\\');
					var fileName = "";
					if (fileNameObj[fileNameObj.length - 1].length > 15) {
						fileName = fileNameObj[fileNameObj.length - 1].substr(0, 15) + '...';
					} else {
						fileName = fileNameObj[fileNameObj.length - 1];
					}
					$(owner).val(s + '|' + fileNameObj[fileNameObj.length - 1]);
					$(fileInput).closest('div.dWrapper').find('label')
						.attr('title', fileNameObj[fileNameObj.length - 1]);
					$(fileInput).closest('div.dWrapper').find('label').html('File : ' + fileName);
					setSubmitAction(ownerName, eSubmitAction.Upload);
				}
			}
			reader.onerror = function(evt) {
				window.toast.error('وقوع خطا در انتخاب فایل');
			}
		} else {
			window.toast.error(window.tetaI18n.t('UploadForbiddenFileFormat'));
			$(fileInput).value(null);
		}
	};

	var removeFile = function(fieldId) {
		setSubmitAction(fieldId, eSubmitAction.Remove);
		toggleRemoveButton(fieldId);
		toggleRemovingFileAppearance(fieldId);
	};

	var undoRemove = function(fieldId) {
		setSubmitAction(fieldId, eSubmitAction.Nothing);
		toggleRemoveButton(fieldId);
		toggleRemovingFileAppearance(fieldId);
	};

	return {
		fileValueChanged: fileValueChanged,
		removeFile: removeFile,
		undoRemove: undoRemove,
		eSubmitAction: eSubmitAction
	};
}();