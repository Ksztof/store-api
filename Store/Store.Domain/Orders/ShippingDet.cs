using Store.Domain.Abstractions;
using Store.Domain.Orders.Dto.Request;

namespace Store.Domain.Orders;

public class ShippingDet : Entity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string HomeNumber { get; set; }
    public string PostCode { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public Order Order { get; set; }

    public ShippingDet() { }

    public void CreateShippingDetail(CreateOrderDtoDom createOrderDtoDom)
    {
        FirstName = createOrderDtoDom.FirstName;
        LastName = createOrderDtoDom.LastName;
        Email = createOrderDtoDom.Email;
        Street = createOrderDtoDom.Street;
        StreetNumber = createOrderDtoDom.StreetNumber;
        HomeNumber = createOrderDtoDom.HomeNumber;
        PostCode = createOrderDtoDom.PostCode;
        City = createOrderDtoDom.City;
        PhoneNumber = createOrderDtoDom.PhoneNumber;
    }
}
