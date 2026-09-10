
var diagramUrl = 'https://cdn.staticaly.com/gh/bpmn-io/dmn-js-examples/a71e16/starter/diagram.dmn';

var CLASS_NAMES = {
	drd: 'dmn-icon-lasso-tool',
	decisionTable: 'dmn-icon-decision-table',
	literalExpression: 'dmn-icon-literal-expression'
};

var $container = $('.editor-container');
var $tabs = $('.editor-tabs');

// modeler instance
var dmnModeler = new DmnJS({
	container: $container,
	height: 500,
	width: '100%',
	keyboard: {
		bindTo: window
	}
});

$tabs.delegate('.tab',
	'click',
	function(e) {
		var viewIdx = parseInt(this.getAttribute('data-id'), 10);

		var view = dmnModeler.getViews()[viewIdx];

		dmnModeler.open(view);
	});

dmnModeler.on('views.changed',
	function(event) {

		var { views, activeView } = event;

		// clear tabs
		$tabs.empty();

		views.forEach(function(v, idx) {

			var className = CLASS_NAMES[v.type];

			var tab = $(`
            <div class="tab ${v === activeView ? 'active' : ''}" data-id="${idx}">
		<span class="${className}"></span>
		${v.element.name || v.element.id}
	</div>
	`);

			$tabs.append(tab);
		});
	});

/**
* Save diagram contents and print them to the console.
*/
function exportDiagram() {

	dmnModeler.saveXML({ format: true },
		function(err, xml) {

			if (err) {
				return console.error('could not save DMN 1.1 diagram', err);
			}

			alert('Diagram exported. Check the developer tools!');

			console.log('DIAGRAM', xml);
		});
}

function openDiagram(dmnXML) {

	// import diagram
	dmnModeler.importXML(dmnXML,
		function(err) {

			if (err) {
				return console.error('could not import DMN 1.1 diagram', err);
			}

			var activeEditor = dmnModeler.getActiveViewer();

			// access active editor components
			var canvas = activeEditor.get('canvas');

			// zoom to fit full viewport
			canvas.zoom('fit-viewport');
		});
}


// load external diagram file via AJAX and open it
$.get(diagramUrl, openDiagram, 'text');

// wire save button
$('#save-button').click(exportDiagram);