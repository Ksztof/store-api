using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Services
{
    public interface IEmailService
    {
        public Task SendActivationLink(StoreUser user);
        public Task ConfirmEmail(string userId, string encodedEmailToken); //TODO: exceptions are stupid
    }
}
