var PaginationManager = function() { // optional dependency on selection manager
	var jumpToPage = function() {
		var jtpVal = $('#jtpInput').val();
		$('input[name="Page"]').val(jtpVal);
		$("input[name='newPage']").val("0");
		$('#filter-form').submit();
	};

	var pageChanged = function(varName) {
		var pageNumber = $(varName).find("span").attr("page")
			? (+$(varName).find("span").attr("page"))
			: +$(varName).find("span").text();
		$("input[name='newPage']").val("0");
		$("input[name='Page']").val(pageNumber);
		$("input[name='Page']").closest("form").submit();
	};

	return {
		jumpToPage: jumpToPage,
		pageChanged: pageChanged
	};
}();

$('#showFilter')
	.on('click',
		function(e) {
			$('#filterDiv').slideToggle();
			e.stopPropagation();
		});

function submitFilter() {
	if ($("#filter-form")[0].checkValidity()) {
		var startSubmitFilterButton = $("#submit-filter").ladda();
		startSubmitFilterButton.ladda('start');
	}
	var $pageObj = $("input[name='Page']");
	if ($pageObj.length) {
		$pageObj.val(1);
		$pageObj.closest("form").find(':submit').click();
	}
	$("#filter-form").find(':submit').click();
}

function toggleDesignMode() {
	$(".designTool").toggleClass("designMode");
}

$(function() {
	$('.dropdown-menu input').click(function(e) { //being able to put input inside twitter bootstrap's dropdown
		e.stopPropagation();
	});
});
(function($, formUtils) {
	window.FilterParametersManager = function() {
		var updateSelectedIcon = function(fieldName, parameter) {
			var suitableClass =
				$('[id="' + fieldName + '-fp-item-' + parameter + '"] i')	
					.attr('class');
			var targetI = $('[id="' + fieldName + '-fp-icon"]');
			targetI.attr('class', suitableClass);
		};

		var updateParametersList = function(fieldName, parameter) {
			$('[id^="' + fieldName + '-fp-item-"]').removeClass('active');
			$('[id="' + fieldName + '-fp-item-' + parameter + '"]').addClass('active');
		};

		var updateInputsIfNeeded = function(fieldName, parameter) {
			 if (parameter === 'IsNull' || parameter === 'IsNotNull') {
				 $('[name="' + fieldName + '"]').attr('disabled', true);
				 formUtils.clearField($('[name="' + fieldName + '"]'));
			 } else {
				 $('[name="' + fieldName + '"]').removeAttr('disabled');
            }
		};
		var updateUI = function(fieldName, parameter) {
			updateSelectedIcon(fieldName, parameter);
			updateParametersList(fieldName, parameter);
			updateInputsIfNeeded(fieldName, parameter);
		};

		var setValue = function(fieldName, parameter) {
			$('input[name="' + fieldName + '__FilterParameter"]').val(parameter);
		};

		var setParameter = function(fieldName, parameter) {
			setValue(fieldName, parameter);
			updateUI(fieldName, parameter);
		};

		return {
			setParameter: setParameter
		};
	}();
})(window.jQuery, window.FormUtils);