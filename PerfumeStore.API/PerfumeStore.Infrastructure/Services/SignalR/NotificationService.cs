using Microsoft.AspNetCore.SignalR;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.Services.SignalR
{
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
}
