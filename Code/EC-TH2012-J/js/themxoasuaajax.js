//PhanTrangAjax
$(document).ready(function () {
    $(document).on("click", "#nav_grid a[href]", function () {
        $.ajax({
            url: $(this).attr("href"),
            type: 'GET',
            cache: false,
            success: function (result) {
                $('.Ajax-Table').html(result);
                $('html, body').animate({
                    scrollTop: $(".Ajax-Table").offset().top
                }, 500);
            }
        });
        return false;
    });
});
$(document).ready(function () {
    $(document).on("click", "#nav_grid1 a[href]", function () {
        $.ajax({
            url: $(this).attr("href"),
            type: 'GET',
            cache: false,
            success: function (result) {
                $('.Ajax-Table-1').html(result);
                $('html, body').animate({
                    scrollTop: $(".Ajax-Table-1").offset().top
                }, 500);
            }
        });
        return false;
    });
});
$(document).ready(function () {
    $('.form-group').on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault();
        var s = $(this).parent();
        s.find("input").val('');
        $(this).parent().hide(500);
    })
});


//hien anh khi pick file
function readURL(input, id) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(id)
              .attr('src', e.target.result)
              .width(200)
              .height(200);
        };
        reader.readAsDataURL(input.files[0]);
    }
}

//Xoa item ajax
function XoaItem(Url, value) {
    if (confirm("Bạn có chắc muốn xóa dữ liệu?") == true) {
        $.ajax({
            url: Url,
            type: 'POST',
            data: { id: value },
            success: function (result) {
                $('.Ajax-Table').html(result);
                $('#alert-info').html("Xóa dữ liệu thành công");
                $('#alert-info').fadeIn(1000);
                $('#alert-info').fadeOut(3000);
            },
        });
    }


}


//ChiTietItemajax
function ChiTietItem(Url, value) {
    $.ajax({
        url: Url,
        type: 'GET',
        data: { id: value },
        success: function (result) {
            $('#show-dialog-detail').html(result);
            $('#show-dialog-detail').fadeIn(500);
            //$('#alert-info').fadeOut(3000);
        },
    });
}


//tat bang chi tiet item
function closeDialog() {
    $('#show-dialog-detail').fadeOut(500);
}

//check all list
function toggle(source) {
    checkboxes = document.getElementsByName('lstdel');
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        checkboxes[i].checked = source.checked;
    }
}

//multibledel ajax
function multibledel(Url) {
    checkboxes = document.getElementsByName('lstdel');
    var lst = new Array();
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (checkboxes[i].checked) {
            lst.push(checkboxes[i].value);
        }
    }
    if (confirm("Bạn có chắc muốn xóa dữ liệu?") == true) {
        $.ajax({
            url: Url,
            type: 'POST',
            data: { lstdel: lst },
            success: function (result) {
                $('.Ajax-Table').html(result);
                $('#alert-info').html("Xóa dữ liệu thành công");
                $('#alert-info').fadeIn(1000);
                $('#alert-info').fadeOut(3000);
            },
        });
    }
}

function multibleupdate(Url) {
    checkboxes = document.getElementsByName('lstdel');
    var lst = new Array();
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (checkboxes[i].checked) {
            lst.push(checkboxes[i].value);
        }
    }
    if (confirm("Bạn có chắc muốn cập nhật dữ liệu?") == true) {
        $.ajax({
            url: Url,
            type: 'POST',
            data: { lstdel: lst },
            success: function (result) {
                $('.Ajax-Table').html(result);
                $('#alert-info').html("Cập nhật dữ liệu liệu thành công");
                $('#alert-info').fadeIn(1000);
                $('#alert-info').fadeOut(3000);
            },
        });
    }
}

//Submit form by id of form
function submitform(id) {
    document.forms[id].submit();
}

//them thong so kt moi
function ThemTSKTMoi(id) {
    var count = parseInt($('#CountFeld').val(), 10);
    ++count;
    $('#CountFeld').val(count);
    var str = '';
    str += "<div class=\"form-group\" id=\"lstkt" + (count - 1) + "\"><div class=\"col-md-2\">";
    str += "<input name=\"lstkt[" + (count - 1) + "].MaSP\" type=\"hidden\" value=\"" + id + "\">";
    str += "<input class=\"form-control\" name=\"lstkt[" + (count - 1) + "].ThuocTinh\" type=\"text\" value=\"\">";
    str += "</div><div class=\"col-md-4\">";
    str += "<input class=\"form-control\" name=\"lstkt[" + (count - 1) + "].GiaTri\" type=\"text\" value=\"\">";
    str += "</div></div>";
    $('#thongsoktgrp').append(str);
}

