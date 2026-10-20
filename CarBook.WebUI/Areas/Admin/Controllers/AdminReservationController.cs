using CarBook.Dto.ReservationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin/AdminReservation")]
    public class AdminReservationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminReservationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7046/api/Reservations");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultReservationDto>>(jsonData);
                return View(values);
            }
            return View(new List<ResultReservationDto>());
        }

        [Route("DeleteReservation/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7046/api/Reservations/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["AdminResSuccess"] = "Rezervasyon kaydı başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }

        [Route("ChangeStatus/{id}/{status}")]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7046/api/Reservations/ChangeStatus/{id}/{status}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["AdminResSuccess"] = $"Rezervasyon durumu '{status}' olarak güncellendi.";
            }
            return RedirectToAction("Index");
        }
    }
}
