using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Models
{
	public class ProductCategories : IEntity
	{
		public int ProductCategoryId { get; set; }
		public string CategoryName { get; set; }	
	}
}
