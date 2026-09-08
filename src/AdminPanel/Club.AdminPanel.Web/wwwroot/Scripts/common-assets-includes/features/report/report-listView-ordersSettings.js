function submitOrdersSettings(modalKey) {
    var sortFields = "";
    var isFirst = true;
    $("#selected_Columns-" + modalKey + " li")
        .each(function () {
            if (!isFirst)
                sortFields += "#";
            else
                isFirst = false;

            var aggrId = $(this).attr("column-aggrId");
            var sortField = $(this).attr("column-name");
            if (aggrId === "Formula")
                sortField = $(this).attr("column-formula").trim().replace(" ", "");
            if ($(this).find(".governor").hasClass("asc")) {
                sortField += " ASC";
            } else {
                sortField += " DESC";
            }
            if (aggrId !== "InColumn" && aggrId !== "GroupByItem")
                sortField = aggrId + "$" + sortField;
            sortField += " " + $(this).attr("column-entityId");            
            if ($(this).find(".governor2").hasClass("by-id")) {
                sortField += " True";
            } else {
                sortField += " False";
            }
            sortField += " " + $(this).attr("column-associationName");
            
            sortFields += sortField;
        });

    var ajaxParams = {
        NamespaceId: window.top.modalObjects[modalKey].NamespaceId,
        EntityId: window.top.modalObjects[modalKey].EntityId,
        ReportId: window.top.modalObjects[modalKey].ReportId,
        ConfigId: window.top.modalObjects[modalKey].ConfigId,
        SortFields: sortFields
    };

    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Report/ApplyOrdersSettings',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: AddAntiForgeryToken(),
        success: function (res) {
            location.reload();
        },
        error: function (e) {
				window.toast.error(window.tetaI18n.t('Error Occured') + e.responseText);
            Error(e);
        }
    });
}
$(".order-sortable-report-columns")
    .on('click',
        'li',
        function (e) {
            if (e.ctrlKey || e.metaKey) {
                $(this).toggleClass("selected");
            } else {
                $(this).addClass("selected").siblings().removeClass('selected');
            }
        })
    .sortable({
        connectWith: "ul",
        delay: 150, //Needed to prevent accidental drag when trying to select
        revert: 0,
        helper: function (e, item) {
            //Basically, if you grab an unhighlighted item to drag, it will deselect (unhighlight) everything else
            if (!item.hasClass('selected')) {
                item.addClass('selected').siblings().removeClass('selected');
            }

            //////////////////////////////////////////////////////////////////////
            //HERE'S HOW TO PASS THE SELECTED ITEMS TO THE `stop()` FUNCTION:

            //Clone the selected items into an array
            var elements = item.parent().children('.selected').clone();

            //Add a property to `item` called 'multidrag` that contains the 
            //  selected items, then remove the selected items from the source list
            item.data('multidrag', elements).siblings('.selected').remove();

            //Now the selected items exist in memory, attached to the `item`,
            //  so we can access them later when we get to the `stop()` callback

            //Create the helper
            var helper = $('<li/>');
            return helper.append(elements);
        },
        stop: function (e, ui) {
            //Now we access those items that we stored in `item`s data!
            var elements = ui.item.data('multidrag');

            //`elements` now contains the originally selected items from the source list (the dragged items)!!

            //Finally I insert the selected items after the `item`, then remove the `item`, since 
            //  item is a duplicate of one of the selected items.
            ui.item.after(elements).remove();
        }

    });
function toggleOrderType(el) {
    $(el).toggleClass('governor');
    $(el).siblings('.order-type').toggleClass('governor');
};

function toggleById(el) {
    $(el).toggleClass('governor2');
    $(el).siblings('.key-boolean').toggleClass('governor2');
}