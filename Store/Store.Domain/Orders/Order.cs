using Store.Domain.Abstractions;
using Store.Domain.Carts;
using Store.Domain.Orders.Enums;
using Store.Domain.StoreUsers;

namespace Store.Domain.Orders;

public class Order : Entity
{
    public DateTime OrderDate { get; set; }
    public OrderStatuses Status { get; set; }
    public string? StoreUserId { get; set; }
    public StoreUser? StoreUser { get; set; }
    public int CartId { get; set; }
    public Cart Cart { get; set; }
    public int ShippingDetailId { get; set; }
    public ShippingDet ShippingDetail { get; set; }

    public Order() { }

    public void CreateOrder(int cartId, string userId, ShippingDet shippingDetails)
    {
        OrderDate = DateTime.Now;
        Status = OrderStatuses.New;
        StoreUserId = userId;
        CartId = cartId;
        ShippingDetail = shippingDetails;
    }

    public void CreateOrder(int cartId, ShippingDet shippingDetails)
    {
        OrderDate = DateTime.Now;
        Status = OrderStatuses.New;
        CartId = cartId;
        ShippingDetail = shippingDetails;
    }

    public void MarkAsDeleted()
    {
        Status = OrderStatuses.Cancelled;
    }
}