using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Models
{
	public class CartProduct
	{
		public int? CartProductId { get; set; }
		public Product Product { get; set; }
		public decimal ProductQuantity { get; set; }
	}
}
