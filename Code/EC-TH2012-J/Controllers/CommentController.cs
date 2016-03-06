using EC_TH2012_J.Models;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace EC_TH2012_J.Controllers
{
    public class CommentController : Controller
    {
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult Index()
        {
            return View();
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult TimBinhLuan(string key, DateTime? date, string status, int? page)
        {
            CommentModel spm = new CommentModel();
            ViewBag.key = key;
            ViewBag.date = date;
            ViewBag.status = status;
            return PhanTrangBL(spm.TimBinhLuan(key, date, status), page, null);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult PhanTrangBL(IQueryable<BinhLuan> lst, int? page, int? pagesize)
        {
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            return PartialView("BinhLuanPartial", lst.OrderByDescending(m => m.NgayDang).ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        // GET: Comment
        //Tai danh sach binh luan
        public ActionResult LoadComment(string masp, int? page)
        {
            CommentModel cm = new CommentModel();
            //cai dat phan trang
            //So san pham tren 1 trang
            int pageSize = 10;
            //So trang
            int pageNumber = (page ?? 1);
            ViewBag.masp = masp;
            return PartialView("_CommentListPartial", cm.FindByMaSP(masp).OrderByDescending(m => m.NgayDang).ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult ChilComment(int mabl)
        {
            CommentModel cm = new CommentModel();
            return PartialView("_ChilComment",cm.FindChild(mabl));
        }

        [AllowAnonymous]
        //Them binh luan moi sau khi nhan nut gui binh luan se chay ham nay
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(BinhLuan Comment)
        {
            Comment.NgayDang = DateTime.Now;
            Comment.MaKH = User.Identity.GetUserId();
            Comment.DaTraLoi = "C";
            CommentModel cm = new CommentModel();
            cm.AddComment(Comment);
            return RedirectToAction("LoadComment", new { masp = Comment.MaSP });
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRepl(BinhLuan Comment)
        {
            Comment.NgayDang = DateTime.Now;
            Comment.MaKH = User.Identity.GetUserId();
            CommentModel cm = new CommentModel();
            cm.AddComment(Comment);
            cm.UpdateComment(Comment);
            return RedirectToAction("TimBinhLuan");
        }

        [AllowAnonymous]
        //Hien thi form de binh luan co tham so la masp va makh
        public ActionResult AddComment(string masp)
        {
            ViewBag.masp = masp;

            string userid = User.Identity.GetUserId();
            if (userid != null)
            {
                UserModel us = new UserModel();
                var user = us.FindById(userid);
                ViewBag.Name = user.UserName;
            }
            return PartialView("_CommentFormPartial");
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult AddRepl(string masp, int parent)
        {
            BinhLuan bl = new BinhLuan();
            bl.MaSP = masp;
            bl.Parent = parent;
            return View("RepComment", bl);
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult DeleteBinhLuan(int id)
        {
            CommentModel cm = new CommentModel();
            cm.DeleteBinhLuan(id);
            return RedirectToAction("TimBinhLuan");
        }

        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        [HttpPost]
        public ActionResult MultibleDel(List<int> lstdel)
        {
            foreach (var item in lstdel)
            {
                CommentModel spm = new CommentModel();
                spm.DeleteBinhLuan(item);
            }
            return RedirectToAction("TimBinhLuan");
        }

    }
}