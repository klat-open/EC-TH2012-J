using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using EC_TH2012_J.Models;
using EC_TH2012_J.Models.B2B;


namespace EC_TH2012_J.Controllers.B2B
{
    public class DanhSachDKController : Controller
    {
        // GET: DanhSachDK
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TimDS(string tensp, string tenncc, int? tt, int? page)
        {
            DSDangKyModel DSDK = new DSDangKyModel();
            ViewBag.tensp = tensp;
            ViewBag.tenncc = tenncc;
            ViewBag.tt = tt;
            return PhanTrangDSDK(DSDK.TimDS(tensp,tenncc,tt), page, null);
        }

        [HttpPost]
        public ActionResult KyHopDong(string id)
        {
            SanphamcanmuaModel sp = new SanphamcanmuaModel();
            sp.DeleteSPCM(Convert.ToInt32(id));
            return TimDS(null, null, null, null);

        }

        public ActionResult DeleteDSDK(int id)
        {
            DSDangKyModel ncc = new DSDangKyModel();
            if (ncc.Findbyid(id) == null)
            {
                return HttpNotFound();
            }
            ncc.DeleteDSDK(id);
            return TimDS(null, null,null,null);
        }

        [HttpPost]
        public ActionResult MultibleDel(List<string> lstdel)
        {
            foreach (var item in lstdel)
            {
                DSDangKyModel ncc = new DSDangKyModel();
                ncc.DeleteDSDK(Convert.ToInt32(item));
            }
            return TimDS(null, null, null, null);
        }

        public ActionResult PhanTrangDSDK(IQueryable<DanhsachdangkisanphamNCC> lst, int? page, int? pagesize)
        {
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            return PartialView("DSDKPartial", lst.OrderByDescending(m => m.NgayDK).ToPagedList(pageNumber, pageSize));
        }
    }
}