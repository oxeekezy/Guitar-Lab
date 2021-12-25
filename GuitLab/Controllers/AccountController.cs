using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GuitLab.Models.Data;
using GuitLab.Models.ViewModels.Account;
using GuitLab.Models.ViewModels.Order;

namespace GuitLab.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [ActionName("create-account")]
        [HttpGet]

        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("","Password do not match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("",$"Username {model.UserName} is taken.");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,
                    Password = model.Password
                };

                db.Users.Add(userDTO);
                db.SaveChanges();

                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            TempData["msg"] = "You now registered and can login!";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)
                                      && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }

                if (!isValid)
                {
                    ModelState.AddModelError("","Invalid username or password!");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
                }
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;

            UserNavPartialVM model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            return PartialView(model);
        }

        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            string username = User.Identity.Name;

            UserProfileViewModel model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == username);

                model = new UserProfileViewModel(dto);
            }

            return View("UserProfile",model);
        }

        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileViewModel model)
        {
            bool userNameIsChanget = false;

            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("","Passwords do not match!");
                    return View("UserProfile", model);
                }

                using (Db db = new Db())
                {
                    string username = User.Identity.Name;

                    if (username != model.UserName)
                    {
                        username = model.UserName;
                        userNameIsChanget = true;
                    }

                    if (db.Users.Where(x => x.Id != model.Id).Any(x => x.UserName == username))
                    {
                        ModelState.AddModelError("",$"User name {model.UserName} alredy exists");
                        model.UserName = "";
                        return View("UserProfile", model);
                    }

                    UserDTO dto = db.Users.Find(model.Id);

                    dto.FirstName = model.FirstName;
                    dto.LastName = model.LastName;
                    dto.EmailAdress = model.EmailAdress;
                    dto.UserName = model.UserName;

                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        dto.Password = model.Password;
                    }
                    
                    db.SaveChanges();
                }

                TempData["msg"] = "You have edited your profile";

            }
            else
            {
                ModelState.AddModelError("", "Enter the password");
                return View("UserProfile", model);
            }

            if (!userNameIsChanget)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
            
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            List<OrdersForUserViewModel> ordersForUser = new List<OrdersForUserViewModel>();

            using (Db db = new Db())
            {
                UserDTO user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                int userId = user.Id;

                List<OrdersViewModel> orders = db.Orders
                    .Where(x => x.UserId == userId)
                    .ToArray()
                    .Select(x => new OrdersViewModel(x))
                    .ToList();

                foreach (var order in orders)
                {
                    Dictionary<string,int> servicesAndQty = new Dictionary<string, int>();
                    decimal total = 0m;

                    List<OrderDetailsDTO> orderDetailsDTO = db.OrderDetails
                        .Where(x => x.OrderId == order.OrderId)
                        .ToList();

                    foreach (var orderDetails in orderDetailsDTO)
                    {
                        ServicesDTO service = db.Services
                            .FirstOrDefault(x => x.Id == orderDetails.ServiceId);
                        decimal price = service.Price;
                        string serviceName = service.Name;

                        servicesAndQty.Add(serviceName,orderDetails.Quantity);
                        total += orderDetails.Quantity * price;
                    }

                    ordersForUser.Add(new OrdersForUserViewModel() 
                     {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ServiceAndQty = servicesAndQty,
                        CreatedAt = order.CreatedAt

                     });

                }
            }

            

            return View(ordersForUser);
        }
    }
}