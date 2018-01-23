﻿namespace FastFood.Models
{
    using FastFood.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
        public int Id { get; set; }

        [Required]
        public string Customer { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public OrderType Type { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        public Employee Employee { get; set; }

        public ICollection<OrderItem>OrderItems { get; set; }

    }
}