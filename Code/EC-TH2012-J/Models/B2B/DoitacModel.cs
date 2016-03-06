using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class DoitacModel
    {
        public List<NhaCungCap> LayDoitac()
        {
            using(Entities db = new Entities())
            {
                var ds = (from p in db.NhaCungCaps where p.Net_user == null select p).ToList();
                //var ds = (from p in db.NhaCungCaps select p).ToList();
                return ds;
            }
        }
    }
}