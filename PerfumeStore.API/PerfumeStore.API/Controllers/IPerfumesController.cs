using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.Controllers;

namespace PerfumeStore.API.Controllers
{
	public interface IPerfumesController // : ICRUD<Product>
	{
		public Task<IActionResult> CreateAsync([FromBody] CreateFrom createFrom);
		public Task<IActionResult> UpdateAsync(int id);
		public Task<IActionResult> DeleteAsync(int id);
		public Task<IActionResult> GetByIdAsync(int id);
		public Task<IActionResult> GetAllAsync();
	}
}
