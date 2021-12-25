using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Lab;

namespace GuitLab.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index","Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            List<CategoryViewModel> categoryVmList;

            using (Db db = new Db())
            {
                categoryVmList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryViewModel(x))
                    .ToList();
            }

            return PartialView("_CategoryMenuPartial", categoryVmList);
        }

        public ActionResult Category(string name)
        {
            List<ServicesViewModel> servVMList;

            using (Db db = new Db())
            {
                CategoryDTO categoryDTO = db.Categories
                    .Where(x => x.Slug == name)
                    .FirstOrDefault();

                int catId = categoryDTO.Id;

                servVMList = db.Services
                    .ToArray()
                    .Where(x => x.CategoryId == catId)
                    .Select(x => new ServicesViewModel(x))
                    .ToList();

                var servCat = db.Services
                    .Where(x => x.CategoryId == catId)
                    .FirstOrDefault();

                if (servCat == null)
                {
                    var catName = db.Categories
                        .Where(x => x.Slug == name)
                        .Select(x => x.Name)
                        .FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = servCat.CategoryName;
                }

            }

            return View(servVMList);
        }

        [ActionName("service-details")]
        public ActionResult ServiceDetails(string name)
        {
            ServicesDTO dto;
            ServicesViewModel model;

            int id = 0;
            using (Db db = new Db())
            {
                if (!db.Services.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                dto = db.Services
                    .Where(x => x.Slug == name)
                    .FirstOrDefault();

                id = dto.Id;

                model = new ServicesViewModel(dto);
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            return View("ServiceDetails", model);
        }
    }
}