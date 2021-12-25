using GuitLab.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitLab.Models.ViewModels.Pages
{
    public class PageViewModel
    {
        public PageViewModel()
        {
            //TODO: Null value row Exception
        }

        public PageViewModel(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Bodey = row.Bodey;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        [Required] [StringLength(50, MinimumLength = 3)] public string Title { get; set; }
        [Required] [StringLength(int.MaxValue, MinimumLength = 3)] public string Slug { get; set; }

        [Required] [StringLength(int.MaxValue, MinimumLength = 10)] [AllowHtml] public string Bodey { get; set; }
        public int Sorting { get; set; }
    }
}