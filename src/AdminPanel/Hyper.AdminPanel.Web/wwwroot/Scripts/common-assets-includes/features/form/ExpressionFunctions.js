// CEF stands for Client-side Expression Functions
window.CEF = function () {
    
    var isTableEmpty = function (name) {
        return $('table[name="' + name + '"] tbody tr:not([doppelganger])').length === 0;
    };

    return {
        IsTableEmpty: isTableEmpty
    };
}();