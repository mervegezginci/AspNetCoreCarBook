using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CarBook.Dto.RentACarDtos;

namespace CarBook.WebUI.Controllers
{
    public class RentACarListController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RentACarListController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int? id, int? locationID)
        {
            var loc = id ?? locationID;
            if (loc == null && TempData["locationID"] != null)
            {
                if (int.TryParse(TempData["locationID"]?.ToString(), out var parsed))
                {
                    loc = parsed;
                }
            }

            TempData.Keep("bookpickdate");
            TempData.Keep("bookoffdate");
            TempData.Keep("timepick");
            TempData.Keep("timeoff");
            TempData.Keep("locationID");
            TempData.Keep("dropOffLocationID");

            ViewBag.locationID = loc;

            var client = _httpClientFactory.CreateClient();
            var apiUrl = loc.HasValue && loc.Value > 0
                ? $"https://localhost:7046/api/RentACars?locationID={loc.Value}&available=true"
                : $"https://localhost:7046/api/RentACars?locationID=0&available=true";

            var responseMessage = await client.GetAsync(apiUrl);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<FilterRentACarDto>>(jsonData);
                return View(values ?? new List<FilterRentACarDto>());
            }
            return View(new List<FilterRentACarDto>());
        }
    }
}
