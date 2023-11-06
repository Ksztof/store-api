using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.DTOs.Response
{
  public class OrderResponse
  {
    public int Id { get; set; }
    public decimal TotalCartValue { get; set; }
    public IEnumerable<CheckCartDto> AboutProductsInCart { get; set; }
    public IEnumerable<string> Errors { get; set; }
  }
}