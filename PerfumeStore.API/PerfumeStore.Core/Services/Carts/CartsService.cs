﻿using PerfumeStore.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services.Carts
{
    public class CartsService : ICartsService
	{
		private readonly ICartsRepository _cartRepository;

		public CartsService(ICartsRepository cartRepository)
		{
			_cartRepository = cartRepository;
		}
	}
}
