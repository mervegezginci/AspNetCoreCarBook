using CarBook.Dto.CarPricingDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CarBook.WebUI.ViewComponents.DashboardComponents
{
    public class _AdminDashboardStatisticsComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _AdminDashboardStatisticsComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var random = new Random();

            ViewBag.v1 = random.Next(10, 95);
            ViewBag.locationCountRandom = random.Next(10, 95);
            ViewBag.brandCountRandom = random.Next(10, 95);
            ViewBag.avgRentPriceForDailyRandom = random.Next(10, 95);

            // 1. Toplam Araç Sayısı
            try
            {
                var response = await client.GetAsync("https://localhost:7046/api/Cars");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var jArray = JArray.Parse(data);
                    ViewBag.v = jArray.Count;
                }
                else
                {
                    ViewBag.v = 0;
                }
            }
            catch
            {
                ViewBag.v = 0;
            }

            // 2. Lokasyon Sayısı
            try
            {
                var response = await client.GetAsync("https://localhost:7046/api/Locations");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var jArray = JArray.Parse(data);
                    ViewBag.locationCount = jArray.Count;
                }
                else
                {
                    ViewBag.locationCount = 0;
                }
            }
            catch
            {
                ViewBag.locationCount = 0;
            }

            // 3. Marka Sayısı
            try
            {
                var response = await client.GetAsync("https://localhost:7046/api/Brands");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var jArray = JArray.Parse(data);
                    ViewBag.brandCount = jArray.Count;
                }
                else
                {
                    ViewBag.brandCount = 0;
                }
            }
            catch
            {
                ViewBag.brandCount = 0;
            }

            // 4. Ortalama Günlük Kiralama Ücreti
            try
            {
                var response = await client.GetAsync("https://localhost:7046/api/CarPricings");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var pricings = JsonConvert.DeserializeObject<List<ResultCarPricingWithCarDto>>(data);
                    var dailyPricings = pricings?
                        .Where(x => (x.PricingName != null && x.PricingName.Contains("Günlük")) || x.PricingID == 1)
                        .ToList();

                    if (dailyPricings != null && dailyPricings.Any())
                    {
                        var avg = dailyPricings.Average(x => x.Amount);
                        ViewBag.avgRentPriceForDaily = avg.ToString("N2");
                    }
                    else if (pricings != null && pricings.Any())
                    {
                        var avg = pricings.Average(x => x.Amount);
                        ViewBag.avgRentPriceForDaily = avg.ToString("N2");
                    }
                    else
                    {
                        ViewBag.avgRentPriceForDaily = "0.00";
                    }
                }
                else
                {
                    ViewBag.avgRentPriceForDaily = "0.00";
                }
            }
            catch
            {
                ViewBag.avgRentPriceForDaily = "0.00";
            }

            return View();
        }
    }
}
