using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Account;
using GuitLab.Models.ViewModels.Order;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        // GET: Admin/User
        public ActionResult Index()
        {
            List<UserViewModel> usersList;

            using (Db db = new Db())
            {
                usersList = db.Users.ToArray()
                    .OrderBy(c => c.Id)
                    .Select(c => new UserViewModel(c))
                    .ToList();
            }

            return View(usersList);
        }

        public ActionResult DeleteUser(int id)
        {
            using (Db db = new Db())
            {
                UserDTO dto = db.Users.Find(id);
                db.Users.Remove(dto);
                db.SaveChanges();
            }

            TempData["msg"] = "You have deleted a user";
            return RedirectToAction("Index");
        }
    }
}