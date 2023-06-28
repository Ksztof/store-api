using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PerfumeStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PerfumesController : ControllerBase, IPerfumesController
	{
		private readonly IPerfumesService _perfumesService;
		public PerfumesController(IPerfumesService perfumesService)
		{
			_perfumesService = perfumesService;
		}


		[HttpPost]
		public async Task<IActionResult> CreateAsync([FromBody] CreateForm createFrom)
		{
			throw new NotImplementedException();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAsync(int id)
		{
			throw new NotImplementedException();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAsync()
		{
			throw new NotImplementedException();
		}
	}
}
