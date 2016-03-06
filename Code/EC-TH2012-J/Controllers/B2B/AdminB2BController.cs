using EC_TH2012_J.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace EC_TH2012_J.Controllers
{
    public class AdminB2BController : Controller
    {
        // GET: AdminB2B
        public ActionResult Taohopdong()
        {
            HopdongNCCModel Ncc = new HopdongNCCModel();
            ViewBag.TenNCC = new SelectList(Ncc.getDsNhaCC(), "MaNCC", "TenNCC");
            ViewBag.MaSP = new SelectList(Ncc.getDsSanPham(), "ID", "TenSP");
            return View();
        }
        [HttpPost]
        public ActionResult Taohopdong(HopDongNCC a)
        {
            HopdongNCCModel Ncc = new HopdongNCCModel();
            if (ModelState.IsValid)
            {
                string MaHD;
                if((MaHD = Ncc.ThemmoiHopDongNCC(a)) != "")
                {
                    ConfigAPI a1 = new ConfigAPI();
                    a1.MaNCC = a.MaNCC;
                    return View("ConfigAPI", a1);
                }
            }
            
            ViewBag.TenNCC = new SelectList(Ncc.getDsNhaCC(), "MaNCC", "TenNCC");
            ViewBag.MaSP = new SelectList(Ncc.getDsSanPham(), "ID", "TenSP");
            return View(a);
        }
        // nhan call back trả về
        public ActionResult NhanCallback(string verifier_token, string request_token)
        {
            return RedirectToAction("GetTokenRequest", new { verifier_token = verifier_token, request_token = request_token });
        }
        public ActionResult ConfigAPI(string MaNCC)
        {
            ConfigAPI a1 = new ConfigAPI();
            a1.MaNCC = MaNCC;
            return View(a1);
        }
        [HttpPost]
        public ActionResult ConfigAPI(ConfigAPI a)
        {
            ConfigAPIModel model = new ConfigAPIModel();
            if(model.ThemmoiConfig(a))
            {
                return RedirectToAction("GetNhaDoiTac");
            }
            return View(a);
        }
        // luu consumer_key
        public JsonResult Saveconsumer_key(string consumer_key)
        {
            ManagerObiect.consumer_key = consumer_key;
            return Json("success");
        }
        public ActionResult GetTokenRequest(string verifier_token, string request_token)
        {
            if (verifier_token != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                    var model = new
                    {
                        consumer_key = ManagerObiect.consumer_key,
                        verifier_token = verifier_token,
                        request_token = request_token
                    };
                    HttpResponseMessage response = client.PostAsJsonAsync(
                        ManagerObiect.configAPI.LinkAccessToken,
                        model
                    ).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var value = response.Content.ReadAsAsync<Token>().Result;
                        ManagerObiect.Token = value.access_token;
                    }
                    else
                        ViewBag.thongbao = response.StatusCode.ToString();
                    return Redirect(ManagerObiect.reDirectUrl);
                }
            }
            return RedirectToAction("","");
        }
        //xu li get orders
        [HttpPost]
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult GetOrders(string user, string pass, string supplier_key)
        {
            if(ManagerObiect.DoitacID != null)
            {
                using(HttpClient client = new HttpClient())
                {
                    string authInfo = user + ":" + pass;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);

                    //client.BaseAddress = new Uri(ManagerObiect.GetBaseUrl(ManagerObiect.configAPI.LinkAccessToken));

                    HttpResponseMessage response = client.GetAsync(ManagerObiect.configAPI.LinkKiemTraLuongTon + supplier_key).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string code = response.StatusCode.ToString();
                        // Parse the response body. Blocking!
                        var data = response.Content.ReadAsAsync<List<Hopdong>>().Result;
                        return View(data);
                    }
                }
            }
            return Content("Error");
        }
        [HttpPost]
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult GetOrdersOauth(string access_token,string supplier_key)
        {
            if (ManagerObiect.DoitacID != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", access_token);

                    HttpResponseMessage response = client.GetAsync(ManagerObiect.configAPI.LinkKiemTraLuongTon + supplier_key).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string code = response.StatusCode.ToString();
                        // Parse the response body. Blocking!
                        var data = response.Content.ReadAsAsync<List<Hopdong>>().Result;
                        return View("GetOrders", data);
                    }
                }
            }
            ManagerObiect.Token = "";
            return Content("Error");
        }
        public ActionResult Laysoluongton(string doiacID)
        {
            ManagerObiect.DoitacID = doiacID;
            ManagerObiect.configAPI = new ConfigAPIModel().getConfig(doiacID);
            if (ManagerObiect.configAPI == null)
            {
                return RedirectToAction("ConfigAPI", new { MaNCC = doiacID });
            }
            ViewBag.doitac = doiacID;
            return View();
        }
        public ActionResult GetAccesstoken(string redirectUrl)
        {
            if(ManagerObiect.configAPI != null)
            {
                ManagerObiect.reDirectUrl = redirectUrl;
                ViewBag.action = ManagerObiect.configAPI.LinkRequesrToken;
                return View();
            }
            return RedirectToAction("", "");
        }
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult GetDanhsachNhaDoiTac()
        {
            return View();
        }
        // get danh sach các Nhà đối tác.
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult GetNhaDoiTac()
        {
            return View();
        }
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult TimDoiTac(string key, int? page)
        {
            DoitacModel model = new DoitacModel();
            ViewBag.key = key;
            return PhanTrangDoitac(model.LayDoitac(), page, null);
        }
        [AuthLog(Roles = "Quản trị viên,Nhân viên")]
        public ActionResult PhanTrangDoitac(List<NhaCungCap> lst, int? page, int? pagesize)
        {
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            return PartialView("DoitacPartial", lst.OrderBy(m => m.TenNCC).ToPagedList(pageNumber, pageSize));
        }
        // Xác nhận giao hàng
        public ActionResult XemXacNhanGiaoHang(string doiTacID)
        {
            ManagerObiect.DoitacID = doiTacID;
            ManagerObiect.configAPI = new ConfigAPIModel().getConfig(doiTacID);
            if (ManagerObiect.configAPI == null)
            {
                return RedirectToAction("ConfigAPI", new { MaNCC = doiTacID });
            }
            HopdongNCCModel model = new HopdongNCCModel();
            ViewBag.doitac = doiTacID;
            ViewBag.HD = new SelectList(model.getMaHD(doiTacID), "Mahd", "Mahd");
            return View();
        }
        //
        public ActionResult BasicParitial()
        {
            return View();
        }
        public ActionResult BasicOauthParitial()
        {
            return View();
        }
        // 
        [HttpPost]
        public ActionResult XemXacNhanGiaoHang(Hopdong a, string access_token, string username, string password, string doitac)
        {
            NhaCungCapModel modelNCC = new NhaCungCapModel();
            HopdongNCCModel modelhd = new HopdongNCCModel();
            ViewBag.doitac = doitac;
            ViewBag.HD = new SelectList(modelhd.getMaHD(doitac), "Mahd", "Mahd");
            if(!modelNCC.Checkthanhtoan(a.order_id))
            {
                ModelState.AddModelError("", "Hợp đồng này lúc trước chưa thanh toán");
                return View(a);
            }
            
            if (access_token == "" || access_token == null)
            {
                if ((username != "" && password != "") && (username != null && password!=null))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json")
                        );
                        string authInfo = username + ":" + password;
                        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                        var model = new
                        {
                            supplier_key = a.supplier_key,
                            order_id = a.order_id,
                            product_id = a.product_id,
                            product_quantity = a.product_quantity,
                            product_date = a.product_date
                        };
                        HttpResponseMessage response = client.PostAsJsonAsync(
                            ManagerObiect.configAPI.LinkXacNhanGiaoHang,
                            model
                        ).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ModelState.AddModelError("", "Đã ghi nhận thành công");
                            modelhd.SetXacnhangiaohang(a.order_id);
                        }
                        else
                        {
                            ViewBag.thongbao = "Thất bại " + response.Content.ReadAsStringAsync().Result;
                            ModelState.AddModelError("", ViewBag.thongbao);
                        }
                    }
                }
                else
                {
                    ViewBag.thongbao = "Thông tin nhập không chính xác";
                    return View(a);
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", access_token);
                    var model = new
                    {
                        supplier_key = a.supplier_key,
                        order_id = a.order_id,
                        product_id = a.product_id,
                        product_quantity = a.product_quantity,
                        product_date = a.product_date
                    };
                    HttpResponseMessage response = client.PostAsJsonAsync(
                        ManagerObiect.configAPI.LinkXacNhanGiaoHang,
                        model
                    ).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("", "Đã ghi nhận thành công");
                        modelhd.SetXacnhangiaohang(a.order_id);
                    }
                    else
                    {
                        ViewBag.thongbao = "Thất bại " + response.Content.ReadAsStringAsync().Result;
                        ModelState.AddModelError("", ViewBag.thongbao);
                    }
                }
            }
            return View(a);
        }
        public JsonResult GetMaspFromMahd(string MaHD)
        {
            HopdongNCCModel model = new HopdongNCCModel();
            string masp = model.GetMaSP(MaHD);
            return Json(masp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Theodoithanhtoan()
        {
            return View();
        }
    }
}