using EC_TH2012_J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC_TH2012_J.Controllers.B2B
{
    public class AuctionController : Controller
    {
        // GET: Auction
        [AuthLog(Roles = "Quản trị viên,Nhân viên,Nhà cung cấp")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Danhsachcanmua()
        {
            SanphamcanmuaModel sp = new SanphamcanmuaModel();
            var temp = sp.getDS(0, 5);
            return View(temp);        
        }
        public ActionResult ProductFilterB2B()
        {
            CategoryModel cat = new CategoryModel();
            return View(cat.GetCategory());
        }
        public ActionResult ChitietAuction(Sanphamcanmua a)
        {
            SanphamcanmuaModel sp = new SanphamcanmuaModel();
            a.SanPham = sp.getSP(a.MaSP);
            return View(a);
        }
        public ActionResult ChitietSanphamAuction(int spcm)
        {
            SanphamcanmuaModel spmodel = new SanphamcanmuaModel();
            Sanphamcanmua temp = spmodel.getSanphamcanmua(spcm);
            temp.SanPham = spmodel.getSP(temp.MaSP);
            return View(temp);
        }
        public ActionResult ModalRegister(int ID)
        {
            DangkisanphamModel model = new DangkisanphamModel();
            var temp = model.createDangKiSanPham(User.Identity.Name,ID);
            return View(temp);
        }
        [HttpPost]
        public ActionResult RegisterProduct(DangkisanphamModel a)
        {
            if(new DangkisanphamModel().ThemDanhKi(a))
            {
                Session["TB"] = true;
            }
            else
            {
                Session["TB"] = false;
            }

            return RedirectToAction("ChitietSanphamAuction", new { spcm = a.MaSPCM });
        }
        public ActionResult DanhsachNCCDK(int IDSPCM)
        {
            var ds = new DangkisanphamModel().getDSDKNCC(IDSPCM);
            return View(ds);
        }
        public ActionResult Hopdong()
        {
            return View();
        }
    }
}