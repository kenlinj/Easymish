$(document).ready(function () {
    $("#footer").stickyfooter();
    //stickyFooter();
    //$(window).scroll(stickyFooter).resize(stickyFooter);
});

function stickyFooter() {
    var bodyHeight = $("body").height();
    var windowHeight = $(window).height();
    var footerHeight = $("#footer").height();
    if (windowHeight > bodyHeight + footerHeight) {
        $("#footer").css("position", "absolute").css("bottom", 0);
    }
    else {
        $("#footer").css("position", "inherit").css("bottom", "inherit");
    }
}
