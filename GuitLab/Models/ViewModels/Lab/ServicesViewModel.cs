using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitLab.Models.Data;

namespace GuitLab.Models.ViewModels.Lab
{
    public class ServicesViewModel
    {
        public ServicesViewModel()
        {
        }

        public ServicesViewModel(ServicesDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Desc = row.Desc;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
        }

        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public string Slug { get; set; }
        [Required] public string Desc { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required][DisplayName("Category")] public int CategoryId { get; set; }
        [DisplayName("Image")] public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}