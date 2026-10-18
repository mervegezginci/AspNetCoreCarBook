using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CarBook.Dto.StatisticsDtos;

namespace CarBook.WebUI.ViewComponents.DefaultViewComponents
{
    public class _DefaultStatisticsComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public _DefaultStatisticsComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();

            #region İstatistik1 - Toplam Araç Sayısı
            try
            {
                var responseMessage = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCount");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<ResultStatisticsDto>(jsonData);
                    ViewBag.carCount = values?.carCount ?? 0;
                }
            }
            catch
            {
                ViewBag.carCount = 0;
            }
            #endregion

            #region İstatistik2 - Lokasyon Sayısı
            try
            {
                var responseMessage2 = await client.GetAsync("https://localhost:7046/api/Statistics/GetLocationCount");
                if (responseMessage2.IsSuccessStatusCode)
                {
                    var jsonData2 = await responseMessage2.Content.ReadAsStringAsync();
                    var values2 = JsonConvert.DeserializeObject<ResultStatisticsDto>(jsonData2);
                    ViewBag.locationCount = values2?.locationCount ?? 0;
                }
            }
            catch
            {
                ViewBag.locationCount = 0;
            }
            #endregion

            #region İstatistik3 - Marka Sayısı
            try
            {
                var responseMessage3 = await client.GetAsync("https://localhost:7046/api/Statistics/GetBrandCount");
                if (responseMessage3.IsSuccessStatusCode)
                {
                    var jsonData3 = await responseMessage3.Content.ReadAsStringAsync();
                    var values3 = JsonConvert.DeserializeObject<ResultStatisticsDto>(jsonData3);
                    ViewBag.brandCount = values3?.brandCount ?? 0;
                }
            }
            catch
            {
                ViewBag.brandCount = 0;
            }
            #endregion

            #region İstatistik4 - Elektrikli Araç Sayısı
            try
            {
                var responseMessage4 = await client.GetAsync("https://localhost:7046/api/Statistics/GetCarCountByFuelElectric");
                if (responseMessage4.IsSuccessStatusCode)
                {
                    var jsonData4 = await responseMessage4.Content.ReadAsStringAsync();
                    var values4 = JsonConvert.DeserializeObject<ResultStatisticsDto>(jsonData4);
                    ViewBag.carCountByFuelElectric = values4?.carCountByFuelElectric ?? 0;
                }
            }
            catch
            {
                ViewBag.carCountByFuelElectric = 0;
            }
            #endregion

            return View();
        }
    }
}
