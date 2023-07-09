using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DbModels
{
	public class Cart : IEntity
	{
		public int CartId { get; set; }
		public int UserId { get; set; }
		public Dictionary<int, CartProduct>? CartProducts { get; set; }
	}
}
