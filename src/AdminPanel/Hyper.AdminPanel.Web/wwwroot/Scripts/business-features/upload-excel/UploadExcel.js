$(".upload").click(function (e) {
    $(".hide").trigger("click");
});

AddAntiForgeryToken = function (data) {
    return { "__RequestVerificationToken": $('input[name="__RequestVerificationToken').val() };
    //data.__RequestVerificationToken = $('input[name="__RequestVerificationToken').val();
    //return data;
};
function loadData(urlAddress, select, cb, queryObject, isUpdate) {    
    if (!isUpdate && $(select).attr("loaded") == "true") {
        $(select).trigger("mousedown");
    } else {
    	var //referenceId, errorMessage,
			data = queryObject || {};
        var obj = JSON.stringify(data);
        $.ajax({
            url: window.top.rootUrl + urlAddress,
            dataType: "json",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            async: true,
            processData: true,
            cache: false,
            data: obj,
            headers: AddAntiForgeryToken(),
            success: function (data) {                
                if (data && data.Rows) {
                	var r = data.Rows;
                    select.blur();
	                //select.clear();
                    //                    $(select).html('<option value=""></option>');
                    $(select).html('');
                    for (var i = 0, len = r.length; i < len; i++) {
                        $(select).append('<option value=' + r[i].Ids + '>' + r[i].DisplayValue + '</option>');
                    }
                    $(select).attr("loaded", true);
                    $(select).val(data.selectedId);
                   /* if (document.createEvent) {
                        var e = document.createEvent("MouseEvents");
                        e.initMouseEvent("mousedown", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
                        worked = select.dispatchEvent(e);
                    } else if (select.fireEvent) {
                        worked = select.fireEvent("onmousedown");
                    }
                    $(select).trigger("mousedown");*/
                    if (cb) {
	                    cb();
                    }
                }
            },
            error: function (xhr) {
                console.log(xhr);
                //alert('خواندن اطلاعات با مشکل مواجه شده است.');
                window.toastr.warning('خواندن اطلاعات با مشکل مواجه شده است.');
                Error(xhr);
            }
        });
    }
}
function checkLength(input) {
    var val = $(input).val().trim();
    if (val.length > 4) {
        val = val.substring(0, 4);
        $(input).val(val);
    }
}
function uploadValueChange(args) {
    var vObj = args.value.split('\\');
    var val = vObj[vObj.length - 1];
    $("#fileStatus").html(val);
}
function submitExcelFile(btn, left) {
    //if (left) {
    //    $('input[name="left"]').val(true);
    //} else {
    //    $('input[name="left"]').val(false);
    //}
    var args = $("input[type='file']")[0];
    var vObj = args.value.split('\\');
    var val = vObj[vObj.length - 1];
    if (val) {
        $(btn).closest("form").find("#btnSubmit").click();
    } else {
        alert("ابتدا فایل مورد نظر خود را انتخاب کنید");
    }
}
