﻿using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
    public class CartLine
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public decimal Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}