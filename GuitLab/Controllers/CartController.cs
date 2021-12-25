using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Cart;

namespace GuitLab.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View(cart);
        }

        public ActionResult CartPartial()
        {
            CartViewModel model = new CartViewModel();

            int qty = 0;

            decimal price = 0m;

            if (Session["cart"] != null)
            {
                var list = (List<CartViewModel>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price +=item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                model.Quantity = 0;
                model.Price = 0m;
            }

            return PartialView("_CartPartial",model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            CartViewModel model = new CartViewModel();

            using (Db db = new Db())
            {
                ServicesDTO service = db.Services.Find(id);

                var serviceInCart = cart.FirstOrDefault(x => x.ServiceId == id);

                if (serviceInCart == null)
                {
                    cart.Add(new CartViewModel()
                    {
                        ServiceId = service.Id,
                        ServiceName = service.Name,
                        Quantity = 1,
                        Price = service.Price,
                        Image = service.ImageName

                    });
                }
                else
                {
                    serviceInCart.Quantity++;
                }
            }

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            Session["cart"] = cart;

            return PartialView("_AddToCartPartial",model);
        }

        public JsonResult IncrementService(int serviceId)
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (Db db = new Db())
            {
                CartViewModel model = cart.FirstOrDefault(x => x.ServiceId == serviceId);

                model.Quantity++;

                var result = new {qty = model.Quantity, price = model.Price};

                return Json(result,JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult DecrementService(int serviceId) 
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (Db db = new Db())
            {
                CartViewModel model = cart.FirstOrDefault(x => x.ServiceId == serviceId);

                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public void RemoveService(int serviceId)
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            using (Db db = new Db())
            {
                CartViewModel model = cart.FirstOrDefault(x => x.ServiceId == serviceId);

                cart.Remove(model);
            }
        }

        public ActionResult PaypalPartial()
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            return PartialView(cart);
        }

        [HttpPost]
        public void PlaceOrder()
        {
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel>;

            string userName = User.Identity.Name;

            int orderId = 0;

            using (Db db = new Db())
            {
                OrderDTO orderDto = new OrderDTO();

                var q = db.Users.FirstOrDefault(x => x.UserName == userName);
                int userId = q.Id;

                orderDto.UserId = userId;
                orderDto.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDto);
                db.SaveChanges();

                orderId = orderDto.OrderId;

                OrderDetailsDTO orderDetailsDto = new OrderDetailsDTO();

                decimal total = 0m;
                List<string> listToEmail = new List<string>();

                foreach (var item in cart)
                {
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ServiceId = item.ServiceId;
                    orderDetailsDto.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();

                    total += item.Total;

                    listToEmail.Add(item.ServiceName + " | "+ item.Quantity + " pc.");
                }

                SendEmail mail = new SendEmail();
                mail.Send("guitarlabsys@gmail.com", "guitarlabtest", q.EmailAdress, "New order in Guitar Lab",
                    mail.BodeyMessage(orderDetailsDto.OrderId,total, q.FirstName, q.LastName,listToEmail), "smtp.gmail.com");


                ClientsDTO clientsDto = new ClientsDTO();

                if (db.Clients.Any(x => x.Email == q.EmailAdress))
                {
                    
                }
                else
                {
                    clientsDto.UserId = q.Id;
                    clientsDto.FirstName = q.FirstName;
                    clientsDto.LastName = q.LastName;
                    clientsDto.Email = q.EmailAdress;

                    db.Clients.Add(clientsDto);
                    db.SaveChanges();
                }

            }

            
            

            Session["cart"] = null;


        }
    }
}