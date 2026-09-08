window.FormUtils = function() {
	var getDefaultValue = function($input) {
		return $input.data('default-value');
	};

	var trySetOptionIfExists = function($select, optionValue) {
		if ($select.find('option[value="' + optionValue + '"]').length > 0) {
			$select.val(optionValue);
			return true;
		}
		return false;
	};

	var clearSelect = function($select) {
		if ($select.is('[multiple]')) {
			$select.find('option:selected').removeAttr("selected");
		} else {
			var hadOptionalValue = trySetOptionIfExists($select, '0') || trySetOptionIfExists($select, '');
			if (!hadOptionalValue /*&& $select.is('[nodatamandatory]')*/) {
				$select.prop('selectedIndex', -1);
			}
		}
		var defaultValue = getDefaultValue($select);
		if (defaultValue)
			$select.val(defaultValue);
		$select.trigger('change');
	};

	var clearInput = function($input) {
		$input.val('');
		$input.trigger('change');
	};

	var setButtonActiveIfExisted = function($input, value) {
		var $buttonLabel = $input.parent('label.btn');
		$buttonLabel.removeClass('active');
		if (value) {
			$buttonLabel.addClass('active');
		}
	};

	var clearCheckBoxOrRadio = function($input) {
		var defaultValue = getDefaultValue($input);
		if (defaultValue === undefined) {
			$input.removeAttr('checked').removeAttr('selected');
			setButtonActiveIfExisted($input, false);
		} else if (defaultValue === 'checked') {
			$input.prop('checked', 'checked');
			setButtonActiveIfExisted($input, true);
		} else if (defaultValue === 'selected') {
			$input.prop('selected', 'selected');
		} else {
			console.error('Invalid default value');
			return;
		}
		$input.trigger('change');
	};

	var clearField = function($field) {
		if ($field.is('input:text, input:password, input:file, textarea, input[type="number"]'))
			clearInput($field);
		else if ($field.is('input:radio, input:checkbox'))
			clearCheckBoxOrRadio($field);
		else if ($field.is('select'))
			clearSelect($field);
	};

	var clearFields = function($fields) {
		$fields.each(function(idx, input) {
			clearField($(input));
		});
	};

	var clearFieldsOf = function(selector) {
		var $form = $(selector);
		var $fields = $form.find(
			'input:text, input:password, input:file, textarea, input[type="number"], input:radio, input:checkbox, select');
		clearFields($fields);
	};

	var getSelectValue = function($select) {
		if ($select.attr('data-isremote') && $select.attr('initvalue') && !$select.attr('init-fetched')) {
			return $select.attr('initvalue');
		}
		return $select.val();
		
	};

	return {
		clearField: clearField,
		clearFields: clearFields,
		clearFieldsOf: clearFieldsOf,
		getSelectValue: getSelectValue
	};
}();

var OnloadUiRulesManager = function() {
	var ajaxIsRunning = false;
	var alreadyLoaded = false;

	var setAjaxRunning = function(b) {
		ajaxIsRunning = b;
		window.MainPartLoadingManager.setShouldWait(b);
	};

	var thingsToDoAfterUiRulesAreDone = function() {
		$('form').trigger('reinitialize.areYouSure');
		window.MainPartLoadingManager.showMain();
	};

	var rulesAreDone = function() {
		if (ajaxIsRunning) {
			return;
		}
		if (alreadyLoaded) {
			console.warn('Already loaded!');
			return;
		}
		alreadyLoaded = true;
		thingsToDoAfterUiRulesAreDone();
	};

	var ajaxRulesAreDone = function() {
		setAjaxRunning(false);
		rulesAreDone();
	};

	return {
		rulesAreDone: rulesAreDone,
		setAjaxRunning: setAjaxRunning,
		ajaxRulesAreDone: ajaxRulesAreDone
	}
}();

lc = {};

function runAjax(NamespaceId, EntityId, FormId, source, eventName, cb, scope, paramList) {
	OnloadUiRulesManager.setAjaxRunning(true)
	var AjaxParams = {
		source: source ? source : "",
		eventName: eventName ? eventName : "",
		NamespaceId: NamespaceId ? NamespaceId : "",
		EntityId: EntityId ? EntityId : "",
		FormId: FormId ? FormId : "",
		Qs: getQs(paramList),
		LCs: getLCs()
	}
	var obj = JSON.stringify(AjaxParams);
	$.ajax({
		type: 'POST',
		url: window.top.rootUrl + 'Form/GetOperations',
		data: obj,
		headers: AddAntiForgeryToken(),
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		async: true,
		processData: true,
		cache: false,
		success: function(data) {
			try {
				if (data)
					cb(data, scope);
			} catch (e) {
				console.log(e);
			}
			OnloadUiRulesManager.ajaxRulesAreDone();
		},
		error: function(xhr) {
			OnloadUiRulesManager.ajaxRulesAreDone();
			Error(xhr);
			//alert('عدم ارتباط با سرور.');
		}
	});
}

