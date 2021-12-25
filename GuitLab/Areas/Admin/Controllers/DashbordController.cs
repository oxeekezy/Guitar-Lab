using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashbordController : Controller
    {
        // GET: Admin/Dashbord
        public ActionResult Index()
        {
            return View();
        }
    }
}