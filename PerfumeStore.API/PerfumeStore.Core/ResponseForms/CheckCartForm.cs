using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.ResponseForms
{
	public class CheckCartForm
	{
		public decimal TotalCartValue { get; set; }
		public IEnumerable<CheckCart> ProductsInCart { get; set; }
	}
}
