using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EC_TH2012_J.Models;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using System.Web.Http.Cors;

namespace EC_TH2012_J.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrdersController : ApiController
    {
        private Entities db = new Entities();

        // GET api/Orders
        //[HttpGet]
        //[Route("api/orders")]
        //[Authorize]
        //public HttpResponseMessage GetHopDongNCCs(string supplier_key)
        //{
        //    try
        //    {
        //            var maNcc = from p in db.Oauths where p.Consumer_key == supplier_key select new { MaNCC = p.MaNCC };
        //            string ma = maNcc.ToList()[0].MaNCC;
        //            var listofHD = (from p in db.HopDongNCCs where p.MaNCC == ma select new { order_id = p.MaHD, product_id = p.MaSP, product_name = p.SanPham.TenSP, product_quantity = p.SanPham.SoLuong });
        //            return Request.CreateResponse(HttpStatusCode.OK, listofHD.ToList());
                
        //    }
        //    catch (Exception e) { return Request.CreateResponse(HttpStatusCode.NotFound); }
        //}

        [HttpGet]
        [Route("api/orders/{supplier_key}")]
        [IdentityAuthentication(true)] // enable basic for this action
        public HttpResponseMessage GetHopDongNCC(string supplier_key)
        {
            try
            {
                    var maNcc = from p in db.Oauths where p.Consumer_key == supplier_key select new { MaNCC = p.MaNCC };
                    string ma = maNcc.ToList()[0].MaNCC;
                    var listofHD = (from p in db.HopDongNCCs where p.MaNCC == ma select new { order_id = p.MaHD, product_id = p.MaSP, product_name = p.SanPham.TenSP, product_quantity = p.SanPham.SoLuong });
                    return Request.CreateResponse(HttpStatusCode.OK, listofHD.ToList());

            }
            catch (Exception e) { return Request.CreateResponse(HttpStatusCode.NotFound); }
        }

        
        [HttpPost]
        [Route("api/orders/start_shipping")]
        [IdentityAuthentication(true)]
        public HttpResponseMessage Xacnhangiaohang([FromBody]Shipping param)
        {
            try
            {
                var maNcc = from p in db.Oauths where p.Consumer_key == param.supplier_key select new { MaNCC = p.MaNCC };
                string MaNCC = maNcc.ToList()[0].MaNCC;
                HopDongNCC hopdong = db.HopDongNCCs.Where(m => m.MaHD == param.order_id & m.MaNCC == MaNCC & m.MaSP == param.product_id).FirstOrDefault();
                if(hopdong == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,"Không tìm thấy dữ liệu");
                }
                hopdong.SLCungCap = param.product_quantity;
                hopdong.TGGiaoHang = DateTime.Parse(param.product_date);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
    }
}