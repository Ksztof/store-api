using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using PerfumeStore.Core.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PerfumeShop.Client.Pages
{
    public partial class Products
    {
        private IEnumerable<ProductResponse> AllProducts = new List<ProductResponse>();
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
 
        protected override async Task OnInitializedAsync()
        {
            var result = await HttpClient.GetAsync(Config["apiUrl"] + "/api/Products");

            if (result.IsSuccessStatusCode)
            {
                AllProducts = await result.Content.ReadFromJsonAsync<List<ProductResponse>>();
                Console.WriteLine(AllProducts);
            }
        }
    }
}
