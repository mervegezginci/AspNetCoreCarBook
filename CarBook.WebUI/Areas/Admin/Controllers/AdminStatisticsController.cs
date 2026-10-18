using CarBook.Dto.StatisticsDtos;
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
            var random = new Random();
            var client = _httpClientFactory.CreateClient();

            // Default random percentages
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

            #region 1. Toplam Araç Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCount");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.v = val?.carCount ?? 0;
                }
            }
            catch { ViewBag.v = 0; }
            #endregion

            #region 2. Lokasyon Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetLocationCount");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.locationCount = val?.locationCount ?? 0;
                }
            }
            catch { ViewBag.locationCount = 0; }
            #endregion

            #region 3. Yazar Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetAuthorCount");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.authorCount = val?.authorCount ?? 0;
                }
            }
            catch { ViewBag.authorCount = 0; }
            #endregion

            #region 4. Blog Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetBlogCount");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.blogCount = val?.blogCount ?? 0;
                }
            }
            catch { ViewBag.blogCount = 0; }
            #endregion

            #region 5. Marka Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetBrandCount");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.brandCount = val?.brandCount ?? 0;
                }
            }
            catch { ViewBag.brandCount = 0; }
            #endregion

            #region 6. Ortalama Günlük Fiyat
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetAvgRentPriceForDaily");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.avgRentPriceForDaily = val?.avgPriceForDaily.ToString("0.00") ?? "0.00";
                }
            }
            catch { ViewBag.avgRentPriceForDaily = "0.00"; }
            #endregion

            #region 7. Ortalama Haftalık Fiyat
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetAvgRentPriceForWeekly");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.avgRentPriceForWeekly = val?.avgRentPriceForWeekl.ToString("0.00") ?? "0.00";
                }
            }
            catch { ViewBag.avgRentPriceForWeekly = "0.00"; }
            #endregion

            #region 8. Ortalama Aylık Fiyat
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetAvgRentPriceForMonthly");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.avgRentPriceForMonthly = val?.avgRentPriceForMonthly.ToString("0.00") ?? "0.00";
                }
            }
            catch { ViewBag.avgRentPriceForMonthly = "0.00"; }
            #endregion

            #region 9. Otomatik Vites Araç Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCountByTranmissionIsAuto");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.carCountByTranmissionIsAuto = val?.carCountByTranmissionIsAuto ?? 0;
                }
            }
            catch { ViewBag.carCountByTranmissionIsAuto = 0; }
            #endregion

            #region 10. En Fazla Araçlı Marka
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetBrandNameByMaxCar");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.brandNameByMaxCar = val?.brandNameByMaxCar ?? "-";
                }
            }
            catch { ViewBag.brandNameByMaxCar = "-"; }
            #endregion

            #region 11. En Çok Yorum Alan Blog
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetBlogTitleByMaxBlogComment");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.blogTitleByMaxBlogComment = val?.blogTitleByMaxBlogComment ?? "-";
                }
            }
            catch { ViewBag.blogTitleByMaxBlogComment = "-"; }
            #endregion

            #region 12. Km'si 1000'den Az Araçlar
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCountByKmSmallerThen1000");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.carCountByKmSmallerThen1000 = val?.carCountByKmSmallerThen1000 ?? 0;
                }
            }
            catch { ViewBag.carCountByKmSmallerThen1000 = 0; }
            #endregion

            #region 13. Benzinli veya Dizel Araç Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCountByFuelGasolineOrDiesel");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.carCountByFuelGasolineOrDiesel = val?.carCountByFuelGasolineOrDiesel ?? 0;
                }
            }
            catch { ViewBag.carCountByFuelGasolineOrDiesel = 0; }
            #endregion

            #region 14. Elektrikli Araç Sayısı
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCountByFuelElectric");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.carCountByFuelElectric = val?.carCountByFuelElectric ?? 0;
                }
            }
            catch { ViewBag.carCountByFuelElectric = 0; }
            #endregion

            #region 15. En Yüksek Fiyatlı Araç
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarBrandAndModelByRentPriceDailyMax");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.carBrandAndModelByRentPriceDailyMax = val?.carBrandAndModelByRentPriceDailyMax ?? "-";
                }
            }
            catch { ViewBag.carBrandAndModelByRentPriceDailyMax = "-"; }
            #endregion

            #region 16. En Uygun Fiyatlı Araç
            try
            {
                var res = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarBrandAndModelByRentPriceDailyMin");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var val = JsonConvert.DeserializeObject<ResultStatisticsDto>(json);
                    ViewBag.carBrandAndModelByRentPriceDailyMin = val?.carBrandAndModelByRentPriceDailyMin ?? "-";
                }
            }
            catch { ViewBag.carBrandAndModelByRentPriceDailyMin = "-"; }
            #endregion

            return View();
        }
    }
}
