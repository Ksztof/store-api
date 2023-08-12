using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using PerfumeShop.Client.Services;
using PerfumeStore.Core.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace PerfumeShop.Client.Pages
{
    public partial class Products
    {
        private List<ProductResponse> AllProducts = new();
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private ITokenService TokenService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var tokenResponse = await TokenService.GetToken("PerfumeStoreAPI.read");
            //HttpClient.SetBearerToken(tokenResponse.AccessToken);

            var result = await HttpClient.GetAsync(Config["apiUrl"] + "/api/products");

            if (result.IsSuccessStatusCode)
            {
                AllProducts = await result.Content.ReadFromJsonAsync<List<ProductResponse>>();
            }
        }
    }
}
