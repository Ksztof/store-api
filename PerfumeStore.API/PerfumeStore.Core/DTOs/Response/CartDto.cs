using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PerfumeStore.Core.DTOs.Response
{
    public class CartDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public IEnumerable<CartLineDto> CartLineDto { get; set; }
    }
}
