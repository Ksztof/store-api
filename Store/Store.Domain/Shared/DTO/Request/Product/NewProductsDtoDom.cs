using Store.Domain.Shared.DTO.models;

namespace Store.Domain.Shared.DTO.Request.Product
{
    public class NewProductsDtoDom
    {
        public ProductInCartDom[] Products { get; set; }
    }
}
