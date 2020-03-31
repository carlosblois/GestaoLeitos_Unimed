var x = 1;

var y = 2;

var velocity = 3;
var step3 = 0;
$(document).ready(function () {


    $('.bt_close_filter').click(function () {
        //$('#box_filter').toggle('slide');
        $('#box_filter').hide();
        $('#bt_filter').show();
    });

    $('#bt_filter').click(function () {
        //$('#box_filter').show('slide', { direction: 'left' }, 450);
        $('#box_filter').show();
        $('#bt_filter').hide();
    });

    $('.scrolling').scrollTop();
    $('.arrow_right').click(function () {
        $('.jquery-modal').removeClass('hideModal');
    });
    $('.close-modal').click(function () {
        $('.jquery-modal').addClass('hideModal');
    });

    setInterval(function () {
        $('.scrolling').height($(window).height() - 250);
        $('.boxIntern').height($(window).height() - 250);
    }, 1);
});

function showCoordsIntern(event) {
    var x = event.clientX;
    if (x < 300) {
        step3 = -velocity;
    }
    if (x > (document.getElementById("pannelActivities").offsetWidth - 300)) {
        step3 = velocity;
    }
}

function stopshowCoordsIntern() {
    step3 = 0;
}