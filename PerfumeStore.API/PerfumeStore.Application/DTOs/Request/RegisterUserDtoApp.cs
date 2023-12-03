using PerfumeStore.Domain.StoreUsers;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Application.DTOs.Request
{
    public class RegisterUserDtoApp
    {
        public StoreUser StoreUser { get; set; }
        public string Password { get; set; }

    }
}
