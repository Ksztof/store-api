using Microsoft.AspNetCore.Http;
using PerfumeStore.Application.Contracts.HttpContext;
using PerfumeStore.Infrastructure.Services.HttpContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.Services.Cookies
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextService _contextService;

        public CookieService(IHttpContextService contextService)
        {
            _contextService = contextService;
        }

        public void SetCookieWithToken(string token)
        {
            var time = DateTimeOffset.Now.AddDays(1);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, //TODO: for localhost false 
                Expires = DateTimeOffset.Now.AddDays(1),
            };

            _contextService.SendCookieWithToken(token, cookieOptions);
        }
    }
}
