using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DbModels
{
	public class ProductCategories : IEntity
	{
		public int ProductCategoryId { get; set; }
		public string Name { get; set; }
	}
}
