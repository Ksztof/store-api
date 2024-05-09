using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Shared.DTO.Request
{
    public class IsCurrentCartDtoApp
    {
        public int CartId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
