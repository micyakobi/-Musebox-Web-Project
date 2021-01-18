


$(function () {

    var form = $('#brandS');
    var url = form.attr('action');


    $('#brandName').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
               // alert(data);
                 $('tbody').html(data);
            }
        });

    });
});