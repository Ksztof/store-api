using Microsoft.AspNetCore.SignalR;
using Store.Application.Payments.SignalR;
using Store.Domain.Abstractions;

namespace Store.Infrastructure.Services.SignalR;

public class NotificationService : INotificationService
{
    private readonly IHubContext<PaymentHub> _hubContext;

    public NotificationService(IHubContext<PaymentHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendPaymentStatusAsync(string orderId, string status, Error? error)
    {
        var response = new
        {
            Status = status,
            Error = error
        };

        await _hubContext.Clients.Group(orderId).SendAsync("ReceivePaymentStatus", response);
    }
}
