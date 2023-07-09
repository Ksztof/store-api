using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Models
{
	public class CheckCart
	{
		public int ProductId { get; set; }
		public decimal ProductUnitPrice { get; set; }
		public decimal ProductTotalPrice { get; set; }
		public decimal Quantity { get; set; }
	}
}
