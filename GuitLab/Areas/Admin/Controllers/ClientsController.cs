using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Clients;
using GuitLab.Models.ViewModels.Order;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClientsController : Controller
    {
        // GET: Admin/Clients
        public ActionResult Index()
        {
            List<ClientsViewModel> clientsList;

            using (Db db = new Db())
            {
                clientsList = db.Clients
                    .ToArray()
                    .OrderBy(c => c.UserId)
                    .Select(c => new ClientsViewModel(c))
                    .ToList(); ;
            }

            return View(clientsList);
        }

        public ActionResult DeleteClient(int id)
        {
            using (Db db = new Db())
            {
                ClientsDTO dto = db.Clients.Find(id);
                db.Clients.Remove(dto);
                db.SaveChanges();
            }

            TempData["msg"] = "You have deleted a client";
            return RedirectToAction("Index");
        }
    }
}