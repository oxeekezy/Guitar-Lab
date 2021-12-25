
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Lab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            List<CategoryViewModel> categoryList;

            using (Db db = new Db())
            {
                categoryList = db.Categories
                                .ToArray()
                                .OrderBy(c => c.Sorting)
                                .Select(c => new CategoryViewModel(c))
                                .ToList();

            }

            return View(categoryList);
        }

        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                CategoryDTO dto = new CategoryDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
            }

            return id;
        }

        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                CategoryDTO dto;

                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);
                db.SaveChanges();
            }

            TempData["msg"] = "You have deleted a category.";
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }
            return "ok";
        }

        [HttpGet]
        public ActionResult AddService()
        {
            ServicesViewModel model = new ServicesViewModel();

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AddService(ServicesViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                if (db.Services.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That service name is taken");
                    return View(model);
                }
            }


            int id;

            using (Db db = new Db())
            {
                ServicesDTO service = new ServicesDTO();

                service.Name = model.Name;
                service.Slug = model.Name.Replace(" ", "-").ToLower();
                service.Desc = model.Desc;
                service.Price = model.Price;
                service.CategoryId = model.CategoryId;

                CategoryDTO cat = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                service.CategoryName = cat.Name;

                db.Services.Add(service);
                db.SaveChanges();

                id = service.Id;
            }

            TempData["msg"] = "You have added a service";

            #region Upload Image

            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Services");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/png" &&
                    ext != "image/x-png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image type");
                        return View(model);
                    }
                }


                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    ServicesDTO dto = db.Services.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                file.SaveAs(path);
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);
            }
            #endregion

            return RedirectToAction("AddService");
        }

        [HttpGet]
        public ActionResult Services(int? page, int? catId)
        {
            List<ServicesViewModel> listOfServiceVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                listOfServiceVM = db.Services.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ServicesViewModel(x))
                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.SelectedCat = catId.ToString();
            }

            var onePageOfService = listOfServiceVM.ToPagedList(pageNumber, 20);
            ViewBag.onePageOfService = onePageOfService;

            return View(listOfServiceVM);
        }


        [HttpGet]
        public ActionResult EditService(int id)
        {
            ServicesViewModel model;

            using (Db db = new Db())
            {
                ServicesDTO dto = db.Services.Find(id);

                if (dto == null)
                {
                    return Content("That service does not exist");
                }

                model = new ServicesViewModel(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditService(ServicesViewModel model, HttpPostedFileBase file)
        {
            int id = model.Id;

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Services/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Services.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That service name is taken");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                ServicesDTO dto = db.Services.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Desc = model.Desc;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO cat = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = cat.Name;

                db.SaveChanges();
            }

            TempData["msg"] = "You have edited the service";

            #region ImageUpload

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/png" &&
                    ext != "image/x-png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image type");
                        return View(model);
                    }
                }

                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ServicesDTO dto = db.Services.Find(id);

                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");
                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);

            }


            #endregion

            return RedirectToAction("EditService");
        }

        public ActionResult DeleteService(int id)
        {
            using (Db db = new Db())
            {
                ServicesDTO dto = db.Services.Find(id);
                db.Services.Remove(dto);
                db.SaveChanges();
            }

            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Services\\" + id.ToString());

            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString, true);
            }

            return RedirectToAction("Services");
        }

        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if (file != null && file.ContentLength > 0)
                {
                    var originalDerictory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string pathString1 = Path.Combine(originalDerictory.ToString(), "Services\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDerictory.ToString(), "Services\\" + id.ToString() + "\\Gallery\\Thumbs");

                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1, 1);
                    img.Save(path2);
                }
            }
        }

        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Services/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Services/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }

            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }

    }
}