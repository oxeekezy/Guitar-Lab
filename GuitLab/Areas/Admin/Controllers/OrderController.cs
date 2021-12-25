using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitLab.Areas.Admin.ViewModels.Orders;
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Lab;
using GuitLab.Models.ViewModels.Order;

namespace GuitLab.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        // GET: Admin/Order
        public ActionResult Index()
        {
            List<OrdersForAdminViewModel> ordersForAdmin = new List<OrdersForAdminViewModel>();

            using (Db db = new Db())
            {
                List<OrdersViewModel> orders = db.Orders
                    .ToArray()
                    .Select(x => new OrdersViewModel(x))
                    .ToList();

                foreach (var order in orders)
                {
                    Dictionary<string,int> serviceAndQty = new Dictionary<string, int>();

                    decimal total = 0m;

                    List<OrderDetailsDTO> orderDetailsList =
                        db.OrderDetails
                            .Where(x => x.OrderId == order.OrderId)
                            .ToList();

                    UserDTO user = db.Users.FirstOrDefault(x => x.Id == order.UserId);
                    string username = user.UserName;

                    foreach (var orderDetails in orderDetailsList)
                    {
                        ServicesDTO services = db.Services.FirstOrDefault(x => x.Id == orderDetails.ServiceId);

                        decimal price = services.Price;
                        string servicename = services.Name;

                        serviceAndQty.Add(servicename,orderDetails.Quantity);
                        total += orderDetails.Quantity * price;
                    }

                    ordersForAdmin.Add(new OrdersForAdminViewModel()
                    {
                        OrderNumber = order.OrderId,
                        UserName = username,
                        Total = total,
                        ServiceAndQty = serviceAndQty,
                        CreatedAt = order.CreatedAt
                    });

                }
            }

            return View(ordersForAdmin);
        }
    }
} 