﻿namespace Store.Domain.Products.Dto.Request;

public class UpdateProductDtoDom
{
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductManufacturer { get; set; }
    public ICollection<int>? ProductCategoriesIds { get; set; }
}