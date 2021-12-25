using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GuitLab.Models.Data;

namespace GuitLab.Models.ViewModels.Clients
{
    public class ClientsViewModel
    {
        public ClientsViewModel()
        {
        }

        public ClientsViewModel(ClientsDTO row)
        {
            UserId = row.UserId;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Email = row.Email;
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }
}