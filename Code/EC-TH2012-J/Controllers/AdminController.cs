using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC_TH2012_J.Models;
using System.Net;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.IO;


namespace EC_TH2012_J.Controllers
{
    public class AdminController : Controller
    {
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult SanPham()
        {
            SanPhamModel spm = new SanPhamModel();
            ViewBag.HangSX = new SelectList(spm.GetAllHangSX(), "HangSX", "TenHang");
            ViewBag.LoaiSP = new SelectList(spm.GetAllLoaiSP(), "MaLoai", "TenLoai");
            return View();
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult EditSP(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPhamModel spm = new SanPhamModel();
            SanPham sp = spm.FindById(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            ViewBag.HangSX = new SelectList(spm.GetAllHangSX().ToList(), "HangSX", "TenHang", sp.HangSX);
            ViewBag.LoaiSP = new SelectList(spm.GetAllLoaiSP().ToList(), "MaLoai", "TenLoai", sp.LoaiSP);
            return View(sp);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSP([Bind(Include = "MaSP,TenSP,LoaiSP,HangSX,XuatXu,GiaGoc,MoTa,SoLuong,isnew,ishot")] SanPham sanpham, HttpPostedFileBase ad, HttpPostedFileBase an, HttpPostedFileBase ak)
        {
            SanPhamModel spm = new SanPhamModel();
            if (ModelState.IsValid)
            {
                spm.EditSP(sanpham);
                UploadAnh(ad,sanpham.MaSP + "1");
                UploadAnh(an, sanpham.MaSP + "2");
                UploadAnh(ak, sanpham.MaSP + "3");
                return RedirectToAction("SanPham");
            }
            ViewBag.HangSX = new SelectList(spm.GetAllHangSX(), "HangSX", "TenHang", sanpham.HangSX);
            ViewBag.LoaiSP = new SelectList(spm.GetAllLoaiSP(), "MaLoai", "TenLoai", sanpham.LoaiSP);            
            return View(sanpham);
        }

        [AuthLog(Roles = "Quản trị viên")]
        public ActionResult DeleteSP(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPhamModel spm = new SanPhamModel();
            DeleteAnh(spm.FindById(id).AnhDaiDien);
            DeleteAnh(spm.FindById(id).AnhNen);
            DeleteAnh(spm.FindById(id).AnhKhac);
            spm.DeleteSP(id);
            return TimSP(null,null,null);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public bool UploadAnh(HttpPostedFileBase file,string tenfile)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var name = Path.GetExtension(file.FileName);
                // extract only the filename
                if (!Path.GetExtension(file.FileName).Equals(".jpg"))
                {
                    return false;
                }
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/images/products"), tenfile + ".jpg");
                file.SaveAs(path);
                return true;
            }
            // redirect back to the index action to show the form once again
            return false;
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSP([Bind(Include = "TenSP,LoaiSP,HangSX,XuatXu,GiaGoc,MoTa,SoLuong,isnew,ishot")] SanPham sanpham, HttpPostedFileBase ad, HttpPostedFileBase an, HttpPostedFileBase ak)
        {
            SanPhamModel spm = new SanPhamModel();
            if (ModelState.IsValid)
            {
                string masp = spm.ThemSP(sanpham);
                UploadAnh(ad, masp + "1");
                UploadAnh(an, masp + "2");
                UploadAnh(ak, masp + "3");
                ThongSoKyThuat ts = new ThongSoKyThuat();
                ts.MaSP = masp;
                List<ThongSoKyThuat> lst = new List<ThongSoKyThuat>();
                lst.Add(ts);
                return View("ThemThongSoKT", lst);
            }
            ViewBag.HangSX = new SelectList(spm.GetAllHangSX(), "HangSX", "TenHang", sanpham.HangSX);
            ViewBag.LoaiSP = new SelectList(spm.GetAllLoaiSP(), "MaLoai", "TenLoai", sanpham.LoaiSP);
            return View("SanPham",sanpham);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult SPDetail(string id)
        {
            SanPhamModel sp = new SanPhamModel();
            return PartialView("SPDetail", sp.FindById(id));
        }

        [AuthLog(Roles = "Quản trị viên")]
        [HttpPost]
        public ActionResult MultibleDel(List<string> lstdel)
        {
            if (lstdel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foreach (var item in lstdel)
            {
                SanPhamModel spm = new SanPhamModel();
                DeleteAnh(spm.FindById(item).AnhDaiDien);
                DeleteAnh(spm.FindById(item).AnhNen);
                DeleteAnh(spm.FindById(item).AnhKhac);
                spm.DeleteSP(item);
            } 
            return TimSP(null,null,null);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        [HttpPost]
        public ActionResult ThemThongSoKT(List<ThongSoKyThuat> lstkt) 
        {
            if(lstkt.Count == 0)
            {
                return RedirectToAction("SanPham");
            }
            SanPhamModel spm = new SanPhamModel();
            foreach (var item in lstkt)
            {
                if (!string.IsNullOrEmpty(item.ThuocTinh))
                    spm.ThemTSKT(item);
            }
            return RedirectToAction("SanPham"); 
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        [HttpPost]
        public ActionResult SuaThongSoKT(List<ThongSoKyThuat> lstkt)
        {
            if (lstkt.Count == 0)
            {
                return RedirectToAction("SanPham");
            }
            SanPhamModel spm = new SanPhamModel();
            spm.DelAllTSKT(lstkt[0].MaSP);
            foreach (var item in lstkt)
            {
                if (!string.IsNullOrEmpty(item.ThuocTinh))
                    spm.ThemTSKT(item);
            }
            return RedirectToAction("SanPham");
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult SuaThongSoKT(string masp)
        {
            SanPhamModel spm = new SanPhamModel();
            if (spm.GetTSKT(masp).ToList().Count == 0)
            {
                ThongSoKyThuat ts = new ThongSoKyThuat();
                ts.MaSP = masp;
                List<ThongSoKyThuat> lst = new List<ThongSoKyThuat>();
                lst.Add(ts);
                return View("ThemThongSoKT", lst);
            }
            return View("SuaThongSoKT",spm.GetTSKT(masp).ToList());
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult TimSP(string key,string maloai,int? page)
        {
            SanPhamModel spm = new SanPhamModel();
            ViewBag.key = key;
            ViewBag.maloai = maloai;
            return PhanTrangSP(spm.AdvancedSearch(key, maloai, null, null, null),page,null);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult PhanTrangSP(IQueryable<SanPham> lst,int? page, int? pagesize)
        {
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            return PartialView("SanPhamPartial", lst.OrderBy(m => m.MaSP).ToPagedList(pageNumber, pageSize));
        }

        [AuthLog(Roles = "Quản trị viên")]
        public bool DeleteAnh(string filename)
        {
            string fullPath = Request.MapPath("~/images/products/" + filename);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            return false;
        }

    }
}