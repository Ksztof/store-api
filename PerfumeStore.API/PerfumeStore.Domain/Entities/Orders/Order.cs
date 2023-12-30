using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Entities.StoreUsers;
using PerfumeStore.Domain.Repositories.Generics;
using PerfumeStore.Domain.Shared.Enums;

namespace PerfumeStore.Domain.Entities.Orders
{
    public class Order : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatuses Status { get; set; }
        public string? StoreUserId { get; set; }
        public StoreUser? StoreUser { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ShippingDetailId { get; set; }
        public ShippingDet ShippingDetail { get; set; }

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
}