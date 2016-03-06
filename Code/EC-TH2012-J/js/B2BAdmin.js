$(document).ready(function () {
    $("#basic").submit(function (event) {

        var url = "/AdminB2B/GetOrders";
        username = $("#username").val();
        password = $("#password").val();
        supp = $("#supplier_key").val();
        $.ajax({
            url: url,
            type: "POST",
            data: { user: username, pass: password, supplier_key: supp },
            success : function(result)
            {
                $(".Ajax-Table").html(result);
            }
        });
        return false;
    });
    $("#Oauth").submit(function (event) {

        var url = "/AdminB2B/GetOrdersOauth";
        access = $("#access_token").val();
        supplier = $("#supplier_key1").val();
        supp = $("#supplier_key").val();
        $.ajax({
            url: url,
            type: "POST",
            data: { access_token: access, supplier_key: supplier },
            success: function (result) {
                $(".Ajax-Table").html(result);
            }
        });
        return false;
    });
});