using CarBook.Dto.LocationDtos;
using CarBook.Dto.RentACarDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.Controllers
{
    public class RentACarListController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RentACarListController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int? id, int? locationID, int? dropOffLocationID)
        {
            var loc = id ?? locationID;
            if (loc == null && TempData["locationID"] != null)
            {
                if (int.TryParse(TempData["locationID"]?.ToString(), out var parsed))
                {
                    loc = parsed;
                }
            }

            var dropLoc = dropOffLocationID;
            if (dropLoc == null && TempData["dropOffLocationID"] != null)
            {
                if (int.TryParse(TempData["dropOffLocationID"]?.ToString(), out var dropParsed))
                {
                    dropLoc = dropParsed;
                }
            }

            TempData.Keep("bookpickdate");
            TempData.Keep("bookoffdate");
            TempData.Keep("timepick");
            TempData.Keep("timeoff");
            TempData.Keep("locationID");
            TempData.Keep("dropOffLocationID");

            ViewBag.locationID = loc;
            ViewBag.dropOffLocationID = dropLoc;
            ViewBag.v1 = "Araçlarımız";
            ViewBag.v2 = "Aracınızı Seçiniz";

            var client = _httpClientFactory.CreateClient();

            if (loc.HasValue && loc.Value > 0)
            {
                var locResponse = await client.GetAsync($"https://localhost:7046/api/Locations/{loc.Value}");
                if (locResponse.IsSuccessStatusCode)
                {
                    var locJson = await locResponse.Content.ReadAsStringAsync();
                    var locData = JsonConvert.DeserializeObject<ResultLocationDto>(locJson);
                    if (locData != null && !string.IsNullOrEmpty(locData.Name))
                    {
                        ViewBag.locationName = locData.Name;
                    }
                }
            }

            if (dropLoc.HasValue && dropLoc.Value > 0)
            {
                var dropResponse = await client.GetAsync($"https://localhost:7046/api/Locations/{dropLoc.Value}");
                if (dropResponse.IsSuccessStatusCode)
                {
                    var dropJson = await dropResponse.Content.ReadAsStringAsync();
                    var dropData = JsonConvert.DeserializeObject<ResultLocationDto>(dropJson);
                    if (dropData != null && !string.IsNullOrEmpty(dropData.Name))
                    {
                        ViewBag.dropOffLocationName = dropData.Name;
                    }
                }
            }

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
