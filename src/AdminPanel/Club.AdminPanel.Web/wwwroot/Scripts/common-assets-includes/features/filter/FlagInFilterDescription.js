$(function () {
	var filteredByText = $('span.filtered-by').text();
	var startOfIso2String = filteredByText.indexOf("کد ایزو");
	if (startOfIso2String !== -1) {
		var startOfIso2Code = filteredByText.indexOf("^"); //todo very bad practice related to filterNames convention
		var endOfIso2Code = filteredByText.indexOf("~");

		var separator = filteredByText.substr(startOfIso2String, endOfIso2Code);

		var iso2 = filteredByText.slice(startOfIso2Code + 1, endOfIso2Code - 1);
		var arr = filteredByText.split(separator);

		var otherText = arr[0] + arr[1];

		$('span.filtered-by').html(otherText +
			window.tetaI18n.t('Country') +
			": " +
			countryNamesBasedOnIso2[iso2] +
			' <img style="width:30px;" src="' +
			window.top.rootUrl +
			'Content/common-assets-includes/flags/' +
			countryNamesBasedOnIso2[iso2] +
			'.svg"' +
			'/>');
	}
});
