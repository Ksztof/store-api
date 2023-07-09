using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DbModels
{
	public class Product : IEntity
	{
		[Key]
		public int ProductId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Description { get; set; }
		public int CategoryId { get; set; }
		public string? Manufacturer { get; set; }
		public DateTime DateAdded { get; set; }
	}
}
