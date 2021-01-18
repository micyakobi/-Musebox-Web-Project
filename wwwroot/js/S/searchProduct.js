


$(function () {

    var form = $('#proD');
    var url = form.attr('action');


    $('#productN').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                 $('tbody').html(data);
            }
        });

    });
    $('#productP').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
    $('#productT').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
    $('#productB').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
});