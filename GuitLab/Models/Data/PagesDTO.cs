using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GuitLab.Models.Data
{
    [Table("Pages")]
    public class PagesDTO
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Bodey { get; set; }
        public int Sorting { get; set; }
    }
}