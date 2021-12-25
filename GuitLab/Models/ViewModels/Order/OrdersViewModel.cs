using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GuitLab.Models.Data;

namespace GuitLab.Models.ViewModels.Order
{
    public class OrdersViewModel
    {
        public OrdersViewModel()
        {
        }

        public OrdersViewModel(OrderDTO row)
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            CreatedAt = row.CreatedAt;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}