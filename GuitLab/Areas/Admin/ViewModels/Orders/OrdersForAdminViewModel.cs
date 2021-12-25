using System;
using System.Collections.Generic;

namespace GuitLab.Areas.Admin.ViewModels.Orders
{
    public class OrdersForAdminViewModel
    {
        public int OrderNumber { get; set; }
        public string UserName { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string,int> ServiceAndQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}