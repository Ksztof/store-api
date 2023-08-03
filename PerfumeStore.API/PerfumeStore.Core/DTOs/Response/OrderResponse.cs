using PerfumeStore.Domain.Enums;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PerfumeStore.Core.DTOs.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDto> AboutProductsInCart { get; set; }
    }
}
