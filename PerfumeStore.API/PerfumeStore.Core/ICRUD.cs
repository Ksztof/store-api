using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PerfumeStore.Core
{
	public interface ICRUD<T> where T : class
	{
		public Task<IActionResult> CreateAsync([FromBody] T createFrom);
		public Task<IActionResult> UpdateAsync([FromBody] T updateFrom);
		public Task<IActionResult> DeleteAsync([FromBody] T deleteFrom);
		public Task<IActionResult> GetByIdAsync(int id);
		public Task<IActionResult> GetAllAsync();
	}
}
