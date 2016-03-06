using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class Giohang
    {
        private List<Chitietgiohang> Cart;
        public double phiVanChuyen = 0;
        public Giohang()
        {
            Cart = new List<Chitietgiohang>();
        }
        public List<Chitietgiohang> getGiohang()
        {
            return Cart;
        }
        public void addCart(Chitietgiohang a)
        {
            Cart.Add(a);
        }
        public bool removeCart(int index)
        {
            try
            {
                Cart.RemoveAt(index);
                return true;
            }
            catch (Exception e) { return false; }
        }
        /// <summary>
        /// Tính tổng tiền của các sản phẩm trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public double Tinhtongtiensanpham()
        {
            double count = 0;
            foreach(var temp in Cart)
            {
                count += temp.Thanhtien;
            }
            return count;
        }
        public double TinhtongtienCart()
        {
            return Tinhtongtiensanpham() + phiVanChuyen;
        }
        public bool Changequanlity(int index, string soluong)
        {
            try
            {
                Cart[index].Soluong = Int32.Parse(soluong);
                Cart[index].Tinhtien();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public int Tinhtongsoluongtronggio()
        {
            int count = 0;
            foreach (var temp in Cart)
            {
                count += temp.Soluong;
            }
            return count;
        }
    }
}