using PerfumeStore.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
	public class PerfumesService // : interfejsy
	{
		private PerfumesRepository perfumesRepository = null;
		private PerfumesRepository _perfumesRepository
		{
			get
			{
				if (perfumesRepository == null)
				{
					perfumesRepository = new PerfumesRepository();
				}
				return perfumesRepository;
			}
		}

		public PerfumesService()
		{

		}

		//FUNKCJE
	}
}
