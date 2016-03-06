function LoadMorePro(skipnum,action,id,id1) {
    var url = "/Home/"+action;
    $.ajax({
        url: url,
        type: 'Get',
        data: {skip:skipnum},
        success: function (result) {
            var a = result.toString();
            if (!a)
            {
                $(id).html(a);
            }
            else
            {
                $(id1).append(result);
                skipnum += 4;
                var fixa = "<a class=\"btn-loadmore\" href=\'javascript:LoadMorePro(" + skipnum.toString() + ",\"" + action + "\",\"" + id + "\",\"" + id1 + "\")\'>";
                fixa += "<i class=\"fa fa-plus\"></i>Tải thêm</a>";
                $(id).html(fixa);
            }
        },
    });
}