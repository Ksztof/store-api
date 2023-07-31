﻿using PerfumeStore.Domain.Enums;
using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DbModels
{
    public class Order : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatusE Status { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public void CreateOrder(int cartId)
        {
            OrderDate = DateTime.Now;
            Status = OrderStatusE.New;
            CartId = cartId;
        }
    }
}
