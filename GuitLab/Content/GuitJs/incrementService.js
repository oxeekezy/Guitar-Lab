$(function () {

    $("a.incservice").click(function (e) {
        e.preventDefault();

        var ServiceId = $(this).data("id");
        var url = "/cart/IncrementService";

        $.getJSON(url, { ServiceId: ServiceId }, function (data) {
            $("td.qty" + ServiceId).html(data.qty);

            var price = data.qty * data.price;
            var priceHtml = price.toFixed(2) + "$";

            $("td.total" + ServiceId).html(priceHtml);

            var gt = parseFloat($("td.grandtotal span").text())
            var grandtotal = (gt + data.price).toFixed(2);

            $("td.grandtotal span").text(grandtotal+"$");
        });
    });
});

/*-----------------------------------------------------------*/

/* Decriment Service */
$(function () {

    $("a.decService").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var ServiceId = $(this).data("id");
        var url = "/cart/DecrementService";

        $.getJSON(url, { ServiceId: ServiceId }, function (data) {

            if (data.qty == 0) {
                $this.parent().fadeOut("fast", function () {
                    location.reload();
                });
            }
            else {
                $("td.qty" + ServiceId).html(data.qty);

                var price = data.qty * data.price;
                var priceHtml = price.toFixed(2) + "$";

                $("td.total" + ServiceId).html(priceHtml);

                var gt = parseFloat($("td.grandtotal span").text());
                var grandtotal = (gt - data.price).toFixed(2);

                $("td.grandtotal span").text(grandtotal);
            }
        });
    });
});
/*-----------------------------------------------------------*/

/* Remove Service */
$(function () {

    $("a.removeService").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var ServiceId = $(this).data("id");
        var url = "/cart/RemoveService";

        $.get(url, { ServiceId: ServiceId }, function (data) {
            location.reload();
        });
    });
});
/*-----------------------------------------------------------*/
