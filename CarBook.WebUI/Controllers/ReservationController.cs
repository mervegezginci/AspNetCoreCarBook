using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using CarBook.Dto.LocationDtos;
using CarBook.Dto.ReservationDtos;
using CarBook.Dto.CarDtos;
using CarBook.Dto.BrandDtos;
using CarBook.Dto.CarPricingDtos;

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

            decimal dailyPrice = 0;

            if (id > 0)
            {
                // 1. Araç Bilgilerini Çek
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

                // 2. Aracın Günlük Fiyatını Çek
                try
                {
                    var priceRes = await client.GetAsync("https://localhost:7046/api/CarPricings/GetCarPricingWithTimePeriodList");
                    if (priceRes.IsSuccessStatusCode)
                    {
                        var priceJson = await priceRes.Content.ReadAsStringAsync();
                        var prices = JsonConvert.DeserializeObject<List<ResultCarPricingListWithModelDto>>(priceJson);
                        var carPrice = prices?.FirstOrDefault(x => x.CarID == id);
                        if (carPrice != null && carPrice.DailyAmount > 0)
                        {
                            dailyPrice = carPrice.DailyAmount;
                        }
                    }
                }
                catch { }

                if (dailyPrice == 0)
                {
                    try
                    {
                        var priceRes2 = await client.GetAsync("https://localhost:7046/api/CarPricings");
                        if (priceRes2.IsSuccessStatusCode)
                        {
                            var priceJson2 = await priceRes2.Content.ReadAsStringAsync();
                            var prices2 = JsonConvert.DeserializeObject<List<ResultCarPricingWithCarDto>>(priceJson2);
                            var daily = prices2?.FirstOrDefault(x => x.CarID == id && (x.PricingName.Contains("Günlük") || x.PricingID == 1));
                            if (daily != null)
                            {
                                dailyPrice = daily.Amount;
                            }
                        }
                    }
                    catch { }
                }
            }

            ViewBag.DailyPrice = dailyPrice;
            ViewBag.PickDate = TempData["bookpickdate"]?.ToString() ?? "";
            ViewBag.DropDate = TempData["bookoffdate"]?.ToString() ?? "";

            // Lokasyonları Çek
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
        public async Task<IActionResult> Index(CreateReservationDto createReservationDto, string? PickUpDate, string? DropOffDate, string? TotalAmountCalculated)
        {
            var client = _httpClientFactory.CreateClient();

            // Tarih ve hesaplanan tutar bilgilerini Description alanına otomatik ekle
            if (!string.IsNullOrEmpty(PickUpDate) || !string.IsNullOrEmpty(DropOffDate))
            {
                string datesSummary = $"[Kiralama Bilgisi: Alış: {PickUpDate}, Teslim: {DropOffDate}, Tutar: {TotalAmountCalculated} ₺]";
                if (string.IsNullOrEmpty(createReservationDto.Description))
                {
                    createReservationDto.Description = datesSummary;
                }
                else
                {
                    createReservationDto.Description = $"{datesSummary} - Not: {createReservationDto.Description}";
                }
            }

            var jsonData = JsonConvert.SerializeObject(createReservationDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7046/api/Reservations", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessCustomer"] = $"{createReservationDto.Name} {createReservationDto.Surname}";
                TempData["SuccessPhone"] = createReservationDto.Phone;
                TempData["SuccessEmail"] = createReservationDto.Email;
                TempData["SuccessDates"] = $"{PickUpDate} - {DropOffDate}";
                TempData["SuccessTotal"] = TotalAmountCalculated;
                TempData["SuccessCarID"] = createReservationDto.CarID.ToString();
                TempData["SuccessMessage"] = "Rezervasyon talebiniz başarıyla oluşturuldu!";
                return RedirectToAction("ReservationSuccess");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReservationSuccess()
        {
            ViewBag.v1 = "Rezervasyon";
            ViewBag.v2 = "Rezervasyon Onayı";

            if (TempData["SuccessCarID"] != null && int.TryParse(TempData["SuccessCarID"]?.ToString(), out int carId))
            {
                var client = _httpClientFactory.CreateClient();
                try
                {
                    var carRes = await client.GetAsync($"https://localhost:7046/api/Cars/{carId}");
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

            return View();
        }

        [HttpGet]
        public IActionResult MyReservations()
        {
            ViewBag.v1 = "Rezervasyon";
            ViewBag.v2 = "Rezervasyonlarım & Durum Sorgulama";
            return View(new List<ResultReservationDto>());
        }

        [HttpPost]
        public async Task<IActionResult> MyReservations(string phone)
        {
            ViewBag.v1 = "Rezervasyon";
            ViewBag.v2 = "Rezervasyonlarım & Durum Sorgulama";
            ViewBag.SearchPhone = phone;

            if (string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.Error = "Lütfen telefon numaranızı giriniz.";
                return View(new List<ResultReservationDto>());
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7046/api/Reservations/GetByPhone/{phone}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ResultReservationDto>>(json);
                if (list == null || !list.Any())
                {
                    ViewBag.Warning = "Bu telefon numarasına ait aktif rezervasyon bulunamadı.";
                }
                return View(list ?? new List<ResultReservationDto>());
            }

            ViewBag.Error = "Rezervasyon bilgileri alınamadı.";
            return View(new List<ResultReservationDto>());
        }

        [HttpGet]
        public async Task<IActionResult> CancelRequest(int id, string phone)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7046/api/Reservations/ChangeStatus/{id}/İptal Talebi Alındı");
            if (response.IsSuccessStatusCode)
            {
                TempData["CancelSuccess"] = $"#REZ-{id} numaralı rezervasyonunuz için iptal talebi oluşturuldu. Yetkilimiz en kısa sürede inceleyecektir.";
            }
            else
            {
                TempData["CancelError"] = "İptal talebi oluşturulurken bir hata oluştu.";
            }

            ViewBag.v1 = "Rezervasyon";
            ViewBag.v2 = "Rezervasyonlarım & Durum Sorgulama";
            ViewBag.SearchPhone = phone;

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var listRes = await client.GetAsync($"https://localhost:7046/api/Reservations/GetByPhone/{phone}");
                if (listRes.IsSuccessStatusCode)
                {
                    var json = await listRes.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<ResultReservationDto>>(json);
                    return View("MyReservations", list ?? new List<ResultReservationDto>());
                }
            }

            return View("MyReservations", new List<ResultReservationDto>());
        }
    }
}
