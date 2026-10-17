using CarBook.Dto.AuthorDtos;
using CarBook.Dto.BlogDtos;
using CarBook.Dto.BrandDtos;
using CarBook.Dto.CarDtos;
using CarBook.Dto.CarPricingDtos;
using CarBook.Dto.CommentDtos;
using CarBook.Dto.LocationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin/AdminStatistics")]
    public class AdminStatisticsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminStatisticsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("Index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var random = new Random();

            // Random Yüzdelik Değerler
            ViewBag.v1 = random.Next(10, 95);
            ViewBag.locationCountRandom = random.Next(10, 95);
            ViewBag.authorCountRandom = random.Next(10, 95);
            ViewBag.blogCountRandom = random.Next(10, 95);
            ViewBag.brandCountRandom = random.Next(10, 95);
            ViewBag.avgRentPriceForDailyRandom = random.Next(10, 95);
            ViewBag.avgRentPriceForWeeklyRandom = random.Next(10, 95);
            ViewBag.avgRentPriceForMonthlyRandom = random.Next(10, 95);
            ViewBag.carCountByTranmissionIsAutoRandom = random.Next(10, 95);
            ViewBag.brandNameByMaxCarRandom = random.Next(10, 95);
            ViewBag.blogTitleByMaxBlogCommentRandom = random.Next(10, 95);
            ViewBag.carCountByKmSmallerThen1000Random = random.Next(10, 95);
            ViewBag.carCountByFuelGasolineOrDieselRandom = random.Next(10, 95);
            ViewBag.carCountByFuelElectricRandom = random.Next(10, 95);
            ViewBag.carBrandAndModelByRentPriceDailyMaxRandom = random.Next(10, 95);
            ViewBag.carBrandAndModelByRentPriceDailyMinRandom = random.Next(10, 95);

            // 1. Araçlar Listesi
            List<ResultLast5CarsWithBrandsDto> cars = new();
            try
            {
                var carResponse = await client.GetAsync("https://localhost:7046/api/Cars/GetCarWithBrand");
                if (!carResponse.IsSuccessStatusCode)
                {
                    carResponse = await client.GetAsync("https://localhost:7046/api/Cars");
                }
                if (carResponse.IsSuccessStatusCode)
                {
                    var data = await carResponse.Content.ReadAsStringAsync();
                    cars = JsonConvert.DeserializeObject<List<ResultLast5CarsWithBrandsDto>>(data) ?? new();
                }
            }
            catch { }

            // 2. Lokasyonlar
            List<ResultLocationDto> locations = new();
            try
            {
                var locResponse = await client.GetAsync("https://localhost:7046/api/Locations");
                if (locResponse.IsSuccessStatusCode)
                {
                    var data = await locResponse.Content.ReadAsStringAsync();
                    locations = JsonConvert.DeserializeObject<List<ResultLocationDto>>(data) ?? new();
                }
            }
            catch { }

            // 3. Yazarlar
            List<ResultAuthorDto> authors = new();
            try
            {
                var authorResponse = await client.GetAsync("https://localhost:7046/api/Authors");
                if (authorResponse.IsSuccessStatusCode)
                {
                    var data = await authorResponse.Content.ReadAsStringAsync();
                    authors = JsonConvert.DeserializeObject<List<ResultAuthorDto>>(data) ?? new();
                }
            }
            catch { }

            // 4. Bloglar
            List<ResultAllBlogsWithAuthorDto> blogs = new();
            try
            {
                var blogResponse = await client.GetAsync("https://localhost:7046/api/Blogs/GetAllBlogsWithAuthorsList");
                if (!blogResponse.IsSuccessStatusCode)
                {
                    blogResponse = await client.GetAsync("https://localhost:7046/api/Blogs");
                }
                if (blogResponse.IsSuccessStatusCode)
                {
                    var data = await blogResponse.Content.ReadAsStringAsync();
                    blogs = JsonConvert.DeserializeObject<List<ResultAllBlogsWithAuthorDto>>(data) ?? new();
                }
            }
            catch { }

            // 5. Markalar
            List<ResultBrandDto> brands = new();
            try
            {
                var brandResponse = await client.GetAsync("https://localhost:7046/api/Brands");
                if (brandResponse.IsSuccessStatusCode)
                {
                    var data = await brandResponse.Content.ReadAsStringAsync();
                    brands = JsonConvert.DeserializeObject<List<ResultBrandDto>>(data) ?? new();
                }
            }
            catch { }

            // 6. Fiyatlandırmalar
            List<ResultCarPricingWithCarDto> carPricings = new();
            try
            {
                var pricingResponse = await client.GetAsync("https://localhost:7046/api/CarPricings");
                if (pricingResponse.IsSuccessStatusCode)
                {
                    var data = await pricingResponse.Content.ReadAsStringAsync();
                    carPricings = JsonConvert.DeserializeObject<List<ResultCarPricingWithCarDto>>(data) ?? new();
                }
            }
            catch { }

            // 7. Yorumlar
            List<ResultCommentDto> comments = new();
            try
            {
                var commentResponse = await client.GetAsync("https://localhost:7046/api/Comments");
                if (commentResponse.IsSuccessStatusCode)
                {
                    var data = await commentResponse.Content.ReadAsStringAsync();
                    comments = JsonConvert.DeserializeObject<List<ResultCommentDto>>(data) ?? new();
                }
            }
            catch { }

            // --- DEĞERLERİN ATANMASI ---
            // Başlık 1
            ViewBag.v = cars.Count;
            ViewBag.locationCount = locations.Count;
            ViewBag.authorCount = authors.Count;
            ViewBag.blogCount = blogs.Count;

            // Başlık 2
            ViewBag.brandCount = brands.Count;

            var dailyPricings = carPricings.Where(x => (x.PricingName != null && x.PricingName.Contains("Günlük")) || x.PricingID == 1).ToList();
            var weeklyPricings = carPricings.Where(x => (x.PricingName != null && x.PricingName.Contains("Haftalık")) || x.PricingID == 2).ToList();
            var monthlyPricings = carPricings.Where(x => (x.PricingName != null && x.PricingName.Contains("Aylık")) || x.PricingID == 3).ToList();

            ViewBag.avgRentPriceForDaily = dailyPricings.Any() ? dailyPricings.Average(x => x.Amount).ToString("N2") : (carPricings.Any() ? carPricings.Average(x => x.Amount).ToString("N2") : "0.00");
            ViewBag.avgRentPriceForWeekly = weeklyPricings.Any() ? weeklyPricings.Average(x => x.Amount).ToString("N2") : (dailyPricings.Any() ? (dailyPricings.Average(x => x.Amount) * 6).ToString("N2") : "0.00");
            ViewBag.avgRentPriceForMonthly = monthlyPricings.Any() ? monthlyPricings.Average(x => x.Amount).ToString("N2") : (dailyPricings.Any() ? (dailyPricings.Average(x => x.Amount) * 24).ToString("N2") : "0.00");

            // Başlık 3
            ViewBag.carCountByTranmissionIsAuto = cars.Count(x => !string.IsNullOrEmpty(x.Transmission) && (x.Transmission.ToLower().Contains("oto") || x.Transmission.ToLower().Contains("auto")));

            var maxCarBrandGroup = cars.GroupBy(x => x.BrandName).OrderByDescending(g => g.Count()).FirstOrDefault();
            ViewBag.brandNameByMaxCar = maxCarBrandGroup != null && !string.IsNullOrEmpty(maxCarBrandGroup.Key) ? maxCarBrandGroup.Key : (brands.FirstOrDefault()?.name ?? "-");

            var maxCommentBlogGroup = comments.GroupBy(x => x.BlogID).OrderByDescending(g => g.Count()).FirstOrDefault();
            if (maxCommentBlogGroup != null)
            {
                var topBlog = blogs.FirstOrDefault(x => x.BlogID == maxCommentBlogGroup.Key);
                ViewBag.blogTitleByMaxBlogComment = topBlog != null ? topBlog.Title : (comments.FirstOrDefault()?.BlogTitle ?? "-");
            }
            else
            {
                ViewBag.blogTitleByMaxBlogComment = blogs.FirstOrDefault()?.Title ?? "-";
            }

            ViewBag.carCountByKmSmallerThen1000 = cars.Count(x => x.Km < 10000);

            // Başlık 4
            ViewBag.carCountByFuelGasolineOrDiesel = cars.Count(x => !string.IsNullOrEmpty(x.Fuel) && (x.Fuel.ToLower().Contains("benzin") || x.Fuel.ToLower().Contains("dizel") || x.Fuel.ToLower().Contains("diesel")));
            ViewBag.carCountByFuelElectric = cars.Count(x => !string.IsNullOrEmpty(x.Fuel) && (x.Fuel.ToLower().Contains("elektrik") || x.Fuel.ToLower().Contains("electric")));

            var sortedDaily = dailyPricings.Any() ? dailyPricings : carPricings;
            var maxDaily = sortedDaily.OrderByDescending(x => x.Amount).FirstOrDefault();
            var minDaily = sortedDaily.OrderBy(x => x.Amount).FirstOrDefault();

            ViewBag.carBrandAndModelByRentPriceDailyMax = maxDaily != null ? $"{maxDaily.BrandName} {maxDaily.Model}" : "-";
            ViewBag.carBrandAndModelByRentPriceDailyMin = minDaily != null ? $"{minDaily.BrandName} {minDaily.Model}" : "-";

            return View();
        }
    }
}
