using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class Shipping
    {
        public string supplier_key { get; set; }
        public string order_id { get; set; }
        public string product_id { get; set; }
        public int product_quantity { get; set; }
        public string product_date { get; set; }
        public string access_token { get; set; }
    }
}