using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Infrastructure.Emails
{
    public interface IEmailService
    {
        public Task SendActivationLink(StoreUser user);
        public Task ConfirmEmail(string userId, string encodedEmailToken); //TODO: exceptions are stupid
    }
}