function getLCs() {
	var LCs = [];
	for (i in lc) {
		if (i == "proptotype") continue;
		LCs.push({ Field: i, Value: lc[i] });
	}
	return LCs;
}

function getQs(paramList) {
	var qList = [];
	qList.push({ Field: "__parentIds", Value: window.top.logicParentIds });
	if (paramList) {
		var params = paramList.split(',');
		if (params && params.length > 0) {
			for (var i = 0, len = params.length; i < len; i++) {
				var element = $("[name='" + params[i] + "']");
				AddElementToQList(qList, element, params[i]);
			}
		}
	} else {
		var allElem =
			$(
				'select[name], input[name][type="text"], input[name][type="hidden"], input[name][type="number"], textarea');
		if (allElem && allElem.length > 0) {
			for (var i = 0, len = allElem.length; i < len; i++) {
				AddElementToQList(qList, $(allElem[i]), null);
			}
		}
	}
	//console.log("getQs:" );
	//console.log(qList);
	return qList;
}

function AddElementToQList(qList, eleman, name) {
	//if (name) {
	//	console.log("AddElementToQList:" + name + ":" );
	//	console.log(eleman);
	//}
	var obj = {};
	obj.Field = name ? name : eleman.attr('name');
	if (!obj.Field) return;
	var v = null;
	if (eleman.is('select')) {
		var elemanfind = eleman.find("option:selected");
		if (elemanfind) {
			if (eleman.attr('multiple') === 'multiple') {
				
				if (FormUtils.getSelectValue(eleman).length)
					v = "{" + eleman.val() + "}"; // majboor shodam majboor 
				else
					v = String(eleman.val());
			} else
				v = FormUtils.getSelectValue(eleman);
		}
	} else {
		v = eleman.val();
	}
	obj.Value = v;
	//if (name) {
	//	console.log(obj);
	//}
	if (obj.Field && obj.Value)
		qList.push(obj);
}

function getFieldValue(pId, elem, scope, bReport) {
	var table;
	var rowIndex;
	var cellIndex;
	if (scope || bReport) {
		rowIndex = $(elem).closest("tr").index();
		if (bReport) {
			table = $(elem).closest("table");
			var targetElem = $("*[name='" + pId + "']");
			var targetVal = targetElem ? $(targetElem).val() : "";
			if (targetVal) return targetVal;
			if (pId == 'Ids') {
				return $(elem).closest("tr").attr("ids");
			}
			$(table).find("th").each(function(idx, el) {
				var colname = $(el).data("colname");
				if (colname == pId) {
					cellIndex = $(el).index();
					return;
				}
			});
			targetElem = $($(elem).closest("tr ").find("td")[cellIndex]).find("input,select");
			if (targetElem) {
				targetVal = $(targetElem).val();
			} else {
				targetVal = $($(elem).closest("tr ").find("td")[cellIndex]).find("span").html();
			}
			return targetVal;
			//return $(elem).closest("tr").attr("ids");
		}
		if (scope) {
			table = $('table[name="' + scope + '"]');
			var p = $($(table).find("tr")[rowIndex]).find('input[name="' +
				scope +
				'[' +
				(rowIndex - 1) +
				'].' +
				pId +
				'"],select[name="' +
				scope +
				'[' +
				(rowIndex - 1) +
				'].' +
				pId +
				'"]').val();
			return p;
		}
	} else {
		return $("*[name='" + pId + "']").val();
	}
}

function fetchLC(lcId) {
	return lc["lc_" + lcId];
}

function getLCValue(lcId) {
	return lc[lcId];
}

function setLCValue(lcId, val) {
	return lc[lcId] = val;
}

function IsTrue(elem) {
	if (elem)
		return true;
	return false;
}

function IsFalse(elem) {
	if (elem)
		return false;
	return true;
}

