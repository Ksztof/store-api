using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.DTOs.Request
{
    public class ProductModificationApplication
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
