using CarBook.Dto.CarDtos;
using CarBook.Dto.CarFeatureDtos;
using CarBook.Dto.FeatureDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin/AdminCarFeatureDetail")]
    public class AdminCarFeatureDetailController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminCarFeatureDetailController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("CreateFeatureByCarID/{id?}")]
        [Route("CreateFeatureByCarID")]
        public async Task<IActionResult> CreateFeatureByCarID(int? id, int? carId)
        {
            int targetCarId = id ?? carId ?? 0;
            ViewBag.CarID = targetCarId;

            var client = _httpClientFactory.CreateClient();

            // 1. Arabaları Listele (Dropdown için)
            var carResponse = await client.GetAsync("https://localhost:7046/api/Cars/GetCarWithBrand");
            if (carResponse.IsSuccessStatusCode)
            {
                var jsonCars = await carResponse.Content.ReadAsStringAsync();
                var cars = JsonConvert.DeserializeObject<List<ResultCarWithBrandsDto>>(jsonCars) ?? new List<ResultCarWithBrandsDto>();

                ViewBag.CarList = cars.Select(c => new SelectListItem
                {
                    Text = $"{c.BrandName} - {c.Model} ({c.CarId})",
                    Value = c.CarId.ToString(),
                    Selected = (c.CarId == targetCarId)
                }).ToList();

                if (targetCarId > 0)
                {
                    var selectedCar = cars.FirstOrDefault(x => x.CarId == targetCarId);
                    if (selectedCar != null)
                    {
                        ViewBag.SelectedCarName = $"{selectedCar.BrandName} {selectedCar.Model}";
                    }
                }
            }

            // 2. Mevcut Atanmış Özellikleri Getir
            var assignedFeatureIds = new List<int>();
            if (targetCarId > 0)
            {
                var carFeaturesResponse = await client.GetAsync($"https://localhost:7046/api/CarFeatures?id={targetCarId}");
                if (carFeaturesResponse.IsSuccessStatusCode)
                {
                    var jsonCarFeatures = await carFeaturesResponse.Content.ReadAsStringAsync();
                    var existingFeatures = JsonConvert.DeserializeObject<List<ResultCarFeatureByCarIdDto>>(jsonCarFeatures);
                    if (existingFeatures != null)
                    {
                        assignedFeatureIds = existingFeatures.Where(x => x.Available).Select(x => x.FeatureID).ToList();
                    }
                }
            }
            ViewBag.AssignedFeatureIds = assignedFeatureIds;

            // 3. Tüm Özellikleri Getir
            var featureResponse = await client.GetAsync("https://localhost:7046/api/Features");
            if (featureResponse.IsSuccessStatusCode)
            {
                var jsonFeatures = await featureResponse.Content.ReadAsStringAsync();
                var features = JsonConvert.DeserializeObject<List<ResultFeatureDto>>(jsonFeatures);
                return View(features ?? new List<ResultFeatureDto>());
            }

            return View(new List<ResultFeatureDto>());
        }

        [HttpPost]
        [Route("CreateFeatureByCarID/{id?}")]
        [Route("CreateFeatureByCarID")]
        public async Task<IActionResult> CreateFeatureByCarID(int carId, List<int> selectedFeatures)
        {
            if (carId <= 0 && int.TryParse(Request.Form["CarID"], out int parsedCarId))
            {
                carId = parsedCarId;
            }

            if (carId <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen bir araç seçiniz.";
                return RedirectToAction("CreateFeatureByCarID");
            }

            selectedFeatures ??= new List<int>();

            var client = _httpClientFactory.CreateClient();

            // Tüm özellikleri çekip seçilenleri true, seçilmeyenleri false olarak kaydediyoruz
            var featureResponse = await client.GetAsync("https://localhost:7046/api/Features");
            if (featureResponse.IsSuccessStatusCode)
            {
                var jsonFeatures = await featureResponse.Content.ReadAsStringAsync();
                var allFeatures = JsonConvert.DeserializeObject<List<ResultFeatureDto>>(jsonFeatures) ?? new List<ResultFeatureDto>();

                var carFeaturesToSave = allFeatures.Select(feat => new CreateCarFeatureDto
                {
                    CarID = carId,
                    FeatureID = feat.FeatureID,
                    Available = selectedFeatures.Contains(feat.FeatureID)
                }).ToList();

                var jsonPayload = JsonConvert.SerializeObject(carFeaturesToSave);
                var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var postResponse = await client.PostAsync("https://localhost:7046/api/CarFeatures", stringContent);
                if (postResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Araç özellikleri başarıyla güncellendi.";
                    return RedirectToAction("Index", "AdminCar", new { area = "" });
                }
                else
                {
                    var errorDetail = await postResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Özellikler kaydedilirken bir hata oluştu: {errorDetail}";
                    return RedirectToAction("CreateFeatureByCarID", new { id = carId });
                }
            }

            TempData["ErrorMessage"] = "Özellik listesi alınamadı veya özellikler kaydedilirken bir hata oluştu.";
            return RedirectToAction("CreateFeatureByCarID", new { id = carId });
        }

        [HttpGet]
        [Route("Index/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7046/api/CarFeatures?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCarFeatureByCarIdDto>>(json);
                if (values != null && values.Any())
                {
                    ViewBag.CarID = id;
                    return View(values);
                }
            }
            return RedirectToAction("CreateFeatureByCarID", new { id });
        }
    }
}
