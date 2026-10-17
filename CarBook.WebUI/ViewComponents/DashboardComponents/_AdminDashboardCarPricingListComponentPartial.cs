using CarBook.Dto.CarPricingDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.ViewComponents.DashboardComponents
{
    public class _AdminDashboardCarPricingListComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _AdminDashboardCarPricingListComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var responseMessage = await client.GetAsync("https://localhost:7046/api/CarPricings");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultCarPricingWithCarDto>>(jsonData);

                    var grouped = values?
                        .GroupBy(x => new { x.CarID, x.BrandName, x.Model, x.CoverImageUrl })
                        .Select(g => new ResultCarPricingListWithModelDto
                        {
                            Brand = g.Key.BrandName ?? "",
                            Model = g.Key.Model ?? "",
                            CoverImageUrl = g.Key.CoverImageUrl ?? "",
                            DailyAmount = g.FirstOrDefault(x => x.PricingName != null && x.PricingName.Contains("Günlük"))?.Amount 
                                          ?? g.FirstOrDefault(x => x.PricingID == 1)?.Amount ?? 0,
                            WeeklyAmount = g.FirstOrDefault(x => x.PricingName != null && x.PricingName.Contains("Haftalık"))?.Amount 
                                           ?? g.FirstOrDefault(x => x.PricingID == 2)?.Amount ?? 0,
                            MonthlyAmount = g.FirstOrDefault(x => x.PricingName != null && x.PricingName.Contains("Aylık"))?.Amount 
                                            ?? g.FirstOrDefault(x => x.PricingID == 3)?.Amount ?? 0
                        })
                        .Take(5)
                        .ToList();

                    return View(grouped ?? new List<ResultCarPricingListWithModelDto>());
                }
            }
            catch { }

            return View(new List<ResultCarPricingListWithModelDto>());
        }
    }
}
