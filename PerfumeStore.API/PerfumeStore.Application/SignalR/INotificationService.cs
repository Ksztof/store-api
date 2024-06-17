using PerfumeStore.Application.Abstractions.Result.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.SignalR
{
    public interface INotificationService
    {
        public Task SendPaymentStatusAsync(string orderId, string status, Error? error);
    }
}
