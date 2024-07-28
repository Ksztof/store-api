using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.SignalR
{
    public interface INotificationService
    {
        public Task SendPaymentStatusAsync(string orderId, string status, Error? error);
    }
}
