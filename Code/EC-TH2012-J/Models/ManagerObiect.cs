using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class ManagerObiect
    {
        private static ManagerObiect manager;
        public Giohang giohang = new Giohang();
        public static string Token ="";
        public static string DoitacID;
        private List<SanPham> sanPhamMoiXem = new List<SanPham>();
        public static ConfigAPI configAPI = null;
        public static string consumer_key;
        public static string reDirectUrl;
        public string userName { get; set; }
        public static ManagerObiect getIntance()
        {
            if(manager == null)
            {
                manager = new ManagerObiect();
            }
            return manager;
        }
        public void saveCarttoCookie(HttpResponseBase reponse,Giohang gh)
        {

        }
        public void LoadCartfromCookie(HttpRequestBase request)
        {
            HttpCookie cookie = request.Cookies["Cart"];
            if(cookie == null)
            {
                giohang = new Giohang();
                return;
            }
            

        }
        public void SaveTrackingLog(Trackingaction a)
        {
            using (Entities db = new Entities())
            {
                db.Trackingactions.Add(a);
                db.SaveChanges();
            }
        }
        public void Themsanphammoixem(SanPham a)
        {
            int count = sanPhamMoiXem.Count(m => m.MaSP == a.MaSP);
            if (count == 0)
                sanPhamMoiXem.Add(a);
        }
        public List<SanPham> Laydanhsachsanphammoixem()
        {
            return sanPhamMoiXem;
        }
        public static string GetBaseUrl(string url)
        {
            url = url.Replace("//","*");
            string []ds = url.Split('/');
            url = ds[0].Replace("*","//");
            return url;
        }
    }
}