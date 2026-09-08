var FormStateManager = function($, pageAddressManager) {
	var getStateKey = function(stateType) {
		return pageAddressManager.getPageCode() + stateType;
	};

	var loadLocalState = function(stateType) {
		var prevStateJson = localStorage.getItem(getStateKey(stateType));
		return prevStateJson === null ? {} : JSON.parse(prevStateJson);
	};

	var saveLocalState = function(stateType, stateId, value) {
		var state = loadLocalState(stateType);
		state[stateId] = value;
		localStorage.setItem(getStateKey(stateType), JSON.stringify(state));
	};

	var manageBootstrapTabsState = function() {
		$('a[data-toggle="tab"]').click(function(e) {
			e.preventDefault();
			$(this).tab('show');
		});

		$('a[data-toggle="tab"]').on("shown.bs.tab",
			function(e) {
				var selectedTab = $(e.target).attr("href");
				saveLocalState('selectedTab',
					$(e.target).closest('.neo-control').data('id'),
					selectedTab);
			});

		var savedSelectedTabs = loadLocalState('selectedTab');
		for (var tab in savedSelectedTabs) {
			if (savedSelectedTabs.hasOwnProperty(tab)) {
				var tabSelector =
					(tab === 'null' ? '' : '[data-id="' + tab + '"] ') +
						'a[data-toggle="tab"][href="' +
						savedSelectedTabs[tab] +
						'"]';
				$(tabSelector)
					.tab('show');
			}
		}
	};

	manageBootstrapTabsState();
}(window.jQuery, window.PageAddressManager);