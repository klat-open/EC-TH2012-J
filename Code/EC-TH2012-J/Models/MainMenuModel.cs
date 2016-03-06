using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class MainMenuModel
    {
        private Entities db = new Entities();

        public List<Link> GetMenuLink()
        {
            return db.Links.Where(m => m.Group.Equals("MainMenu")).ToList();
        }
        public List<MenuItem> GetMenuList() 
        { 
            List<MenuItem> mnlist = new List<MenuItem>();
            var loaiSPlst = db.LoaiSPs.OrderBy(a => a.MaLoai).Where(a => !a.MaLoai.Equals("NOTTT")).ToList();
            foreach (var item in loaiSPlst)
            {
                MenuItem mnitem = new MenuItem();
                mnitem.LoaiSP = item;
                mnitem.HangSX = this.GetHangSXLst(item.MaLoai);
                mnlist.Add(mnitem);
            }
            return mnlist;
        }

        private List<HangSanXuat> GetHangSXLst(string maloai)
        {
            List<HangSanXuat> hsxlist = (from p in db.SanPhams where p.LoaiSP == maloai select p.HangSanXuat).Distinct().ToList();
            return hsxlist;
        }
    }

    public class MenuItem
    {
        public LoaiSP LoaiSP { get; set; }
        public List<HangSanXuat> HangSX { get; set; }
    }
}

