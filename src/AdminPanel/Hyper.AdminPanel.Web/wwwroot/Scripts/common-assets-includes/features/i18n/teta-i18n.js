(function() {
    window.tetaI18n = function() {
        var tetaJsTexts = {};

		  var fetchText = function (txtKey) {
				var value = tetaJsTexts[txtKey];
				if (!value) {
					 console.warn(txtKey + ' was not found in the tetaI18n dictionary.');
					 value = txtKey;
				}
				if (arguments.length > 1) {
					for (var i = 1; i < arguments.length; i++) {
						value = value.replace('{' + (i - 1) + '}', arguments[i]);
					}
				}
            return value;
        };

        var addDictionary = function(dicObj) {
            for (var key in dicObj)
                if (dicObj.hasOwnProperty(key))
                    tetaJsTexts[key] = dicObj[key];
        };

        var setCurrentCulture = function (cultureName) {
            //            $.ajax({
            //                type: "POST",
            //                url: '/Account/ChangeCulture?culture=' + cultureName,                
            //                success: function (result) {
            //                    location.reload();
            //                }
            //            });
            window.Cookies.set('_culture', cultureName, { expires: 365 });
            location.reload();
        };

		  return {
			  /**
			   * Fetches the text in the current language
			   * @param {string} txtKey
				* @param {...string} arguments for values which are can be in a format similar to C# string.format like: 'Hello {0}!'
			   */
            t: fetchText,
              addDictionary: addDictionary,
              setCurrentCulture: setCurrentCulture
        };
    }();
})();