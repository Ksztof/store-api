﻿namespace PerfumeStore.Domain.DTO.Response.Cart
{
    public class CheckCartDomRes
    {
        public string ProductName { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}