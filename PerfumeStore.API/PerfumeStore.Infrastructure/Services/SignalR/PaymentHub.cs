using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.Services.SignalR
{
    public class PaymentHub : Hub
    {
        public async Task JoinGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        }

        public async Task LeaveGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
        }

        public async Task SendPaymentStatus(string orderId, string status)
        {
            await Clients.Group(orderId).SendAsync("ReceivePaymentStatus", status);
        }
    }
}
