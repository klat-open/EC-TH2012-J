using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class CategoryModel
    {
        private Entities db;
        public CategoryModel()
        {
            db = new Entities();
        }
        public IQueryable<LoaiSP> GetCategory()
        {
            IQueryable<LoaiSP> lst = db.LoaiSPs;
            return lst;
        }

        internal LoaiSP FindById(string id)
        {
            return db.LoaiSPs.Find(id);
        }

        internal void EditLoaiSP(LoaiSP loai)
        {
            LoaiSP lsp = db.LoaiSPs.Find(loai.MaLoai);
            lsp.TenLoai = loai.TenLoai;
            db.Entry(lsp).State = EntityState.Modified;
            db.SaveChanges();
        }

        internal void DeleteLoaiSP(string id)
        {
            LoaiSP loai = db.LoaiSPs.Find(id);
            db.LoaiSPs.Remove(loai);
            db.SaveChanges();
        }

        internal string ThemLoaiSP(LoaiSP loai)
        {
            loai.MaLoai = TaoMa();
            db.LoaiSPs.Add(loai);
            db.SaveChanges();
            return loai.MaLoai;
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
            var temp = db.LoaiSPs.Find(maID);
            if (temp == null)
                return true;
            return false;
        }

        internal IQueryable<LoaiSP> SearchByName(string key)
        {
            if (string.IsNullOrEmpty(key))
                return db.LoaiSPs;
            return db.LoaiSPs.Where(u => u.TenLoai.Contains(key));
        }


        internal bool KiemTraTen(string p)
        {
            var temp = db.LoaiSPs.Where(m=>m.TenLoai.Equals(p)).ToList();
            if (temp.Count == 0)
                return true;
            return false;
        }
    }
}