//xoa thong so kt
function XoaTSKT() {
    var count = parseInt($('#CountFeld').val(), 10);
    --count;
    $('#CountFeld').val(count);
    var id = "#lstkt" + count;
    $(id).remove();

}



/////////////////////////////
//tim kiem ajax

function thongketheotg() {
    var froms = $('#fday').val();
    var tos = $('#tday').val();
    var html = "<img src=\"/ThongKe/ThongKeDTTheoTG?froms=" + froms + "&tos=" + tos + "\">";
    $('.Ajax-Table').html(html);
}
function thongketheott() {
    var froms = $('#fdaytt').val();
    var tos = $('#tdaytt').val();
    var html = "<img src=\"/ThongKe/ThongKeTiTrong?froms=" + froms + "&tos=" + tos + "\">";
    $('.Ajax-Table-tt').html(html);
}

function timkiemBinhLuan() {
    var key = $('#inputIcon').val();
    var date = $('#comtday').val();
    var status = $('#trangthai').val();
    $.ajax({
        url: "/Comment/TimBinhLuan",
        type: 'GET',
        data: { key: key, date: date, status: status },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemhopdong() {
    var key = $('#mahd').val();
    var tensp = $('#tensp').val();
    var loaihd = $('#loaihd').val();
    $.ajax({
        url: "/HopDong/TimHopDong",
        type: 'GET',
        data: { key: key, tensp: tensp, loaihd: loaihd },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemhopdong1() {
    var key = $('#mahd').val();
    var tensp = $('#tensp').val();
    var loaihd = $('#loaihd').val();
    var tt = $('#ttrang').val();
    $.ajax({
        url: "/HopDong/TTGiaoHang",
        type: 'get',
        data: { key: key, tensp: tensp, loaihd: loaihd, tt:tt },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemspcm() {
    var key = $('#tensp').val();
    $.ajax({
        url: "/SanPhamCanMua/TimSPCM",
        type: 'GET',
        data: { key: key },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemdsdk() {
    var tensp = $('#tensp').val();
    var tenncc = $('#tenncc').val();
    $.ajax({
        url: "/DanhSachDK/TimDS",
        type: 'GET',
        data: { tensp: tensp, tenncc: tenncc },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemdh() {
    var key = $('#madh').val();
    var date = $('#dhday').val();
    var status = $('#trangthai').val();
    var mobile = $('#mobile').val();
    $.ajax({
        url: "/Donhang/TimDonHang",
        type: 'GET',
        data: { key: key, mobile: mobile, date: date, status: status },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemncc() {
    var key = $('#inputIcon').val();
    $.ajax({
        url: "/NhaCungCap/TimNCC",
        type: 'GET',
        data: { key: key},
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemdhncc() {
    var key = $('#madh').val();
    var ncc = $('#nccname').val();
    var ngaylap = $('#ngaylap').val();
    var status = $('#trangthai').val();
    var ngaytt = $('#ngaytt').val();
    $.ajax({
        url: "/DonHangNCC/TimDonHang",
        type: 'GET',
        data: { key: key, ncc: ncc, ngaylap: ngaylap, ngaytt:ngaytt, status: status },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemDSSanPhamKhuyenMai(value) {
    var key = $('#inputIcon1').val();
    var maloai = $('#maloaisearch1').val();
    $.ajax({
        url: "/KhuyenMai/DSSanPhamKhuyenMai",
        type: 'GET',
        data: { key: key, maloai: maloai, makm: value.toString() },
        success: function (result) {
            $('.Ajax-Table-1').html(result);
        },
    });
}

function timkiemDSSanPham(value) {
    var key = $('#inputIcon').val();
    var maloai = $('#maloaisearch').val();
    $.ajax({
        url: "/KhuyenMai/DSSanPham",
        type: 'GET',
        data: { key: key, maloai: maloai, makm: value.toString() },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}


function timkiemajax() {
    var key = $('#inputIcon').val();
    var maloai = $('#maloaisearch').val();
    $.ajax({
        url: "/Admin/TimSP",
        type: 'GET',
        data: { key: key, maloai: maloai },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemkhachhang() {
    var key = $('#tendangnhaps').val();
    var email = $('#emails').val();
    var hoten = $('#hotens').val();
    var phone = $('#sodienthoais').val();
    var quyen = $('#quyens').val();
    $.ajax({
        url: "/Account/TimUser",
        type: 'GET',
        data: { key: key, email: email, hoten:hoten, phone:phone, quyen:quyen },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}


function timkiemloaiSP() {
    var key = $('#inputIcon').val();
    $.ajax({
        url: "/LoaiSP/TimLoaiSP",
        type: 'GET',
        data: { key: key },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemHangSX() {
    var key = $('#inputIcon').val();
    $.ajax({
        url: "/HangSX/TimHangSX",
        type: 'GET',
        data: { key: key },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}

function timkiemKhuyenMai() {
    var key = $('#inputIcon').val();
    var start = $('#startday').val();
    var end = $('#endday').val();
    $.ajax({
        url: "/KhuyenMai/TimKhuyenMai",
        type: 'GET',
        data: { key: key, start: start, end: end },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}
////////////////////////////////////

function kiemgiatritrung(value, Url, id) {
    if (value)
        $.ajax({
            url: Url,
            data: { key: value },
            success: function (result) {
                var s = $(id).parent();
                if (result) {
                    $('#kiemtratrung').html("Ok");
                    s.find("input").css({ "border": "2px solid green" });
                    $('.alert-info').fadeOut(500);
                }
                else {
                    $('#kiemtratrung').html("Đã được sử dụng");
                    s.find("input").css({ "border": "2px solid red" });
                    $('.alert-info').fadeIn(500);
                }

            },
        });
    else {
        var s = $(id).parent();
        $('#kiemtratrung').html("Không được để trống");
        s.find("input").css({ "border": "2px solid red" });
        $('.alert-info').fadeIn(500);
    }
}


//Start SanPhamKhuyenMai///////////////////////////////


function XoaSPKhuyenMai(Url, value1, value2) {
    if (confirm("Bạn có chắc muốn xóa dữ liệu?") == true) {
        $.ajax({
            url: Url,
            type: 'POST',
            data: { makm: value1, masp: value2 },
            success: function (result) {
                $('.Ajax-Table').html(result);
                timkiemDSSanPhamKhuyenMai(value1);
            },
        });
    }


}
//End SanPhamKhuyenMai

//START BINHLUAN
function ThemTraLoi(Url, masp, parent) {
    $.ajax({
        url: Url,
        data: { masp: masp, parent: parent },
        success: function (result) {
            $('#show-dialog-detail').html(result);
            $('#show-dialog-detail').fadeIn(500);
            //$('#alert-info').fadeOut(3000);
        },
    });
}
//END BINH LUAN

//STAR PHANQUYEN
function phanquyens(Url,QuyenName) {
    checkboxes = document.getElementsByName('lstdel');
    var lst = new Array();
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (checkboxes[i].checked) {
            lst.push(checkboxes[i].value);
        }
    }
    if (!QuyenName)
        QuyenName = $('#setquyens').val();
    if (confirm("Dữ liệu quan trọng sắp được thay đổi, bạn có muốn tiếp tục?") == true) {
        $.ajax({
            url: Url,
            type: 'POST',
            data: { lstu: lst, quyen:QuyenName },
            success: function (result) {
                $('.Ajax-Table').html(result);
                $('#alert-info').html("Cập nhật thành công");
                $('#alert-info').fadeIn(1000);
                $('#alert-info').fadeOut(3000);
            },
        });
    }
}
//END PHAN QUYEN

//START UPDATE DON HANG STATUS
function SetStatus(Url) {
    checkboxes = document.getElementsByName('lstdel');
    var lst = new Array();
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (checkboxes[i].checked) {
            lst.push(checkboxes[i].value);
        }
    }
    var tt = $('#setstatus').val();
    if (confirm("Dữ liệu quan trọng sắp được thay đổi, bạn có muốn tiếp tục?") == true) {
        $.ajax({
            url: Url,
            type: 'POST',
            data: { lst: lst, tt: tt },
            success: function (result) {
                $('.Ajax-Table').html(result);
                $('#alert-info').html("Cập nhật thành công");
                $('#alert-info').fadeIn(1000);
                $('#alert-info').fadeOut(3000);
            },
        });
    }
}
//END UPDATE DON HANG STATUS

function thanhtoanB2B(param) {
    $.ajax({
        url: "/HopDong/Xacnhanthanhtoan",
        type: "GET",
        data: { MaHD: param },
        success: function (result) {
            if (result == "success")
                $('.tt_' + param).html("<span class=\"label label-info\">Đã thanh toán</span>");
        }
    });
}
function timkiemhopdongB2B() {
    var key = $('#mahd').val();
    var tensp = $('#tensp').val();
    var loaihd = $('#loaihd').val();
    $.ajax({
        url: "/HopDong/TimHopDongB2B",
        type: 'GET',
        data: { key: key, tensp: tensp, loaihd: loaihd },
        success: function (result) {
            $('.Ajax-Table').html(result);
        },
    });
}