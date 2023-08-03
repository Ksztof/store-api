﻿using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Repositories
{
    public interface IOrdersRepository
    {
        public Task<Order> CreateOrderAsync(Order order);
        public Task<Order?> GetByIdAsync(int orderId);
        public Task DeleteOrderAsync(Order order);
        public Task UpdateAsync(Order order);
    }
}
