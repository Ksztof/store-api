﻿using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Repositories
{
  public class ProductCategoriesRepository : IProductCategoriesRepository
  {
    private readonly ShopDbContext _shopDbContext;

    public ProductCategoriesRepository(ShopDbContext shopDbContext)
    {
      _shopDbContext = shopDbContext;
    }

    public async Task<ProductCategory?> GetByIdAsync(int id)
    {
      ProductCategory? ProductCategory = await _shopDbContext.ProductCategories.FindAsync(id);
      return ProductCategory;
    }

    public async Task<ICollection<ProductCategory>> GetByIdsAsync(ICollection<int> ids)
    {
      ICollection<ProductCategory> ProductCategory = await _shopDbContext.ProductCategories.Where(x => ids.Contains(x.Id)).ToListAsync();
      return ProductCategory;
    }
  }
}