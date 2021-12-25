using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GuitLab.Models.Data
{
    [Table("Clients")]
    public class ClientsDTO
    {
        [Key] public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [ForeignKey("UserId")] public virtual UserDTO User { get; set; }
    }
}