/*
stickyfooter.js

add watermark when this function is invoked. nornally it should be invoked in document.ready().
we must deinfe watermarkOn in css file.
*/
(function ($) {
    $.fn.stickyfooter = function (options) {
        var bodyHeight = $("body").height();
        var windowHeight = $(window).height();
        var footerHeight = $(this).height();
        if (windowHeight > bodyHeight + footerHeight) {
            $(this).css("position", "absolute").css("bottom", 0);
        }
        else {
            $(this).css("position", "inherit").css("bottom", "inherit");
        }

        $(window).scroll(stickyFooter).resize(stickyFooter);
    };
}(jQuery));
