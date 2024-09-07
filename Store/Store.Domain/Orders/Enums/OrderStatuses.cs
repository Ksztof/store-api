namespace Store.Domain.Orders.Enums;

public enum OrderStatuses
{
    New,
    Processing,
    Paid,
    Shipped,
    Delivered,
    Completed,
    Cancelled,
    Returned,
    Refunded,
}