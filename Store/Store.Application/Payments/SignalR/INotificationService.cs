using Store.Domain.Abstractions;

namespace Store.Application.Payments.SignalR
{
    public interface INotificationService
    {
        public Task SendPaymentStatusAsync(string orderId, string status, Error? error);
    }
}
