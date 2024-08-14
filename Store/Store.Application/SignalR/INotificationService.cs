using Store.Domain.Abstractions;

namespace Store.Application.SignalR
{
    public interface INotificationService
    {
        public Task SendPaymentStatusAsync(string orderId, string status, Error? error);
    }
}
