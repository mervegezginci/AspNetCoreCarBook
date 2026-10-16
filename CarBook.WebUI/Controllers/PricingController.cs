using CarBook.Dto.CarPricingDtos;
using CarBook.Dto.PricingDtos;
using CarBook.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.Controllers
{
    public class PricingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PricingController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.v1 = "Ödemeler";
            ViewBag.v2 = "Ödeme Türleri";

            var client = _httpClientFactory.CreateClient();

            // Pricing tiplerini çek (sütun başlıkları)
            var pricingResponse = await client.GetAsync("https://localhost:7046/api/Pricings");
            List<ResultPricingDto> pricingTypes = new();
            if (pricingResponse.IsSuccessStatusCode)
            {
                var pricingJson = await pricingResponse.Content.ReadAsStringAsync();
                pricingTypes = JsonConvert.DeserializeObject<List<ResultPricingDto>>(pricingJson) ?? new();
            }

            // Araç + fiyat listesini çek
            var carPricingResponse = await client.GetAsync("https://localhost:7046/api/CarPricings");
            List<ResultCarPricingWithCarDto> carPricings = new();
            if (carPricingResponse.IsSuccessStatusCode)
            {
                var carPricingJson = await carPricingResponse.Content.ReadAsStringAsync();
                carPricings = JsonConvert.DeserializeObject<List<ResultCarPricingWithCarDto>>(carPricingJson) ?? new();
            }

            // Araç bazında grupla
            var grouped = carPricings
                .GroupBy(x => x.CarID)
                .Select(g => new ResultCarPricingGroupedDto
                {
                    CarID = g.Key,
                    BrandName = g.First().BrandName,
                    Model = g.First().Model,
                    CoverImageUrl = g.First().CoverImageUrl,
                    Prices = g.ToDictionary(x => x.PricingName, x => x.Amount)
                })
                .ToList();

            var viewModel = new PricingViewModel
            {
                PricingTypes = pricingTypes,
                CarPricings = grouped
            };

            return View(viewModel);
        }
    }
}
