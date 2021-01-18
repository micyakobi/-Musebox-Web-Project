


$(function () {

    var form = $('#caTa');
    var url = form.attr('action');


    $('#Cname').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                 $('#mybody').html(data);
            }
        });

    });
    $('#Cprice').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('#mybody').html(data);
            }
        });

    });
    $('#Ctype').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('#mybody').html(data);
            }
        });

    });
    $('#Cbrand').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('#mybody').html(data);
            }
        });

    });
});