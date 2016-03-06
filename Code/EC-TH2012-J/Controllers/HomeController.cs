using EC_TH2012_J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace EC_TH2012_J.Controllers
{
    public class HomeController : Controller
    {
        private static Entities db = new Entities();

        public static List<Thanhviennhom> Ds_Group;
        public ActionResult Index()
        {
            ManagerObiect.getIntance();
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Thongtinnhom()
        {
            if (Ds_Group == null)
            {
                Ds_Group = new List<Thanhviennhom>();
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212293",
                    Hoten = "Nguyễn Ngọc Phúc",
                    LinkFacebook = "https://www.facebook.com/phuc.nguyen.eccentric?fref=pb_friends"
                });
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212080",
                    Hoten = "Huỳnh Phạm Hải Đăng",
                    LinkFacebook = "https://www.facebook.com/wayne.pham.507?fref=pb_friends"
                });
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212276",
                    Hoten = "Nguyễn Thành Nhân",
                    LinkFacebook = "https://www.facebook.com/GanderNguyen?fref=pb_friends"
                });
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212437",
                    Hoten = "Phan Ngọc Triều",
                    LinkFacebook = "https://www.facebook.com/taolibra?fref=pb_friends"
                });
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212502",
                    Hoten = "Nguyễn Văn Ty",
                    LinkFacebook = "https://www.facebook.com/vanty8"
                });
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212526",
                    Hoten = "Nguyễn Trương Vương",
                    LinkFacebook = "https://www.facebook.com/vuongtruong.nguyen?fref=pb_friends"
                });
                Ds_Group.Add(new Thanhviennhom
                {
                    MSSV = "1212535",
                    Hoten = "Vũ Thị Thanh Xuân",
                    LinkFacebook = "https://www.facebook.com/harusame.927?fref=pb_friends"
                });
            }
            return View(Ds_Group);
        }
        
        public ActionResult Cart()
        {
            return View(ManagerObiect.getIntance().giohang);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên,Khách hàng")]
        //Đơn hàng
        public ActionResult Xemdonhang(string maKH)
        {
            List<DonhangKHModel> temp = new List<DonhangKHModel>();
            if (maKH.Length != 0)
            {
                DonhangKHModel dh = new DonhangKHModel();
                temp = dh.Xemdonhang(maKH);
            }
            return View(temp);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên,Khách hàng")]
        public ActionResult Huydonhang(string maDH)
        {
            DonhangKHModel dh = new DonhangKHModel();
            dh.HuyDH(maDH);
            var donhang = dh.Xemdonhang(User.Identity.GetUserId());
            return View(donhang);
        }
        public ActionResult Checkout()
        {
            if (Request.IsAuthenticated)
            {
                DonhangKHModel dh = new DonhangKHModel();
                dh.nguoiMua = dh.Xemttnguoidung(User.Identity.GetUserId());
                Donhangtongquan dhtq = new Donhangtongquan()
                {
                    buyer = dh.nguoiMua.HoTen,
                    seller = dh.nguoiMua.HoTen,
                    phoneNumber = dh.nguoiMua.PhoneNumber,
                    address = dh.nguoiMua.DiaChi
                };
                return View(dhtq);
            }
            else
            {
                return RedirectToAction("Authentication", "Account", new { returnUrl = "/Home/Checkout" });
            }
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên,Khách hàng")]
        [HttpPost]
        public ActionResult Checkout(Donhangtongquan dh)
        {
            if (User.Identity.IsAuthenticated)
            {
                DonhangKHModel dhmodel = new DonhangKHModel();
                dhmodel.Luudonhang(dh, User.Identity.GetUserId(), ManagerObiect.getIntance().giohang);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Checkout", "Home");
            }
        }

        public ActionResult MainMenu()
        {
            MainMenuModel mnmodel = new MainMenuModel();
            var menulist = mnmodel.GetMenuList();
            return PartialView("_MainMenuPartial", menulist);
        }

        public ActionResult SPNoiBat(int? skip)
        {
            SanPhamModel sp = new SanPhamModel();
            int skipnum = (skip ?? 0);
            IQueryable<SanPham> splist = sp.SPHot();
            splist = splist.OrderBy(r => r.MaSP).Skip(skipnum).Take(4);
            if (splist.Any())
                return PartialView("_ProductTabLoadMorePartial", splist);
            else
                return null;
        }

        public ActionResult SPMoiNhap(int? skip)
        {
            SanPhamModel sp = new SanPhamModel();
            int skipnum = (skip ?? 0);
            IQueryable<SanPham> splist = sp.SPMoiNhap();
            splist = splist.OrderBy(r => r.MaSP).Skip(skipnum).Take(4);
            if (splist.Any())
                return PartialView("_ProductTabLoadMorePartial", splist);
            else
                return null;
        }

        public ActionResult SPKhuyenMai(int? skip)
        {
            SanPhamModel sp = new SanPhamModel();
            int skipnum = (skip ?? 0);
            IQueryable<SanPham> splist = sp.SPKhuyenMai();
            splist = splist.OrderBy(r => r.MaSP).Skip(skipnum).Take(4);
            if (splist.Any())
                return PartialView("_ProductTabLoadMorePartial", splist);
            else
                return null;
        }

        public ActionResult SPBanChay()
        {
            SanPhamModel sp = new SanPhamModel();
            IQueryable<SanPham> splist = sp.SPBanChay(7);      
            if (splist.Any())
                return PartialView("_BestSellerPartial", splist.ToList());
            else
                return null;
        }
        public ActionResult SPMoiXem()
        {
            return PartialView("_RecentlyViewPartial", ManagerObiect.getIntance().Laydanhsachsanphammoixem());
        }

    }
}