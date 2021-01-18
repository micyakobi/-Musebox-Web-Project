


$(function () {

    var form = $('#usE');
    var url = form.attr('action');


    $('#Nuser').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                 $('tbody').html(data);
            }
        });

    });
    $('#FNuser').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
    $('#LNuser').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
    $('#Euser').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
});