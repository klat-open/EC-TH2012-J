using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EC_TH2012_J.Models;
using EC_TH2012_J.Models.B2B;
using PagedList;
using PagedList.Mvc;

namespace EC_TH2012_J.Controllers.B2B
{
    public class NhaCungCapController : Controller
    {
        //
        // nha cung cap
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SearchByName(string term)
        {
            NhaCungCapModel sp = new NhaCungCapModel();
            IQueryable<NhaCungCap> lst = sp.TimNCC(term);
            var splist = (from p in lst orderby p.TenNCC descending select new { p.MaNCC, p.TenNCC }).Take(5);
            return Json(splist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TimNCC(string key, int? page)
        {
            NhaCungCapModel ncc = new NhaCungCapModel();
            ViewBag.key = key;
            return PhanTrangNCC(ncc.TimNCC(key).Where(m=>!string.IsNullOrEmpty(m.Net_user)), page, null);
        }

        public ActionResult PhanTrangNCC(IQueryable<NhaCungCap> lst, int? page, int? pagesize)
        {
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            return PartialView("NCCPartial", lst.OrderBy(m => m.TenNCC).ToPagedList(pageNumber, pageSize));
        }

      
	}
}