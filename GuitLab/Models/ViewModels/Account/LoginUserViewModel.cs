using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GuitLab.Models.ViewModels.Account
{
    public class LoginUserViewModel
    {
        [Required] [DisplayName("Username")] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [DisplayName("Remember me")]public bool RememberMe { get; set; }
    }
}