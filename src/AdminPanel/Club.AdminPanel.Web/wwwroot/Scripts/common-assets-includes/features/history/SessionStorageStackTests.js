var SessionStorageStackTest = function () {

	var pushTest = function (item) {
		SessionStorageStack.push('TetaHistory', item);
		console.log("pushed items is ");
		console.log(sessionStorage.getItem('TetaHistory'));
	};

	var popTest = function () {
		var temp = SessionStorageStack.pop('TetaHistory');
		console.log("poped item is ");
		console.log(temp);
		console.log(sessionStorage.getItem('TetaHistory'));
	};

	var topTest = function () {
		var tempr = SessionStorageStack.top('TetaHistory');
		console.log("last item is ");
		console.log(tempr);
	};

	return {
		pushTest: pushTest,
		popTest: popTest,
		topTest: topTest
	};
}();