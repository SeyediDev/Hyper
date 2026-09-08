$.get(window.top.rootUrl + 'Enums/FormControlTypes')
    .then(function(res) { window.controlTypes = res }); //todo not robust
var getControlTypes = function() {
    return window.controlTypes;
};
var formDesigner = new FormDesigner(window.jQuery,
    window.top.rootUrl,
    window.controlModels,
    window.propertyTypes,
    getControlTypes,
    false, true);

var formSaver = new FormDesignSaver(window.jQuery,
    window.toast,
    window.top.rootUrl,
    true,
    AddAntiForgeryToken);

function submitColumnSettings() {
    var selectedElements = [];
    
    $("#selectedColumns li")
        .each(function() {
            var $li = $(this);
            selectedElements[selectedElements.length] = {
                ControlId: $li.attr('column-name'),
                Label: $li.find('.alias').text().trim(),
                FormItemType: 4 //todo ColumnField
                ,
                Properties: formDesigner.getProperties($li.attr('column-name'))
            };
        });

    $("#selectedSubjects li")
        .each(function() {
            var $li = $(this);
            selectedElements[selectedElements.length] = {
                ControlId: $li.attr('column-name'),
                Label: $li.find('.alias').text().trim(),
                FormItemType: 5,//todo SubjectField
                Subject: {
                    FormSubjectId: $li.attr('column-name'), //todo
                    HasEdit: $li.find(".has-edit").hasClass("editYes"),
                    HasDetails: $li.find(".has-details").hasClass("detailsYes")
                }
            };
        });
    
    formSaver.saveTheForm(selectedElements, true);
}

function renameColumn(liElem) {
    var $alias = $(liElem).find('.alias');    
    var text = $alias.text().trim();
    var result = prompt('عنوان', text);
    if (result !== null)
        $alias.text(result);
}
function toggleDetails(el) {
    $(el).toggleClass('detailsYes').toggleClass('detailsNo');
}
function toggleEdit(el) {
    $(el).toggleClass('editYes').toggleClass('editNo');
}

$(".sortable-form-columns")
    .on('click',
        'li',
        function(e) {
            if (e.ctrlKey || e.metaKey) {
                $(this).toggleClass("selected");
            } else {
                $(this).addClass("selected").siblings().removeClass('selected');
            }
        })
    .sortable({
        connectWith: ".sortable-form-columns",
        delay: 150, //Needed to prevent accidental drag when trying to select
        revert: 0,
        helper: function(e, item) {
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
        stop: function(e, ui) {
            //Now we access those items that we stored in `item`s data!
            var elements = ui.item.data('multidrag');            
            //`elements` now contains the originally selected items from the source list (the dragged items)!!

            //Finally I insert the selected items after the `item`, then remove the `item`, since 
            //  item is a duplicate of one of the selected items.
            ui.item.after(elements).remove();
        }

    });


