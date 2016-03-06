using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC_TH2012_J.Models
{
    public class GiaoDienModel
    {
        private Entities db = new Entities();

        internal IQueryable<GiaoDien> GetDD()
        {
            return db.GiaoDiens;
        }

        internal IQueryable<Link> GetSlideShow()
        {
            return db.Links.Where(m => m.Group.Contains("SlideShow"));
        }
    }
}