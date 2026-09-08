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