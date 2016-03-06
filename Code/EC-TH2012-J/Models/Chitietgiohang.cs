using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class Chitietgiohang
    {
        public SanPham sanPham { get; set; }
        public int Soluong { get; set; }
        private double thanhtien;

        public double Thanhtien
        {
            get { return (double)sanPham.GiaTien * Soluong; }
            set { thanhtien = value; }
        }
        public DonHangKH Donhangkh { get; set; }
       
        public void Tinhtien()
        {
            Thanhtien = (double)sanPham.GiaTien * Soluong;
        }
    }
}