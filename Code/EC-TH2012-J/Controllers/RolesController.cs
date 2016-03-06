using EC_TH2012_J.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EC_TH2012_J.Controllers
{
    [AuthLog(Roles = "Quản trị viên")]
    public class RolesController : Controller
    {
        ApplicationDbContext context;

        public RolesController()
        {
            context = new ApplicationDbContext(); 
        }

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        public ActionResult Rolelist()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);
        }

        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
	}
}