using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using CarBook.Dto.LocationDtos;
using CarBook.Dto.ReservationDtos;
using CarBook.Dto.CarDtos;
using CarBook.Dto.BrandDtos;

namespace CarBook.WebUI.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ReservationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            ViewBag.v1 = "Araç Kiralama";
            ViewBag.v2 = "Araç Rezervasyon Formu";
            ViewBag.v3 = id;

            var client = _httpClientFactory.CreateClient();

            if (id > 0)
            {
                try
                {
                    var carRes = await client.GetAsync($"https://localhost:7046/api/Cars/{id}");
                    if (carRes.IsSuccessStatusCode)
                    {
                        var carJson = await carRes.Content.ReadAsStringAsync();
                        var car = JsonConvert.DeserializeObject<ResultCarWithBrandsDtos>(carJson);
                        if (car != null && string.IsNullOrEmpty(car.BrandName) && car.BrandID > 0)
                        {
                            try
                            {
                                var brandRes = await client.GetAsync($"https://localhost:7046/api/Brands/{car.BrandID}");
                                if (brandRes.IsSuccessStatusCode)
                                {
                                    var bJson = await brandRes.Content.ReadAsStringAsync();
                                    var bVal = JsonConvert.DeserializeObject<ResultBrandDto>(bJson);
                                    if (bVal != null) car.BrandName = bVal.name;
                                }
                            }
                            catch { }
                        }
                        ViewBag.SelectedCar = car;
                    }
                }
                catch { }
            }

            try
            {
                var responseMessage = await client.GetAsync("https://localhost:7046/api/Locations");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultLocationDto>>(jsonData);
                    List<SelectListItem> values2 = (from x in values
                                                    select new SelectListItem
                                                    {
                                                        Text = x.Name,
                                                        Value = x.LocationID.ToString()
                                                    }).ToList();
                    ViewBag.v = values2;
                }
                else
                {
                    ViewBag.v = new List<SelectListItem>();
                }
            }
            catch
            {
                ViewBag.v = new List<SelectListItem>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CreateReservationDto createReservationDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createReservationDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7046/api/Reservations", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Default");
            }
            return View();
        }
    }
}
