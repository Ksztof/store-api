using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.Services.Cookies
{
    public interface ICookieService 
    {
        public void SetCookieWithToken(string token);
    }
}
