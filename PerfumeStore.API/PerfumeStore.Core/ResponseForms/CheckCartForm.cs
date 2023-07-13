using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.ResponseForms
{
    public class CheckCartForm
    {
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCart> ProductsInCart { get; set; }
    }
}
