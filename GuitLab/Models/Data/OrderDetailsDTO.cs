﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GuitLab.Models.Data
{
    [Table("OrderDetails")]
    public class OrderDetailsDTO
    {
        [Key] public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderDTO Orders { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO Users { get; set; }

        [ForeignKey("ServiceId")]
        public virtual ServicesDTO Services { get; set; }
    }
}