var SortingManager = function () {
    var submitSorting = function (sortingStr) {
        $("#soValues").val(sortingStr);
        $("input[name='Page']").val(1);
        $("input[name='Page']").closest("form").submit();
    };

    var makeSortPartString = function (columnName, sortType) {
        var sortsStr = columnName + " ";

        switch (sortType) {
        case "noSort":
            sortsStr += "ASC";
            break;
        case "ASC":
            sortsStr += "DESC";
            break;
        case "DESC":
            sortsStr = "";
            break;
        default:
            throw "Invalid sorting attribute";
        }
        return sortsStr;
    };

    var columnsAreSame = function (sortPartA, referNameB) {
        return sortPartA.lastIndexOf(referNameB, 0) === 0;
//        return sortPartA.split(' ')[0] === sortPartB.split(' ')[0];
    };

    var mergeSorts = function(newSortPart, newPartColumnName) {
        var alreadySortsString = $("#soValues").val();
        var alreadySortParts = alreadySortsString.split('#');
        var found = false;
        for (var i = 0; i < alreadySortParts.length; i++) {
            if (columnsAreSame(alreadySortParts[i], newPartColumnName)) {
                found = true;

                alreadySortParts[i] = newSortPart;
                break;
            }
        }
        if (found) 
            return alreadySortParts.filter(function(p){return p!==''}).join('#');
        return alreadySortsString + (newSortPart ? '#' + newSortPart : '');
    };

    var changeSort = function (event, element) {
        var refer = $(element).data("refer"),
            currentSortType = $(element).data("sort");
        var newSortPart = makeSortPartString(refer, currentSortType);
        var sortsStr = event.ctrlKey ? mergeSorts(newSortPart, refer) : newSortPart;
        submitSorting(sortsStr);
    };
    
    return {
        changeSort: changeSort
    };
}();