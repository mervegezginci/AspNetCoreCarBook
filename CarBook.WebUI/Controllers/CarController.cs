using CarBook.Dto.CarDtos;
using CarBook.Dto.LocationDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.Controllers
{
    public class CarController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CarController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int? locationID, int? dropOffLocationID, string? pickDate, string? offDate, string? timePick)
        {
            var loc = locationID;
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

            ViewBag.v1 = "Araçlarımız";
            ViewBag.v2 = "Aracınızı Seçiniz";
            ViewBag.locationID = loc;
            ViewBag.dropOffLocationID = dropLoc;
            ViewBag.pickDate = pickDate;
            ViewBag.offDate = offDate;

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

            var responseMessage = await client.GetAsync(
           "https://localhost:7046/api/Cars/GetCarWithBrand"
       );

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                var values =
                    JsonConvert.DeserializeObject<List<ResultCarWithBrandsDto>>(jsonData);

                return View(values);
            }

            return View();
        }

        public async Task<IActionResult> CarDetail(int id)
        {
            ViewBag.v1 = "Araç Detayları";
            ViewBag.v2 = "Aracın Teknik Aksesuar ve Özellikleri";
            ViewBag.carid = id;
            return View();
        }
    }
}