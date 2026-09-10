var WorkAssignmentView = function () {
	var constRecordsPerPage = 8;
	var list = new List('work-assignment-users',
		{
			valueNames: [
				'Name',
				'Family',
				'Username',
				{ data: ['id'] },
				{ name: 'avatarUrl', attr: 'src' }
			],
			item: '<div class="col-sm-3 assignment-item"> \
					  <div class="t-card mouse-pointer assign-action"> \
					   <img onerror="handleAvatarError(this)" class="user-image avatarUrl"></img> \
					   <div class="full-name text-center"><span class="Name"></span> <span class="Family"></span></div>\
					   <div class="Username text-center text-muted"></div>\
					  </div>\
					 </div>',
			 pagination: true,
			 page: constRecordsPerPage
		});

	$(document).on('click',
		'.assign-action',
		function() {
			 WorkAssignment.assignWorksToUser($(this).closest('.assignment-item').data('id'));
		});

	var getUserAvatarUrl = function(avatarId) {
		if (avatarId)
			return window.top.rootUrl + 'Download/DL/' + avatarId;
		return window.top.rootUrl + 'Content/common-assets-includes/images/global/login.png';
	};

	var getDisplayableUsers = function(usersList) {
		return $.map(usersList,
			function(user) {
				return {
					id: user.Id,
					Name: user.Name,
					Family: user.Family,
					Username: user.Username,
					avatarUrl: getUserAvatarUrl(user.AvatarId)
				};
			});
	 };

	var showOrHidePaging = function(shouldShow) {
		 var $pagination = $('#work-assignment-users .pagination');		 
		 $pagination.removeClass('hidden');
		 if(!shouldShow)
		   $pagination.addClass('hidden');
	};

	var displayUsersList = function(usersList) {
		list.clear();
		 list.add(getDisplayableUsers(usersList));
		 showOrHidePaging(usersList.length > constRecordsPerPage);
		$('#work-assignment-modal').modal('show');
	};
	return {
		displayUsersList: displayUsersList
	};
}();

var WorkAssignment = function() {
	var aiAddressings = [];

	var createAiAddressingOfRow = function($row) {
		var taskAddressing = {};
		taskAddressing.ProcessId = $row.data('processid');
		taskAddressing.TaskId = $row.data('activityid');
		taskAddressing.ActivityInstanceId = $row.data('aiid');
		taskAddressing.ProcessVersion = $row.data('version');
		return taskAddressing;
	};
	var tryConstructAiAddressings = function(buttonElement) {
		aiAddressings = [];
		var $button = $(buttonElement);
		if ($button.is('[multiple-assignment]')) {
			if (SelectionManager.findOutCheckBoxesState() === SelectionManager.eCheckBoxesState.allPagesChecked) {
				window.toast.info('امکان تخصیص کارهای صفحات دیگر وجود ندارد.');
				return false;
			}
			var selectedRows = SelectionManager.getSelectedRows();
			if (!selectedRows.length) {
				window.toast.info('لطفا ابتدا کاری را برای تخصیص انتخاب نمایید.');
				return false;
			}
			$.each(selectedRows,
				function(idx, row) {
					aiAddressings.push(createAiAddressingOfRow($(row)));
				});
		} else {
			aiAddressings.push(createAiAddressingOfRow($button.closest('tr')));
		}
		return true;
	};
	var getUsersList = function(pageNo) {
		return $.ajax({
			type: "POST",
			url: window.top.rootUrl + 'Process/GetActivitiesUsers?pageNo=' + pageNo + '&pageSize=' + 400,
			data: JSON.stringify({ activities: aiAddressings }),
			contentType: 'application/json; charset=utf-8',
			headers: AddAntiForgeryToken()
		});
	};
	var openUsersList = function(buttonElement) {
		var b = tryConstructAiAddressings(buttonElement);
		if (!b) return;
		getUsersList(1).then(WorkAssignmentView.displayUsersList).catch(function(er) {
			window.toast.error('خطا در دریافت فهرست کاربران قابل تخصیص');
			console.error(er);
		});
	};
	var assignmentSucceed = function(successModel) {
		$('#work-assignment-modal').modal('hide');
		for (var i = 0; i < aiAddressings.length; i++) {
			var row = $('#ai-row-' + aiAddressings[i].ActivityInstanceId);
			row.find('.actual-owner-name').text(successModel.Name + ' ' + successModel.Family);
			row.find('.resource-state-text').text('تخصیص یافته');
		}
	};
	var assignWorksToUser = function(userId) {
		var assignObj = { "newUserId": userId };
		$.extend(assignObj, { activities: aiAddressings });
		console.log(assignObj);
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + 'Process/ChangeUser',
			data: JSON.stringify(assignObj),
			contentType: 'application/json; charset=utf-8',
			headers: AddAntiForgeryToken(),
			success: assignmentSucceed,
			error: function(er) {
				//alert('تخصیص موفق نبود.');
				window.toast.error('تخصیص موفق نبود.', 'خطا');
				console.error(er);
			}
		});
	};
	return {
		openUsersList: openUsersList,
		assignWorksToUser: assignWorksToUser
	};
}();