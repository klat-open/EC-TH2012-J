using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class Classsupport
    {
    }
    public class Hopdong
    {
        public string order_id { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string product_quantity { get; set; }
        public DateTime product_date { get; set; }
        public string supplier_key { get; set; }
    }
    public class Token
    {
        public string access_token { get; set; }
    }
}