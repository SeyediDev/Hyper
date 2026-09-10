function submitColumnSettings(modalKey) {
    var selectedColumns = [];

    if (!!$("#selectedDimensions-" + modalKey)) {
        $("#selectedDimensions-" + modalKey + " li")
            .each(function () {
                selectedColumns[selectedColumns.length] =
                    {
                        EntityId: $(this).attr('column-entityId'),
                        AssociationName: $(this).attr('column-associationName'),
                        FieldId: $(this).attr('column-name'),
                        AggrId: $(this).attr('column-aggrId'),
                        Alias: $(this).find('.alias').text().trim(),
                        Formula: $(this).attr('column-formula')
                    };
            });
    }
    $("#selectedColumns-" + modalKey + " li")
        .each(function () {
            var selectedColumn = {
                EntityId: $(this).attr('column-entityId'),
                AssociationName: $(this).attr('column-associationName'),
                FieldId: $(this).attr('column-name'),
                AggrId: $(this).attr('column-aggrId'),
                Alias: $(this).find('.alias').text().trim(),
                Formula: $(this).attr('column-formula')
            };
            if (selectedColumn.AggrId === "GroupByItem")
                selectedColumn.AggrId = "InColumn";
            if ($(this).find(".tooltip-boolean").hasClass("tooltipYes")) {
                selectedColumn.IsTooltip = true;
            } else {
                selectedColumn.IsTooltip = false;
            }
            if ($(this).find(".matrixType-item").hasClass("matrixTypeVertical")) {
                selectedColumn.MatrixType = 2;
            } else {
                selectedColumn.MatrixType = 1;
            }
            selectedColumns[selectedColumns.length] = selectedColumn;
        });
    var havingConstraint = $("#HavingConstraint-" + modalKey).val().trim();
    var constraint = $("#Constraint-" + modalKey).val().trim();

    var ajaxParams = {
        NamespaceId: window.top.modalObjects[modalKey].NamespaceId,
        EntityId: window.top.modalObjects[modalKey].EntityId,
        ReportId: window.top.modalObjects[modalKey].ReportId,
        ConfigId: window.top.modalObjects[modalKey].ConfigId,
        SelectedColumns: selectedColumns,
        havingConstraint: havingConstraint,
        constraint: constraint
    };
    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Report/ApplyColumns',
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
            alert(window.tetaI18n.t('Error Occured'));
            console.error(e);
//          Error(e);
        }
    });
}

var findSelectedColumnLi = function (modalKey, entityId, associationName, columnId, aggregationId) {
    return $('#selectedColumns-' + modalKey + ' li[column-entityId="' + entityId + '"]'
        + '[column-name="' + columnId + '"]'
        + '[column-aggrId="' + aggregationId + '"]'
        + (associationName ? ('[column-associationName="' + '"]') : '')
    );
}

$(".sortable-report-dimensions")
    .on('click',
        'li',
        function (e) {
            if (e.ctrlKey || e.metaKey) {
                $(this).toggleClass("selected");
            } else {
                $(this).addClass("selected").siblings().removeClass('selected');
                if ($(this).attr('column-aggrId') === 'Formula') {
                    formulaColumnId = this.id;
                }
            }
        })
    .sortable(
        {
            connectWith: ".sortable-report-dimensions",
            delay: 150, //Needed to prevent accidental drag when trying to select
            revert: 0,
            cursor: "grabbing",
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
                var modalKey = $(this).attr("modalKey");
                elements.each(function (idx, element) {
                    var $element = $(element);
                    var formula = $element.attr('column-formula');
                    var entityId = $element.attr('column-entityId');
                    var associationName = $element.attr('column-associationName');
                    var columnId = $element.attr('column-name');
                    var alias = $element.find('.alias').text();
                    var aggrId = 'InColumn';

                    var selectedLi = findSelectedColumnLi(modalKey, entityId, associationName, columnId, aggrId);
                    if (ui.item.parent()[0].id == "selectedDimensions-" + modalKey) {
                        if (selectedLi.length === 0) {
                            $("#selectedColumns-" + modalKey)
                                .append(createLiHtmlString(modalKey, entityId,
                                    associationName, columnId, aggrId, formula, alias));
                        }
                    } else {
                        selectedLi.remove();
                    }
                });

                //`elements` now contains the originally selected items from the source list (the dragged items)!!

                //Finally I insert the selected items after the `item`, then remove the `item`, since 
                //  item is a duplicate of one of the selected items.
                ui.item.after(elements).remove();
            }
        });

