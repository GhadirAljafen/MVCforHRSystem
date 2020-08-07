var popup;
function PopupForm(url) {
    var formDiv = $('<div/>');
    $.get(url)
        .done(function (response) {
            formDiv.html(response);
            popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                height: 500,
                width: 700,
                close: function () {
                    popup.dialog('destroy').remove();
                }
            });
        });
        //.always(function () {
        //    alert("SERVER DOWN");
        //});
}
   

function SubmitForm(form) {
    var formData = new FormData(form);
     // to validate the form before submiting
    $.validator.unobtrusive.parse(form);
    if ($(form).valid()) {
        $.ajax({
            type: "POST",
            url: form.action,
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.success) {
                    popup.dialog('close');
                    //refresh table
                    $('#datatable').DataTable().ajax.reload();
                    $.notify(data.message, {
                        globalPosition: 'top center',
                        className: "success"
                    });
                }
                else {
                    $.notify(data.message, {
                        globalPosition: 'top center',
                        className: "error"
                    });
                }

            }

        });
    }
    return false

}