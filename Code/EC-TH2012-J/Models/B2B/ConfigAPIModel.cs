using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class ConfigAPIModel
    {
        public bool ThemmoiConfig(ConfigAPI a)
        {
            using(Entities db = new Entities())
            {
                try
                {
                    if (kiemtratontai(a.MaNCC, db))
                    {
                        db.Entry(a).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.ConfigAPIs.Add(a);
                        db.SaveChanges();
                    }
                    return true;
                }
                catch (Exception e) { return false; }
            }
        }
        private bool kiemtratontai(string NCC,Entities db)
        {
            var config = (from p in db.ConfigAPIs where p.MaNCC == NCC select new { p.MaNCC }).ToList();
            if (config.Count == 0)
                return false;
            return true;
        }
        public ConfigAPI getConfig(string MaNCC)
        {
            using(Entities db = new Entities())
            {
                var config = (from p in db.ConfigAPIs where p.MaNCC == MaNCC select p).FirstOrDefault();
                return config;
            }
        }
    }
}