//$(".columnsSettingsModal li")
//    .on('click',
//        function(e) {
//            if (e.ctrlKey || e.metaKey) {
//                $(this).toggleClass("selected");
//            }
//            $(".columnsSettingsModal li.selected").removeClass("selected");
//            $(this).addClass("selected");
//        });

$(".sortable-report-columns")
    .on('click',
        'li',
        function (e) {
            var modalKey = $(this).parent().attr("modalKey");
            if (e.ctrlKey || e.metaKey) {
                $('#showEditFormulaId-' + modalKey).addClass('hidden');

                $(this).toggleClass("selected");
            } else {
                $(".columnsSettingsModal li.selected").removeClass("selected");
                $(this).addClass("selected");

                if ($(this).attr('column-aggrId') === 'Formula') {
                    formulaColumnId = this.id;
                    $('#showEditFormulaId-' + modalKey).removeClass('hidden');
                } else {
                    $('#showEditFormulaId-' + modalKey).addClass('hidden');
                }
            }
        })
    .sortable({
        connectWith: ".sortable-report-columns",
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

function renameColumn(obj) {
    var $alias = $(obj).find('.alias');
    var text = $alias.text().trim();
    var result = prompt('عنوان', text);
    if (result !== null)
        $alias.text(result);
}


var formulaColumnId = '';
var isEdit = false;

function showEditFormulaModal(modalKey) {
    var newFormulaName = $('[id="' + formulaColumnId + '"]').text();
    var newFormulaBody = $('[id="' + formulaColumnId + '"]').attr('column-formula');
    isEdit = true;
    $('#addFormulaModal-' + modalKey).modal('show');
    $('#newFormulaName-' + modalKey).val(newFormulaName);
    $('#newFormula-' + modalKey).val(newFormulaBody);

    $('#addFormulaModal-label-' + modalKey).text(window.tetaI18n.t('EditFormula'));
}

function showAddNewFormulaModal(modalKey) {
    isEdit = false;
    $('#addFormulaModal-' + modalKey).modal('show');
    $('#newFormulaName-' + modalKey).val('');
    $('#newFormula-' + modalKey).val('');
    $('#addFormulaModal-label-' + modalKey).text(window.tetaI18n.t('AddNewFormula'));
}

function addToFormulas(modalKey) {
    var newFormulaName = $('#newFormulaName-' + modalKey).val().trim();
    var newFormula = $('#newFormula-' + modalKey).val().trim();
    if (isEdit) {
        $('[id="' + formulaColumnId + '"]').text(newFormulaName);
        $('[id="' + formulaColumnId + '"]').attr('column-formula', newFormula);
    } else {
        var formulaId = "sjFORMULAvs" + parseInt(Math.random() * 100);
        var aggrType = formulaAggrId;
        var entityId = window.top.modalObjects[modalKey].EntityId;
        $('#selectedColumns-' + modalKey)
            .append(createLiHtmlString(modalKey, entityId, "", formulaId, aggrType, newFormula, newFormulaName));
    }
    $('#addFormulaModal-' + modalKey).modal("hide");
}

function addNewItemToFormula(modalKey, item, firstId) {
    if (item == "Count(*)")
        item = "Count(" + firstId + ")";
    var newFormulaObj = $("#newFormula-" + modalKey);
    var oldval = newFormulaObj.val();
    if (oldval)
        oldval += " ";
    newFormulaObj.val(oldval + item);
    newFormulaObj.focus();
}

function createLiHtmlString(modalKey, entityId, associationName, columnId, aggrType, formula, alias) {
    var $li = $("<li/>")
        .attr('column-formula', formula)
        .attr('id', modalKey + entityId + associationName + columnId + aggrType)
        .attr('column-entityId', entityId)
        .attr('column-associationName', associationName)
        .attr('column-name', columnId)
        .attr('column-aggrId', aggrType)
        .attr('ondblclick', 'renameColumn(this)');

    var $span = $("<span/>")
        .attr('class', 'alias')
        .text(alias)
        .appendTo($li);
    
    $li.append("<div class=\"float-left\"><i class=\"fa  tooltip-boolean fa-comment tooltipNo\" onclick=\"toggleTooltip(this)\"></i></div>" +
        (aggrType === "InColumn"
            ? "<div class=\"float-left\"><i class=\"fa  matrixType-item fa-cube matrixTypeHorizontal\" onclick=\"toggleMatrixType(this)\"></i></div>"
            : ""));
    
    return $li;
}

function toggleTooltip(el) {
    $(el).toggleClass('tooltipYes').toggleClass('tooltipNo');
}

function toggleMatrixType(el) {
    $(el).toggleClass('matrixTypeVertical').toggleClass('matrixTypeHorizontal');
}