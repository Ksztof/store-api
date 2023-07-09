using PerfumeStore.Core.GenericInterfaces;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PerfumeStore.Core.Repositories
{
    public interface IProductsRepository : IRepository<Product>
    {
    }
}
