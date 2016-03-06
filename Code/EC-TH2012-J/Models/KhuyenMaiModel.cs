using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class KhuyenMaiModel
    {
        private Entities db = new Entities();

        internal KhuyenMai FindById(string id)
        {
            return db.KhuyenMais.Find(id);
        }

        internal void EditKhuyenMai(KhuyenMai loai)
        {
            KhuyenMai lsp = db.KhuyenMais.Find(loai.MaKM);
            lsp.TenCT = loai.TenCT;
            lsp.NgayBatDau = loai.NgayBatDau;
            lsp.NgayKetThuc = loai.NgayKetThuc;
            lsp.NoiDung = loai.NoiDung;
            db.Entry(lsp).State = EntityState.Modified;
            db.SaveChanges();
        }

        internal void DeleteKhuyenMai(string id)
        {
            var lst = db.SanPhamKhuyenMais.Where(m => m.MaKM.Equals(id)).ToList();
            foreach(var item in lst)
            {
                DeleteSPKhuyenMai(item.MaKM, item.MaSP);
            }
            KhuyenMai loai = db.KhuyenMais.Find(id);
            db.KhuyenMais.Remove(loai);
            db.SaveChanges();
        }

        internal string ThemKhuyenMai(KhuyenMai loai)
        {
            loai.MaKM = TaoMa();
            loai.AnhCT = loai.MaKM + "1.jpg";
            db.KhuyenMais.Add(loai);
            db.SaveChanges();
            return loai.MaKM;
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
            var temp = db.KhuyenMais.Find(maID);
            if (temp == null)
                return true;
            return false;
        }

        internal IQueryable<KhuyenMai> TimKhuyenMai(string key,DateTime? start, DateTime? end)
        {
            IQueryable<KhuyenMai> lst = db.KhuyenMais;
            if (!string.IsNullOrEmpty(key))
                lst = db.KhuyenMais.Where(u => u.TenCT.Contains(key));
            if (start != null)
                lst = db.KhuyenMais.Where(u => u.NgayBatDau >= start);
            if (end != null)
                lst = db.KhuyenMais.Where(u => u.NgayKetThuc <= end);
            return lst;
        }

        internal bool KiemTraTen(string key)
        {
            var temp = db.KhuyenMais.Where(m => m.TenCT.Equals(key)).ToList();
            if (temp.Count == 0)
                return true;
            return false;
        }

        internal IQueryable<SanPhamKhuyenMai> CTKhuyenMai(string key)
        {
            return db.SanPhamKhuyenMais.Where(m => m.MaKM.Equals(key));
        }



        internal void ThemSPKhuyenMai(SanPhamKhuyenMai item)
        {
            db.SanPhamKhuyenMais.Add(item);
            db.SaveChanges();
            SanPhamModel s = new SanPhamModel();
            s.UpdateGiaBan(item.MaSP);
        }
        

        internal void DelAllSPKM(string p)
        {
            db.SanPhamKhuyenMais.RemoveRange(db.SanPhamKhuyenMais.Where(m => m.MaKM == p));
            db.SaveChanges();
        }

        internal IQueryable<SanPhamKhuyenMai> GetSPKM(string MaKM)
        {
            return db.SanPhamKhuyenMais.Where(m => m.MaKM == MaKM);
        }

        internal IQueryable<SanPham> DSSP(string key, string maloai, string makm)
        {
            var lst = db.SanPhamKhuyenMais.Where(m => m.MaKM == makm).Select(m => m.MaSP);
            var lst1 = db.SanPhams.Where(m => !lst.Contains(m.MaSP));
            if (!string.IsNullOrEmpty(key))
                lst1 = lst1.Where(m => m.TenSP.Contains(key));
            if (!string.IsNullOrEmpty(maloai))
                lst1 = lst1.Where(m => m.LoaiSP.Equals(maloai));
            return lst1;
        }

        internal IQueryable<SanPham> DSSanPhamKhuyenMai(string key, string maloai, string makm)
        {
            var lst = db.SanPhamKhuyenMais.Where(m => m.MaKM == makm).Select(m => m.SanPham);
            if (!string.IsNullOrEmpty(key))
                lst = lst.Where(m => m.TenSP.Contains(key));
            if (!string.IsNullOrEmpty(maloai))
                lst = lst.Where(m => m.LoaiSP.Equals(maloai));
            return lst;
        }

        internal void DeleteSPKhuyenMai(string makm, string masp)
        {
            SanPhamKhuyenMai sp = db.SanPhamKhuyenMais.Where(m=>m.MaSP == masp && m.MaKM == makm).FirstOrDefault();
            db.SanPhamKhuyenMais.Remove(sp);
            db.SaveChanges();
            SanPhamModel s = new SanPhamModel();
            s.UpdateGiaBan(masp);
            
        }
    }
}