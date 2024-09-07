using Store.API.Shared.DTO.Models;

namespace Store.API.Shared.DTO.Request.Cart;

public class NewProductsDtoApi
{
    public ProductInCartApi[] Products { get; set; }
}