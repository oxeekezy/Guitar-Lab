using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GuitLab.Models.Data
{
    public class Db : DbContext
    {
        public Db() : base("name=Db")
        {
        }

        public DbSet<PagesDTO> Pages { get; set; }

        public DbSet<CategoryDTO> Categories { get; set; }

        public DbSet<ServicesDTO> Services { get; set; }

        public DbSet<UserDTO> Users { get; set; }

        public DbSet<RoleDTO> Roles { get; set; }

        public DbSet<UserRoleDTO> UserRoles { get; set; }

        public DbSet<OrderDTO> Orders { get; set; }

        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }
        public DbSet<ClientsDTO> Clients { get; set; }
    }
}