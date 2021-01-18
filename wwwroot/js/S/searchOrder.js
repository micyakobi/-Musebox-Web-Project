


$(function () {

    var form = $('#orD');
    var url = form.attr('action');


    $('#Norder').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                $('tbody').html(data);
            }
        });

    });

    $('#Dorder').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                $('tbody').html(data);
            }
        });

    });
});