window.instantiateMenu = function () {    
    var getBottomContent = function () {
        var bottomContent = '<div class="mx-auto p-0">';
        var hasSomething = false;
        if (window.tetaConfigs.brandContent) {
            bottomContent += window.tetaConfigs.brandContent;
            hasSomething = true;
        }
        if (window.tetaConfigs.version) {
            bottomContent += "<div class='version-display'>" +
                window.tetaI18n.t('Version') +
                " : " +
                window.tetaConfigs.version +
                " </div>";
            hasSomething = true;
        }
        bottomContent += '</div>';
        return hasSomething ? bottomContent : null;
    };

    if (!window.tetaConfigs)
        throw new Error('window.tetaConfigs is needed');
    var navbars = [
        {
            "position": "top",
            "content": [
                "searchfield"
            ]
        },
        {
            "position": "top",
            "content": [
                "breadcrumbs"
            ]
        }
    ];
    var bottomContent = getBottomContent();
    if (bottomContent) {
        navbars.push(
            {
                "position": "bottom",
                "content": bottomContent
            });
    }
    $('nav#menu').mmenu({
        offCanvas: {
            position: window.tetaConfigs.isRtl ? "right" : "left",
            "zposition": "front"
        },
        "navbars": navbars,
        "extensions": [
            "position-front",
            window.tetaConfigs.isRtl ? "position-right" : "position-left"
        ]
    },
        {
            "searchfield": {
                "clear": true,
                "panel": true
            },
            language: window.tetaConfigs.neutralCulture
        });
};
