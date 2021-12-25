using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GuitLab.Models.Data;

namespace GuitLab.Models.ViewModels.Account
{
    public class UserViewModel
    {
        public UserViewModel()
        {
        }

        public UserViewModel(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAdress = row.EmailAdress;
            UserName = row.UserName;
            Password = row.Password;
        }
        public int Id { get; set; }
        [Required][DisplayName("First Name")] public string FirstName { get; set; }
        [Required] [DisplayName("Last Name")] public string LastName { get; set; }
        [Required] [DataType(DataType.EmailAddress)]
        [DisplayName("Email")] public string EmailAdress { get; set; }
        [Required] [DisplayName("User Name")] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [Required] [DisplayName("Confim Password")] public string ConfirmPassword { get; set; }
    }
}