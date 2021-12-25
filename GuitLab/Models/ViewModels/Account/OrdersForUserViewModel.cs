using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitLab.Models.ViewModels.Account
{
    public class OrdersForUserViewModel
    {
        public int OrderNumber { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ServiceAndQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}