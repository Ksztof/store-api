using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
    public interface IOrdersService 
    {
        public Task<AboutCartResponse> CreateOrderAsync();
    }
}
