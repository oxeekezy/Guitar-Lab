$(function () {
    /*Page delete query*/

    $("a.delete").click(function () {
        if (!confirm("Confirm page deletion")) return false;
    });

    /*-------------------------------------------------------*/

    /*Sorting drandrop script */

    $("table#pages tbody").sortable
        ({
            items: "tr:not(.home)",
            placeholder: "ui-state-highlight",
            update: function () {
                var ids = $("table#pages tbody").sortable("serialize");
                var url = "/Admin/Pages/ReorderPages";
                $.post(url, ids, function (data) { });
            }
        });

});