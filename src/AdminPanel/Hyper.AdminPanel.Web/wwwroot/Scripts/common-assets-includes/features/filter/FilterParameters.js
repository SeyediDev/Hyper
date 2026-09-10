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