using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageViewModel> pagesList;

            using (Db db = new Db()) 
            {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageViewModel(x)).ToList();
            }
            
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage() 
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db()) 
            {
                string slug;

                PagesDTO dto = new PagesDTO();

                dto.Title = model.Title.ToUpper();

                if (string.IsNullOrWhiteSpace(model.Slug))
                    slug = model.Title.Replace(" ", "-").ToLower();
                else
                    slug = model.Slug.Replace(" ", "-").ToLower();

                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug)) 
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }
                dto.Slug = slug;
                dto.Bodey = model.Bodey;
                dto.Sorting = model.Sorting; //100 add to end

                db.Pages.Add(dto);
                db.SaveChanges();

            }

            TempData["msg"] = "You have added a new page!";
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id) 
        {
            PageViewModel model;

            using (Db db = new Db()) 
            {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null) 
                {
                    return Content("The page does not exist.");
                }

                model = new PageViewModel(dto);
            }

            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageViewModel model) 
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            using (Db db = new Db()) 
            {
                int id = model.Id;

                string slug = "home";

                PagesDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                        slug = model.Title.Replace(" ", "-").ToLower();
                    else
                        slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title alredy exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == model.Slug)) 
                {
                    ModelState.AddModelError("", "That slug alredy exist.");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Bodey = model.Bodey;
                dto.Sorting = model.Sorting;

                db.SaveChanges();
            }
            TempData["msg"] = "You have edited the page.";

            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails

        public ActionResult PageDetails(int id)         
        {
            PageViewModel model;

            using (Db db = new Db()) 
            {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null) 
                {
                    return Content("The page does not exist.");
                }

                model = new PageViewModel(dto);
            }

            return View(model);
        }


        public ActionResult DeletePage(int id) 
        {
            using (Db db = new Db()) 
            {
                PagesDTO dto = db.Pages.Find(id);
                db.Pages.Remove(dto);
                db.SaveChanges();
            }

            TempData["msg"] = "You have deleted a page.";
            return RedirectToAction("Index");
        }

        //Create sortable method

        [HttpPost]
        public void ReorderPages(int[] id) 
        {
            using (Db db = new Db()) 
            {
                int count = 1;

                PagesDTO dto;

                foreach (var pageId in id) 
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

    }
}