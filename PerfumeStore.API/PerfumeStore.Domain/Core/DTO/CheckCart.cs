﻿namespace PerfumeStore.Domain.Core.DTO
{
    public class CheckCartDto
    {
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}