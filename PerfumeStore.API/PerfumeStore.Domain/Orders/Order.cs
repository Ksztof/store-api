using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.ShippingDetails;

namespace PerfumeStore.Domain.Orders
{
    public class Order : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatusE Status { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ShippingDetailId { get; set; }
        public ShippingDet ShippingDetail { get; set; }

        public void CreateOrder(int cartId, ShippingDet shippingDetails)
        {
            OrderDate = DateTime.Now;
            Status = OrderStatusE.New;
            CartId = cartId;
            ShippingDetail = shippingDetails;
        }

        public void MarkAsDeleted()
        {
            Status = OrderStatusE.Cancelled;
        }
    }
}