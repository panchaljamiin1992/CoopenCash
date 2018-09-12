(function ($, undefined) {
    $.fn.GridViewFix = function (params) {
        var settings = $.extend({}, { header: "headerstyle", row: "rowstyle", footer: "footerStyle" }, params);
        return this.each(function () {
            var ctxt = $(this), thead = $("").append($("tr.".concat(settings.header), ctxt)),
                    tbody = $("").append($("tr.".concat(settings.row), ctxt)),
                    tfooter = $("").append($("tr.".concat(settings.footer), ctxt));
            $("tbody", ctxt).remove();
            ctxt.append(thead).append(tbody).append(tfooter)
        });
    }
})(jQuery)