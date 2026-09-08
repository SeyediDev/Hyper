function tooltipFormatter(tooltipsObj) {
	var tooltipHtml =
		"<div style='direction: rtl; z-index:100; background-color:white; margin:-8px; padding:5px;'>";
	for (var field in tooltipsObj) {
		if (tooltipsObj.hasOwnProperty(field)) {
			tooltipHtml += tooltipFormatterItem(field, tooltipsObj[field]);
		}
	}
	tooltipHtml += "</div>";
	return tooltipHtml;
}

function tooltipFormatterItem(field, fieldValue) {
	return '<span class="float-right teta-bidi" style="direction: rtl;">' +
		field +
		': ' +
		fieldValue +
		'</span><br/>';
}

function grabReportData(key) {
	if (!window.top.reportsData)
		window.top.reportsData = {};
	window.top.reportsData[key] = [];
	return window.top.reportsData[key];
}

// Generic dispatcher for chart types. If a specialized renderer exists,
// it will be invoked here. Adds Calendar handling.
function renderChartByType(selector, chartType, data, options){
	options = options || {};
	try{
		if(!chartType) { $(selector).html(''); return; }
		var type = (typeof chartType === 'string') ? chartType : chartType.toString();
		if(type.toLowerCase().indexOf('calendar') !== -1 || type === 'Calendar' || chartType === 24){
			if(window.renderCalendarChart){
				window.renderCalendarChart(selector, data, options);
				return;
			}
		}
		// fallback: if no specific renderer, try to use Highcharts generic
		if(window.renderHighchartGeneric){
			window.renderHighchartGeneric(selector, data, options);
			return;
		}
		// nothing matched
		$(selector).html('');
	}catch(e){
		console.error('renderChartByType error', e);
	}
}

window.renderChartByType = renderChartByType;