function preProcessFormula(formula, elem, bReport) {
	if (!formula) return "";
	if (formula.lastIndexOf("'", 0) === 0)
		return eval(formula);
	var patt = /q\[\w*\]|qm\[\w*\]|qpkv\[\w*\]|qmpkv\[\w*\]/ig;
	var result = formula.match(patt);
	if (result instanceof Array) {
		for (var i in result) {
			if (result[i].replace) {
				var pId = result[i].replace("q[", "").replace("qm[", "").replace("]", "");
				var s = getFieldValue(pId, elem, null, bReport);
				if (!s) {
					s = "0";
				}
				if (isNaN(s)) {
					s = "'" + s + "'";
				}
				formula = formula.replace(result[i], s ? s : "0");
			}
		}
	} else if (result) {
		var pId = result.replace("q[", "").replace("qm[", "").replace("]", "");
		var s = getFieldValue(pId);
		if (!s) {
			s = "0";
		}
		formula = formula.replace(result, s);
	}
	patt = /lc\[[0-9]*\]/ig;
	result = formula.match(patt);
	if (result instanceof Array) {
		for (var i in result) {
			var lcId = result[i].replace("lc[", "").replace("]", "");
			var s = getLCValue(lcId);
			if (!s)
				s = "0";
			formula = formula.replace(result[i], s)
		}
	} else if (result) {
		var lcId = result.replace("lc[", "").replace("]", "");
		var s = this.getLCValue(lcId);
		if (!s)
			s = "0";
		formula = formula.replace(result, s);
	}
	{
		var re = /`/ig;
		formula = String(formula).replace(re, "'");
		if (1 == 1) //is better find the condition for check this block
		{
			re = /;/ig;
			formula = String(formula).replace(re, ",");
		}
		formula = String(formula).replace(/=/g, "==");
		formula = String(formula).replace(/>==/g, ">=");
		formula = String(formula).replace(/<==/g, "<=");
		formula = String(formula).replace(/!==/g, "!=");

		formula = String(formula).replace(/and/ig, " && ");
		formula = String(formula).replace(/or/ig, " || ");
		formula = String(formula).replace(/Number/ig, "Number");
		formula = String(formula).replace(/equal/ig, "FormFunctions.equal");
		//formula = String(formula).replace(/SUM/ig, "SUM");
		//formula = String(formula).replace(/SUBTOTAL/ig, "SUBTOTAL");
		//formula = String(formula).replace(/COUNT/ig, "COUNT");
		//formula = String(formula).replace(/EQUAL/ig, "EQUAL");
		//formula = String(formula).replace(/\[\]/ig, "\"\"");
		//if (formula.indexOf("p[") == -1) {
		//    formula = String(formula).replace(/\[/ig, "\"");
		//    formula = String(formula).replace(/\]/ig, "\"");
		//}
	}
	//while (true) {
	//    var idx = formula.indexOf("Clientfuncs_");
	//    if (idx < 0) break;
	//    if (formula.substr(idx, 7) == "GetIndex") {
	//        formula = formula.replace("Clientfuncs_GetIndex()", "cf_GetIndex(scopeId,tIndex,form)");
	//    }
	//}
	return eval(formula);
}

function getControlContainer(fieldName) {
	return getSpecifierContainer("name=\"" + fieldName + "\"");
}

function getSpecifierContainer(specifier) {
	var $container = $("[" + specifier + "]").closest(".neo-control");
	if ($container.length)
		return $container;
	return $("[" + specifier + "]").closest("div").parent();
}

function runSingleOperation(op, specifier) {
	var getHtmlProperty = function(attr) {
		switch (attr) {
		case 'ReadOnly':
			return 'disabled';
		default:
			throw new Error('Attribute not supported');
		}
	};
	switch (op.Type) {
	case 11: //"SetValue":
		if (op.Value === null || op.Value === '')
			window.FormUtils.clearFields($("[" + specifier + "]"));
		else {
			var $control = $("*[" + specifier + "]");
			if ($control.is('[data-isremote]')) {
				window.Select2Beneficiary.setRemoteValues(op.Value, $control);
			} else {
				$control.val(op.Value).trigger('change');
			}
		}
		break;
	case 12: //"ToggleClass":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).toggleClass(op.Attr);
			break;
		case 2: //Input
			$("*[" + specifier + "]").toggleClass(op.Attr);
			break;
		case 3: //Label
			$("*[" + specifier + "]").parent().find("label").toggleClass(op.Attr);
			break;
		case 4: //column
			$("button[refer='" + op.TargetId + "']").parent().toggleClass(op.Attr);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']")
				.closest('table')
				.find('tbody')
				.find("tr")
				.each(function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).toggleClass(op.Attr);
					}
				});
			//...
			break;
		case 5: //cell
			break;
		}
		break;
	case 13: //"SetClass":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).addClass(op.Attr);
			break;
		case 2: //Input
			$("*[" + specifier + "]").addClass(op.Attr);
			break;
		case 3: //Label
			$($("*[" + specifier + "]").parent()).find("span").addClass(op.Attr);
			break;
		case 4: //column
			//just column Header
			$("button[refer='" + op.TargetId + "']").parent().addClass(op.Attr);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']").closest('table').find('tbody').find("tr").each(
				function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).addClass(op.Attr);
					}
				})
			//...
			break;
		case 5: //cell

			break;
		}
		break;
	case 14: //"RemoveClass":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).removeClass(op.Attr);
			break;
		case 2: //Input
			$("*[" + specifier + "]").removeClass(op.Attr);
			break;
		case 3: //Label
			$($("*[" + specifier + "]").parent()).find("span").removeClass(op.Attr);
			break;
		case 4: //column
			$("button[refer='" + op.TargetId + "']").parent().removeClass(op.Attr);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']").closest('table').find('tbody').find("tr").each(
				function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).removeClass(op.Attr);
					}
				})
			//...
			break;
		case 5: //cell
			break;
		}
		break;
	case 15: //"ChangeCSSAttribute":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).css(op.Attr, op.Value);
			break;
		case 2: //Input
			$("*[" + specifier + "]").css(op.Attr, op.Value);
			break;
		case 3: //Label
			$($("*[" + specifier + "]").parent()).find("span").css(op.Attr, op.Value);
			break;
		case 4: //column
			$("button[refer='" + op.TargetId + "']").parent().css(op.Attr, op.Value);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']").closest('table').find('tbody').find("tr").each(
				function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).css(op.Attr, op.Value);
					}
				})
			//...
			break;
		case 5: //cell
			break;
		}

		break;
	case 17: //"SetProperty":
		var attrVal = (op.Value.toLowerCase() === 'false') && (op.Attr === 'ReadOnly') ? false : op.Value;
		$("[" + specifier + "]").prop(getHtmlProperty(op.Attr), attrVal);
		$("[associated-hidden-name='" + op.TargetId + "']").prop(getHtmlProperty(op.Attr), attrVal);
		break;
	case 21: //"ShowWarning":
		alert(op.Value);
		break;
	case 22: //"ShowError":
		alert(op.Value);
		return;
	case 23: //"ShowPrompt":
		if (!prompt(op.Value)) {
			return;
		}
		break;
	case 24: //"ShowModal":
		break;
	case 25: //"ShowWindow":
		break;
	case 26: //"ShowAutoHideWindow":
		break;
	case 31: //"AddRow":
		break;
	case 32: //"ChangeList":
		var $select = $("select[" + specifier + "]");
		$select.attr("filter-formula", op.Formula);
		var value = $select.val() || $select.attr("initvalue");
		if ($select.is('[data-isremote]')) {
			window.Select2Beneficiary.setRemoteValues(value, $select);
		} else {
			$select.html("");
			for (var i = 0, len = op.Rows.length; i < len; i++) {
				var ids = op.Rows[i].Ids;
				$select.append('<option value=' + ids + '>' + op.Rows[i].DisplayValue + '</option>');
			}
			if (!value || !value.length) return;
			if (typeof value === 'string')
				value = value.split(',');
			$select.val(value);
			$select.trigger('change');
		}
		break;
	}
}

function runOperation(op, scope) {
	var specifier = "name='" + op.TargetId + "'";
	if (!scope)
		return runSingleOperation(op, specifier);

	$.each($('[full-column-name="' + scope + '_' + op.TargetId + '"]'),
		function(idx, item) {
			specifier = "name='" + $(item).prop('name') + "'";
			runSingleOperation(op, specifier);
		}
	);
}

function runOperations(operations, scope) {
	for (var i in operations.Items) {
		var op = operations.Items[i];
		runOperation(op, scope);
	}
}

var eTargetArea =
{
	Container: 1,
	Input: 2,
	Label: 3,
	Column: 4,
	Cell: 5,
	HeaderCell: 6,
};

function Client_AddClass(This, scope, fieldName, className, targetArea, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		var hasRequire = $(targetField).attr("required") || $(targetField).attr("hasrequired");
		if (formula) {
			if (className == "ShowHide") {
				if (hasRequire) {
					$(targetField).attr("hasrequired", true);
					$(targetField).attr("disabled", "disabled");
				}
			}
			$(targetField).addClass(className);
		} else {
			if (className == "ShowHide") {
				if (hasRequire) {
					$(targetField).attr("required", "required");
					$(targetField).removeAttr("disabled");
				}
			}
			$(targetField).removeClass(className);
		}
	} else if (className == "ShowHide" && (targetArea == "Container" || targetArea == "Input")) {
		var $inputField = $("*[name=" + fieldName + "]");
		if ($inputField.prop('type') === 'hidden') {
			$inputField = $("[associated-hidden-name=" + fieldName + "]");
		}
		var hasRequire = $inputField.attr("required") || $inputField.attr("hasrequired");
		if (formula) {
			if (hasRequire) {
				$inputField.attr("disabled", "disabled");
				$inputField.attr("hasrequired", true);
			}
		} else {
			if (hasRequire) {
				$inputField.attr("required", "required");
				$inputField.removeAttr("disabled");
			}
		}
	}
	switch (targetArea) {
	case "Container":
		var $container;
		if (formula) {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$container = $("*[name=" + fieldName + "]").closest("div").parent();
				$container.addClass(className);
			} else {
				if ($("[name=" + fieldName + "]").length)
					$container = getControlContainer(fieldName);
				else $container = $('[id="' + fieldName + '"]');
				$container.addClass(className);
			}
			// Also hide parent table cells/rows to free up space
			if (className == "ShowHide" && $container.length) {
				$container.closest("td, th").addClass(className);
				// If all cells in a row are hidden, hide the row too
				var $row = $container.closest("tr");
				if ($row.length) {
					var visibleCells = $row.find("td, th").not(".ShowHide").length;
					if (visibleCells === 0) {
						$row.addClass(className);
					}
				}
			}
		} else {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$container = $("*[name=" + fieldName + "]").closest("div").parent();
				$container.removeClass(className);
			} else {
				if ($("[name=" + fieldName + "]").length)
					$container = getControlContainer(fieldName);
				else $container = $('[id="' + fieldName + '"]');
				$container.removeClass(className);
			}
			// Also show parent table cells/rows
			if (className == "ShowHide" && $container.length) {
				var $cell = $container.closest("td, th");
				$cell.removeClass(className);
				// Show the row if at least one cell is now visible
				var $row = $container.closest("tr");
				if ($row.length) {
					var visibleCells = $row.find("td, th").not(".ShowHide").length;
					if (visibleCells > 0) {
						$row.removeClass(className);
					}
				}
			}
		}
		break;
	case "Input":
		if (formula) {
			$("*[name=" + fieldName + "]").addClass(className);
		} else {
			$("*[name=" + fieldName + "]").removeClass(className);
		}
		break;
	case "Label":
		if (formula) {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').addClass(className);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').addClass(className);
			}
		} else {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').removeClass(className);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').removeClass(className);
			}

		}
		break
	case eTargetArea.Column:
		break
	case eTargetArea.Cell:
		break
	case eTargetArea.HeaderCell:
		break
	}
}

function Client_RemoveClass(This, scope, fieldName, className, targetArea, formula) {
	Client_AddClass(This, scope, fieldName, className, targetArea, !formula);
}

function Client_ChangeCSSAttr(This, scope, fieldName, attribute, targetArea, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		$(targetField).css(attribute, formula);
	} else
		switch (targetArea) {
		case "Container":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().css(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").parent().css(attribute, formula);
			}

		case "Input":
			$("*[name=" + fieldName + "]").css(attribute, formula);
			break;
		case "Label":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').css(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').css(attribute, formula);
			}

			break;
		case eTargetArea.Column:
			break;
		case eTargetArea.Cell:
			break;
		case eTargetArea.HeaderCell:
			break;
		}
}

function Client_ChangeAttr(This, scope, fieldName, attribute, targetArea, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		$(targetField).attr(attribute, formula);
	} else
		switch (targetArea) {
		case "Container":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().attr(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").attr(attribute, formula);
			}

		case "Input":
			$("*[name=" + fieldName + "]").attr(attribute, formula);
			break;
		case "Label":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').attr(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').attr(attribute, formula);
			}

			break;
		case eTargetArea.Column:
			break;
		case eTargetArea.Cell:
			break;
		case eTargetArea.HeaderCell:
			break;
		}
}

function Client_GetVal(This, scope, fieldName) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		return $(targetField).val();
	} else
		return $("*[name=" + fieldName + "]").val();
}

function Client_SetVal(This, scope, fieldName, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		return $(targetField).val(formula).trigger('change');
	} else
		$("*[name=" + fieldName + "]").val(formula).trigger('change');
}

function Client_SetLC(This, scope, lcName, formula) {
	lc[lcName] = formula;
}

function IsNullOrEmpty(obj) {
	if (obj)
		return false;
	return true;
}

function sysLink(sysLinkCt, bReport, event) {
	var urlAddress = "";
	urlAddress += $(sysLinkCt).attr("systemLinkAddress");
	var paramsObj = $(sysLinkCt).attr("linkParameter").split(',');
	var params = [];
	for (var i in paramsObj) {
		var param = paramsObj[i].toString();
		if (!param) continue;
		var paramPair = param.split('=');
		if (paramPair.length == 2) {
			var nF = preProcessFormula(paramPair[1].toString(), sysLinkCt, bReport);
			var nP = paramPair[0] + "=" + nF;
			params.push(nP);
		}
	}
	urlAddress += "&" + params.join("&");
	if (event && event.ctrlKey)
		window.open(urlAddress, '_blank');
	else
		location.href = urlAddress;
}

function InvalidMsg(elem, event) {
	if (elem) {
		elem.setCustomValidity('');
		if (elem.validity.patternMismatch || elem.validity.rangeOverflow || elem.validity.rangeUnderflow) {
			elem.setCustomValidity(window.tetaI18n.t('InvalidInputValue'));
		} else if (elem.value === '' || elem.value === null){
			elem.setCustomValidity(window.tetaI18n.t('RequiredError'));
		}
	} else if (event) {
		event.target.setCustomValidity('');
		if (!event.target.validity.valid) {
			if (event.target.validity.patternMismatch || event.target.validity.rangeOverflow || event.target.validity.rangeUnderflow) {
				event.target.setCustomValidity(window.tetaI18n.t('InvalidInputValue'));
			} else {
				event.target.setCustomValidity(window.tetaI18n.t('RequiredError'));
			}
		}
	}
	return true;
};

function checkboxlistChanged(chk) {
	var chkName = $(chk).attr("name").split('_')[0];
	var mChkVal = $("input[name=" + chkName + "]").val();
	if (chk.checked) {
		mChkVal = mChkVal | $(chk).val();
	} else
		mChkVal = mChkVal & ~$(chk).val();

	$("input[name=" + chkName + "]").val(mChkVal);
}

function applySeperator(input, event) {
	try {
		var val = $(input).val(), out;
		val = val.replace(/,/g, "");
		var t = new RegExp(/^\d+$/);
		var s = t.test(val.toString());
		var len = val.length;
		val = val.split("");
		var reload = false;
		for (var j = 0, l = val.length; j < l; j++) {
			var v = val[j];
			var s = t.test(v);
			if (!s) {
				val.splice(j, 1);
				reload = true;
			}
		}
		if (reload) {
			$(input).val(val.join(""));
			applySeperator(input, event);
		}
		if (len <= 3) {
			return;
		}
		var NOS = parseInt(len / 3);
		if (NOS) {
			for (var i = 0; i < NOS; i++) {
				var lastIndex = val.indexOf(',');
				if (lastIndex === -1)
					val.splice((val.length - 3), 0, ',');
				else {
					val.splice((lastIndex - 3), 0, ',');
				}
				//val = out();
			}
			if (val[0] === ",") {
				val = val.slice(1, val.length);
			}
			out = val.join("");
			$(input).val(out);
		}
	} catch (e) {
		alert(e);
	}
}

function addComboIdsAndRedirect(initialUrl, comboId) {
	if (!initialUrl || !comboId) {
		console.warn("Wrong arguments: initialUrl=" + initialUrl + " comboId=" + comboId);
		return;
	}
	var $selectBox = $('select[name="' + comboId + '"');
	var val = $selectBox.val();
	if (!val) {
		//alert('لطفا ابتدا یکی از مقادیر لیست را انتخاب نمایید.');
		window.toast.info(window.tetaI18n.t('FirstSelectAValue'));
		return;
	}
	var ids = Array.isArray(val) ? val[0] : val;

	//    console.log(initialUrl);
	//    console.log(comboId);
	//    console.log($('select[name="' + comboId +'"').val());
	window.PageAddressManager.navigateTo(initialUrl + '&ids=' + ids, true);
}

function initializePluginsDefaults() {
	if ($.fn.select2) {
		$.fn.select2.defaults.set("theme", "bootstrap4");
		$.fn.select2.defaults.set("containerCssClass", ":all:");
		$.fn.select2.defaults.set("language", window.tetaConfigs.neutralCulture);
	}
}

var Select2Beneficiary = function() {
	var templateResult = function(data, container) {
		if (data.cssClass) {
			var contextualClass = data.cssClass.toLowerCase();
			$(container).addClass('bg-' + contextualClass);
			if (['danger', 'success'].indexOf(contextualClass) !== -1) {
				$(container).addClass('text-white');
			}
		}
		return data.text;
	};

	var templateSelection = function(data, container) {
		if (data.cssClass) {
			$(container).addClass('bg-' + data.cssClass.toLowerCase());
		}
		return data.text;
	};

	var hasOptionalValue = function($select) {
		return $select.find('option').filter(function() {
			var optionValue = $(this).val();
			return optionValue === '' || optionValue === null || optionValue === undefined || optionValue === '0';
		}).length > 0;
	};

	var ensureOptionalOption = function($select) {
		if ($select.prop('multiple')) return;
		if ($select.prop('required')) return;
		if (hasOptionalValue($select)) return;

		var placeholderText = $select.data('placeholder') || $select.attr('placeholder') || '';
		var optionalOption = new Option(placeholderText, '', false, false);
		$(optionalOption).attr('data-empty-option', 'true');
		$select.prepend(optionalOption);
	};

	var getAjaxObject = function(namespaceId, entityId, fieldId, formId, isMandatory, remoteUrl, filterFormula) {
		var ret = {
			url: remoteUrl || (window.top.rootUrl + 'form/GetComboData'),
			//			dataType: 'json',
			type: 'POST',
			//			contentType: "application/json; charset=utf-8",
			//			params: { // extra parameters that will be passed to ajax
			//				contentType: "application/json; charset=utf-8"
			//			},
			headers: window.AddAntiForgeryToken(),
			delay: 400,
			data: function(params) {
				var filterObj =
				{
					Expression: params.term, // search term
					Filter: $('[name="' + fieldId + '"]').attr('filter-formula') || filterFormula,
					NamespaceId: namespaceId,
					EntityId: entityId,
					FieldId: fieldId,
					PageType: window.PageAddressManager.getPageType(),
					FormId: formId,
					Count: 30,
					IsMandatory: isMandatory,
					Page: params.page || 1,
					Qs: [],
					LCs: []
				};
				if (filterObj.Filter) {
					filterObj.Qs = getQs();
					filterObj.LCs = getLCs();
				}
				return filterObj;
				//				return JSON.stringify(filterObj);
			},
			processResults: function(data, params) {
				params.page = params.page || 1;
				return {
					results: $.map(data.Rows,
						function(item) {
							return {
								text: item.DisplayValue,
								id: item.Ids,
								cssClass: item.Style
							}
						}),
					pagination: {
						more: data.HasMore //(params.page * 30) < data.total_count
					}
				};
			},
			minimumInputLength: 3,
			cache: true
		};
		return ret;
	};

	var selectRows = function(rows, $remoteSelect) {
		if (!rows) return;
		$.each(rows,
			function(idx, row) {
				var $rowOption = $remoteSelect.find("option[value='" + row.id + "']");
				if (!$rowOption.length) {
					//					var newOption = new Option(row.text, row.id, true, true);
					$remoteSelect.append($('<option>',
						{
							val: row.id,
							text: row.text,
							selected: true
						})) /*.trigger('change')*/;
				} else if (!$rowOption.is('[selected]')) {
					$remoteSelect.val(row.id).trigger('change');
				}
			});
	};

	var fetchInitValues = function(initialValues, namespaceId, entityId, fieldId, formId, filter) {
		var filterObj = {
			InitValues: initialValues,
			NamespaceId: namespaceId,
			EntityId: entityId,
			FieldId: fieldId,
			Filter: filter,
			PageType: window.PageAddressManager.getPageType(),
			FormId: formId,
			IsMandatory: false //?
		};
		if (filter) {
			filterObj.Qs = getQs();
		}
		return $.ajax({
			type: 'POST',
			headers: window.AddAntiForgeryToken(),
			url: window.top.rootUrl + "form/GetComboInitValues",
			data: filterObj
		}).then(function(response) {
			return response.Rows.map(function(row) {
				return {
					id: row.Ids,
					text: row.DisplayValue,
					cssClass: row.Style
				};
			});
		});
	};

	var instantiateRemoteDataSelect = function() {
		var thisRemoteSelect = this;
		var $thisRemoteSelect = $(thisRemoteSelect);
		var namespaceId = $thisRemoteSelect.data('namespace') || window.PageAddressManager.getNamespaceId();
		var entityId = $thisRemoteSelect.data('entity') || window.PageAddressManager.getEntityId();
		var fieldId = $thisRemoteSelect.data('column') ||
			$thisRemoteSelect.parents('.neo-control').data('id');

		var formId = $thisRemoteSelect.data('form') || window.PageAddressManager.getPageId();

		ensureOptionalOption($thisRemoteSelect);

		var allowClear = !$thisRemoteSelect.prop('multiple') && !$thisRemoteSelect.prop('required');
		var placeholderText = $thisRemoteSelect.data('placeholder') || $thisRemoteSelect.attr('placeholder');
		if (placeholderText === undefined && allowClear) {
			placeholderText = ' ';
		}

		var select2Options = {
			ajax: getAjaxObject(
				namespaceId,
				entityId,
				fieldId,
				formId,
				$thisRemoteSelect.attr("required") ? true : false,
				$thisRemoteSelect.data("remote-url"),
				$thisRemoteSelect.attr('filter-formula')),
			templateResult: templateResult,
			templateSelection: templateSelection
		};

		if (allowClear) {
			select2Options.allowClear = true;
		}

		if (placeholderText !== undefined) {
			select2Options.placeholder = placeholderText;
		}

		$thisRemoteSelect.select2(select2Options);

		var initialValuesOfSelect = $thisRemoteSelect.attr('initvalue');
		if (Boolean(initialValuesOfSelect)) {
			fetchInitValues(initialValuesOfSelect,
					namespaceId,
					entityId,
					fieldId,
					formId,
					$thisRemoteSelect.attr("filter-formula"))
				.then(function(rows) {
						selectRows(rows, $thisRemoteSelect);
						$thisRemoteSelect.attr('init-fetched', true);
					},
					function(err) {
						console.error(err);
					});
		}
	};

	var instantiateControls = function() {
		if (!$('select').select2) return;
		$('select[data-isremote]')
			.each(instantiateRemoteDataSelect);

		$('select:not([data-isremote])')
			.select2();
	};

	//	var getMissingOptions = function(idsArray, $remoteSelect) {
	//		 return idsArray.filter(function(row) {
	//			 return $remoteSelect.find('option[value="' + row.id + '"][selected="selected"]').length === 0;
	//		});
	//	};

	var setRemoteValues = function(values, $remoteSelect) {
		if (!values)
			return;
		if (Array.isArray(values))
			values = values.toString();
		var namespaceId = $remoteSelect.data('namespace') || window.PageAddressManager.getNamespaceId();
		var entityId = $remoteSelect.data('entity') || window.PageAddressManager.getEntityId();
		var formId = $remoteSelect.data('form') || window.PageAddressManager.getPageId();
		fetchInitValues(values,
				namespaceId,
				entityId,
				$remoteSelect.parents('.neo-control').data('id'),
				formId,
				$remoteSelect.attr("filter-formula"))
			.then(function(rows) {
					//					window.FormUtils.clearField($remoteSelect);
					$remoteSelect.empty();
					ensureOptionalOption($remoteSelect);
					selectRows(rows, $remoteSelect);
				},
				function(err) {
					console.error(err);
				});
	};

	return {
		instantiateControls: instantiateControls,
		getAjaxObject: getAjaxObject,
		fetchInitValues: fetchInitValues,
		templateResult: templateResult,
		templateSelection: templateSelection,

		setRemoteValues: setRemoteValues
	};
}();

var PwtDatepickerBeneficiary = function() {
	var instantiatePwtDatepicker = function($item) {
		var dontConvert = $item.attr('dont-convert');
		var calendarType = $item.hasClass('miladi') ? 'gregorian' : 'persian';
		var pOptions = {
			calendarType: calendarType,
			calendar: {
				persian: {
					showHint: true,
					locale: dontConvert ? 'en' : 'fa'
				},
				gregorian: {
					showHint: true
				}
			},
			format: dontConvert ? 'YYYY/MM/DD' : 'dddd DD MMMM YYYY',
			toolbox: {
				enabled: true,
				calendarSwitch: { enabled: !dontConvert }
			},
			observer: true,
			autoClose: true,
			initialValue: !!$item.attr('value'),
			initialValueType: dontConvert ? calendarType : 'gregorian'
		};
		if (!dontConvert) {
			pOptions.altField = '[name="' + $item.attr('associated-hidden-name') + '"]';
			pOptions.altFieldFormatter =
				function(unixDate) {
					var date = new Date(unixDate);
					return date.getFullYear() +
						'/' +
						("0" + (date.getMonth() + 1)).slice(-2) +
						'/' +
						("0" + date.getDate()).slice(-2);
				}
		}
		$item.pDatepicker(pOptions);
		if (dontConvert) return;
		$item.on('change',
			function() {
				var $this = $(this);
				if (!$this.val())
					$('[id="' + $this.attr('associated-hidden-name') + '"]').val('');
			});
	};

	var instantiateControls = function() {
		if ($.fn.pDatepicker) {
			$('input.dateField').each(function(idx, item) {
				instantiatePwtDatepicker($(item));
			});
		}
	};

	return {
		instantiateControls: instantiateControls,
		instantiatePwtDatepicker: instantiatePwtDatepicker
	};
}();

var AddClearBeneficiary = function() {

	var onClear = function($input) {
		setTimeout(function() {
				$input.trigger('change');
			},
			0);
	};

	var basicOptions = {
		closeSymbol: "✖",
		symbolClass: '',
		onClear: onClear
	};

	var instantiate = function() {
		if ($.fn.addClear === undefined) return;
		$('input[name][type="text"], input[name][type="number"], textarea').addClear(basicOptions);
		// با کلیدِ اسکیپ مقدارِ دیتپیکر خالی نمیشد.
		$('.dateField').addClear(Object.assign({ clearOnEscape: false }, basicOptions));

		//        $('input[name][type="text"]:not(.dateField), input[name][type="number"], textarea').addClear(basicOptions);
		//
		//        // با کلیدِ اسکیپ مقدارِ دیتپیکر خالی نمیشد.
		//        // not([name]) برایِ خراب نشدنِ دیتپیکرِ اسکجولِ گزارش هست!!
		//        $('.dateField:not([name])').addClear(Object.assign({ clearOnEscape: false }, basicOptions));
	};

	return {
		instantiate: instantiate
	};
}();

function instantiatePlugins() {
	Select2Beneficiary.instantiateControls();
	PwtDatepickerBeneficiary.instantiateControls();
	AddClearBeneficiary.instantiate();
}

// Select2 Change Handler - برای اجرای ShowHide و سایر UI Rules
// این handler لازم است چون Select2 از jQuery events استفاده می‌کند و onchange attribute DOM با آن کار نمی‌کند
var Select2ChangeHandler = function() {
	var initializeChangeHandlers = function() {
		// برای همه select ها که onchange attribute دارند
		$(document).on('change', 'select[onchange]', function(e) {
			// اگر از طریق Select2 trigger شده، اجازه بده handler اجرا شود
			var $select = $(this);
			var onchangeAttr = $select.attr('onchange');
			
			
			// اگر onchange شامل inputChanged است، آن را فراخوانی کن
			if (onchangeAttr && onchangeAttr.indexOf('inputChanged') !== -1) {
				try {
					// استخراج پارامترها از onchange attribute
					var match = onchangeAttr.match(/inputChanged\(this,\s*'([^']*)',\s*'([^']*)'\)/);
					if (match) {
						var source = match[1];
						var scope = match[2];
						
						// فراخوانی تابع inputChanged
						if (typeof inputChanged === 'function') {
							inputChanged(this, source, scope);
						}
					}
				} catch (ex) {
					console.error('[Select2ChangeHandler] Error executing onchange:', ex);
				}
			}
		});
	};
	
	return {
		initialize: initializeChangeHandlers
	};
}();

$(function() {
	initializePluginsDefaults();
	instantiatePlugins();
	Select2ChangeHandler.initialize();
	$('[nodatamandatory]').prop('selectedIndex', -1);
	$('[nodatamandatory]').trigger('change');
	$('form').trigger('reinitialize.areYouSure');
});