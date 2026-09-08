/* Normalize iran_mill jVectorMap region codes to ISO-3166-2 (IR-28 = Qom, etc.) */
(function () {
	'use strict';
	var tries = 0, maxTries = 60, delay = 200;
	var titleToIso = {
		'آذربایجان شرقی': 'IR-01',
		'آذربایجان غربی': 'IR-02',
		'اردبیل': 'IR-03',
		'اصفهان': 'IR-04',
		'ایلام': 'IR-05',
		'بوشهر': 'IR-06',
		'تهران': 'IR-07',
		'چهارمحال و بختیاری': 'IR-08',
		'خراسان جنوبی': 'IR-09',
		'خراسان رضوی': 'IR-10',
		'خراسان شمالی': 'IR-11',
		'خوزستان': 'IR-12',
		'زنجان': 'IR-13',
		'سمنان': 'IR-14',
		'سیستان و بلوچستان': 'IR-15',
		'فارس': 'IR-16',
		'کرمان': 'IR-17',
		'کردستان': 'IR-18',
		'کرمانشاه': 'IR-19',
		'کهگیلویه و بویراحمد': 'IR-20',
		'گیلان': 'IR-21',
		'لرستان': 'IR-22',
		'مازندران': 'IR-23',
		'مرکزی': 'IR-24',
		'هرمزگان': 'IR-25',
		'همدان': 'IR-26',
		'یزد': 'IR-27',
		'قم': 'IR-28',
		'گلستان': 'IR-29',
		'قزوین': 'IR-30',
		'البرز': 'IR-32'
	};
	function normalize() {
		var maps = (window.jQuery && window.jQuery.fn && window.jQuery.fn.vectorMap && window.jQuery.fn.vectorMap.maps) || null;
		if (!maps || !maps['iran_mill']) return false;
		var def = maps['iran_mill'];
		var paths = def.paths || {};
		var newPaths = {};
		for (var key in paths) {
			if (!Object.prototype.hasOwnProperty.call(paths, key)) continue;
			var region = paths[key];
			if (!region || !region.name) continue;
			var iso = titleToIso[region.name] || key;
			// keep first occurrence in case of duplicates
			if (!newPaths[iso]) newPaths[iso] = region;
		}
		def.paths = newPaths;
		// re-register the map with normalized codes
		try {
			window.jQuery.fn.vectorMap('addMap', 'iran_mill', def);
		} catch (e) { /* no-op */ }
		return true;
	}
	var timer = setInterval(function () {
		tries++;
		if (normalize() || tries >= maxTries) {
			clearInterval(timer);
		}
	}, delay);
})(); 







