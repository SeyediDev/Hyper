function addLevelInputs() {
	var markup =
		'<div class="row"><div class="col-sm-6"><fieldset>' +
			'<input type="hidden" name="levels[' +
			inputsCount +
			'].MinType" value="Fixed">' +
//        '<div class="radio">'
//        + '<label><input type="radio" name="levels[' + inputsCount
//        + '].MinType" value="Field">از فیلد بخوان</label></div><div class="radio">' +
//        '<label><input type="radio" name="levels[' + inputsCount + '].MinType" value="Fixed">مقدار ثابت</label></div>' +


//        '<div class="form-group"><label for="sel'+inputsCount+'1">فیلد:</label>' +

//        '<select class="form-control" id="sel'+inputsCount+'" name="levels['+inputsCount+'].MinColumnValue">' +

//        '@foreach (var column in structure.SelectedColumns)
//                                        {
//                                            <option value="@column.ColumnTypeName">@column.Alias</option>
//                                        }
//                                    </select>
//                                '</div>' +
			'<div class="form-group"><label for="usr' +
			inputsCount +
			'1">' +
			window.tetaI18n.t('ConstantMin') +
			':</label><input type="number" step="any" class="form-control" id="usr' +
			inputsCount +
			'1" name="levels[' +
			inputsCount +
			'].MinFixedValue">' +
			'</div></fieldset></div><div class="col-sm-6"><fieldset>' +
			'<input type="hidden" name="levels[' +
			inputsCount +
			'].MaxType" value="Fixed">' +

//                                '<div class="radio"><label><input type="radio" name="levels[' + inputsCount + '].MaxType" value="Field">از فیلد بخوان</label></div><div class="radio"><label><input type="radio" name="levels[' +
//                                inputsCount + '].MaxType" value="Fixed">مقدار ثابت</label></div>' +

//        '<div class="form-group"><label for="sel'+inputsCount+'2">فیلد:</label>' +
//        '<select class="form-control" id="sel12" name="levels[1].MaxColumnValue">
//                                        @foreach (var column in structure.SelectedColumns)
//                                        {
//                                            <option value="@column.ColumnTypeName">@column.Alias</option>
//                                        }
//                                    </select>
//                                </div>
			'<div class="form-group"><label for="usr' +
			inputsCount +
			'2">' +
			window.tetaI18n.t('ConstantMax') +
			':</label><input type="number" step="any" class="form-control" id="usr' +
			inputsCount +
			'2" name="levels[' +
			inputsCount +
			'].MaxFixedValue"></div></fieldset></div><div class="col-md-12"><div class="form-group"><label for="clr' +
			inputsCount +
			'">' +
			window.tetaI18n.t('Color') +
			':</label><input type="color" class="form-control" id="clr' +
			inputsCount +
			'" name="levels[' +
			inputsCount +
			'].Color"></div></div></div>';

	$('#range-levels').append(markup);
	inputsCount++;
}

$(function() {
	var frm = $('#report-settings-form');
	frm.submit(function(ev) {
		$.ajax({
			type: frm.attr('method'),
			url: frm.attr('action'),
			data: frm.serialize(),
			headers: AddAntiForgeryToken(),
			success: function(data) {
				location.reload();
			}
		});
		ev.preventDefault();
	});
});