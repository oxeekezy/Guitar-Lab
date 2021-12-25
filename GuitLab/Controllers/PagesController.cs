using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GuitLab.Models.Data;
using System.Web.Mvc;
using GuitLab.Models.ViewModels.Pages;
using Microsoft.Ajax.Utilities;

namespace GuitLab.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Index(string page = "")
        {
            if (page == "")
            {
                page = "home";
            }

            PageViewModel model;
            PagesDTO dto;

            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new {page = ""});
                }
            }

            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            ViewBag.PageTitle = dto.Title;
            ViewBag.SideBar = "Yes";

            model = new PageViewModel(dto);
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageViewModel> pageVMList;

            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Where(x => x.Slug != "home")
                    .Select(x => new PageViewModel(x))
                    .ToList();
            }

            return PartialView("_PagesMenuPartial", pageVMList);
        }

    }
}