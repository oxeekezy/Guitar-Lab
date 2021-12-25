using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitLab.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total
        {
            get { return Quantity * Price; }
        }
        public string Image { get; set; }
    }
}