using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class SanPhamModel
    {
        Entities db = new Entities();
        public IQueryable<SanPham> SearchByName(string term)
        {
            IQueryable<SanPham> lst;
            lst = db.SanPhams.Where(u => u.TenSP.Contains(term));
            return lst ;
        }

        public IQueryable<SanPham> AdvancedSearch(string term, string loai, string hangsx, int? minprice, int? maxprice)
        {
            IQueryable<SanPham> lst = db.SanPhams;
            if(!string.IsNullOrEmpty(term))
                lst = SearchByName(term);
            if(!string.IsNullOrEmpty(loai))
                lst = from p in lst where p.LoaiSP.Equals(loai) select p;
            if (!string.IsNullOrEmpty(hangsx))
                lst = from p in lst where p.HangSX.Equals(hangsx) select p;
            if (minprice != null)
                lst = from p in lst where p.GiaTien >= minprice select p;
            if (maxprice != null)
                lst = from p in lst where p.GiaTien <= maxprice select p;
            return lst;
        }
        public IQueryable<SanPham> SearchByType(string term)
        {
            var splist = (from p in db.SanPhams where p.LoaiSP.Equals(term) select p);
            return splist;
        }



        internal IQueryable<SanPham> SPMoiNhap()
        {
            var splist = db.SanPhams.Where(s => s.isnew == true);
            return splist;
        }

        internal IQueryable<SanPham> SPKhuyenMai()
        {
            var splist = from p in db.SanPhamKhuyenMais
                         orderby p.GiamGia descending
                         where DateTime.Today >= p.KhuyenMai.NgayBatDau && DateTime.Today <= p.KhuyenMai.NgayKetThuc
                         select p.SanPham;
            return splist;
        }

        internal IQueryable<SanPham> SPBanChay(int takenum)
        {
            var s = from p in db.ChiTietDonHangs
                    where p.DonHangKH.TinhTrangDH == 3
                    group p by p.MaSP into gro
                    select new {MaSP = gro.Key,sl = gro.Sum(r => r.SoLuong)};
            var splist = from p in db.SanPhams join ca in s on p.MaSP equals ca.MaSP orderby ca.sl descending select p;
            return splist.Take(takenum);
        }

        internal IQueryable<SanPham> GetAll()
        {
            return db.SanPhams;
        }

        internal SanPham FindById(string id)
        {
            return db.SanPhams.Find(id);
        }

        internal IQueryable<HangSanXuat> GetAllHangSX()
        {
            return db.HangSanXuats;
        }

        internal IQueryable<LoaiSP> GetAllLoaiSP()
        {
            return db.LoaiSPs;
        }

        internal void EditSP(SanPham sanpham)
        {
            //MaSP,TenSP,LoaiSP,HangSX,XuatXu,GiaTien,MoTa,SoLuong,isnew,ishot
            SanPham sp = db.SanPhams.Find(sanpham.MaSP);
            sp.TenSP = sanpham.TenSP;
            sp.LoaiSP = sanpham.LoaiSP;
            sp.HangSX = sanpham.HangSX;
            sp.XuatXu = sanpham.XuatXu;
            sp.GiaGoc = sanpham.GiaGoc;
            sp.GiaTien = tinhgiatien(sp.MaSP, sp.GiaGoc);
            sp.MoTa = sanpham.MoTa;
            sp.SoLuong = sanpham.SoLuong;
            sp.isnew = sanpham.isnew;
            sp.ishot = sanpham.ishot;
            db.Entry(sp).State = EntityState.Modified;
            db.SaveChanges();
        }

        private decimal? tinhgiatien(string masp,decimal? giagoc)
        {
            IQueryable<SanPhamKhuyenMai> s = db.SanPhamKhuyenMais.Where(m => m.MaSP.Equals(masp)).OrderByDescending(m => m.GiamGia);
            if (s.Any())
            {
                return (giagoc * (100-s.First().GiamGia) / 100);
            }
            return giagoc;
        }

        internal void DeleteSP(string id)
        {
            SanPham sanpham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanpham);
            db.SaveChanges();
        }

        internal string ThemSP(SanPham sanpham)
        {
            sanpham.MaSP = TaoMa();
            sanpham.GiaTien = sanpham.GiaGoc;
            sanpham.AnhDaiDien = sanpham.MaSP + "1.jpg";
            sanpham.AnhNen = sanpham.MaSP + "2.jpg";
            sanpham.AnhKhac = sanpham.MaSP + "3.jpg";
            db.SanPhams.Add(sanpham);
            db.SaveChanges();
            return sanpham.MaSP;
        }

        private string TaoMa()
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
            while (!KiemtraID(maID));
            return maID;
        }

        private bool KiemtraID(string maID)
        {
            using (Entities db = new Entities())
            {
                var temp = db.SanPhams.Find(maID);
                if (temp == null)
                    return true;
                return false;
            }
        }
        public SanPham getSanPham(string id)
        {
            var sp = (from p in db.SanPhams where (p.MaSP == id) select p).FirstOrDefault();
            return sp;
        }

        internal void ThemTSKT(ThongSoKyThuat item)
        {
            db.ThongSoKyThuats.Add(item);
            db.SaveChanges();
        }

        internal IQueryable<ThongSoKyThuat> GetTSKT(string masp)
        {
            return db.ThongSoKyThuats.Where(m => m.MaSP == masp);
        }

        internal void DelAllTSKT(string p)
        {
            db.ThongSoKyThuats.RemoveRange(db.ThongSoKyThuats.Where(m => m.MaSP == p));
            db.SaveChanges();
        }

        internal IQueryable<SanPham> SPHot()
        {
            return db.SanPhams.Where(s => s.ishot == true);
        }

        internal void UpdateGiaBan(string p)
        {
            var s = db.SanPhams.Find(p);
            s.GiaTien = tinhgiatien(p, s.GiaGoc);
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();
        }

        internal void UpdateGiaBans(List<SanPham> lst)
        {
            using (var db = new Entities())
            {
                lst.ForEach(m => m.GiaTien = tinhgiatien(m.MaSP, m.GiaGoc));
                db.SaveChanges();
            }
        }

        internal void UpdateSL(string masp,int? sl,bool? loaihd)
        {
            if (sl != null)
            {
                var s = db.SanPhams.Find(masp);
                if (loaihd == true)
                    s.SoLuong += sl;
                else if (loaihd == false)
                    s.SoLuong -= sl;                
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}