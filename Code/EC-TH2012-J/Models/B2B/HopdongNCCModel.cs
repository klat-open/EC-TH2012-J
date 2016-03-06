using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class HopdongNCCModel
    {
        public List<NhaCungCap> getDsNhaCC()
        {
            using(Entities db = new Entities())
            {
                var temp = (from p in db.NhaCungCaps select p).ToList();
                return temp;
            }
        }
        public List<DropdownSanpham> getDsSanPham()
        {
            using (Entities db = new Entities())
            {
                var temp = (from p in db.SanPhams select new DropdownSanpham() { ID = p.MaSP, TenSP = p.TenSP }).ToList();
                return temp;
            }
        }
        public string ThemmoiHopDongNCC(HopDongNCC a)
        {
            using(Entities db = new Entities())
            {
                try
                {
                    a.MaHD = TaoMa(db);
                    a.IsBuy = false;
                    db.HopDongNCCs.Add(a);
                    db.SaveChanges();
                    return a.MaHD;
                }
                catch(Exception e)
                {
                    return "";
                }
            }
        }
        private string TaoMa(Entities db)
        {
            string maID;
            Random rand = new Random();
            do
            {
                maID = "";
                for (int i = 0; i < 5; i++)
                {
                    maID += rand.Next(9);
                }
            }
            while (!KiemtraID(maID,db));
            return maID;
        }

        private bool KiemtraID(string maID,Entities db)
        {
            var temp = db.HopDongNCCs.Find(maID);
            if (temp == null)
                return true;
            return false;
        }
        public List<MaHD> getMaHD(string IDdoitac)
        {
            using(Entities db = new Entities())
            {
                var ds = (from p in db.HopDongNCCs where p.MaNCC == IDdoitac select new MaHD() { Mahd = p.MaHD }).ToList();
                return ds;
            }
        }
        public string GetMaSP(string MaHD)
        {
            using(Entities db = new Entities())
            {
                var temp = (from p in db.HopDongNCCs where p.MaHD == MaHD select new { Masp = p.MaSP }).FirstOrDefault();
                return temp.Masp;
            }
        }
        public bool SetTTThanhtoan(string MaHD,bool value)
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var temp = (from p in db.HopDongNCCs where p.MaHD == MaHD select p).FirstOrDefault();
                    temp.TTThanhToan = value;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public bool SetTTGiaohang(string MaHD, bool value)
        {
            using (Entities db = new Entities())
            {
                try
                {
                    var temp = (from p in db.HopDongNCCs where p.MaHD == MaHD select p).FirstOrDefault();
                    temp.TinhTrang = value;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public void SetXacnhangiaohang(string MaHD)
        {
            SetTTThanhtoan(MaHD, false);
            SetTTGiaohang(MaHD, false);
        }

    }
    public class DropdownSanpham
    {
        public string  ID { get; set; }
        public string TenSP { get; set; }
    }
    public class MaHD
    {
        public string Mahd { get; set; }
    }
}