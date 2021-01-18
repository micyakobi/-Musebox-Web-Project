


$(function () {

    var form = $('#brnC');
    var url = form.attr('action');


    $('#branchN').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                 $('tbody').html(data);
            }
        });

    });
    $('#branchA').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
    $('#branchLA').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
    $('#branchLn').keyup(function () {

        $.ajax({
            url: url,
            data: form.serialize(),
            success: function (data) {

                $('tbody').html(data);
            }
        });

    });
});