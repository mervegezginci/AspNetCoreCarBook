using CarBook.Dto.AppUserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin/Profile")]
    [Route("Admin/AdminProfile")]
    public class AdminProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("Index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst("Username")?.Value;

            string requestUrl;
            if (int.TryParse(userIdStr, out var userId) && userId > 0)
            {
                requestUrl = $"https://localhost:7046/api/AppUsers/{userId}";
            }
            else if (!string.IsNullOrEmpty(username))
            {
                requestUrl = $"https://localhost:7046/api/AppUsers/GetByUsername/{username}";
            }
            else
            {
                requestUrl = "https://localhost:7046/api/AppUsers/GetFirst";
            }

            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<ResultAppUserDto>(json);
                return View(user);
            }

            // Fallback eğer API'den kullanıcı dönmezse:
            var fallback = new ResultAppUserDto
            {
                AppUserId = 2,
                Username = username ?? "admin",
                Name = "CarBook",
                Surname = "Admin",
                Email = "admin@carbook.com",
                RoleName = "Admin"
            };
            return View(fallback);
        }

        [HttpPost]
        [Route("Index")]
        [Route("")]
        public async Task<IActionResult> Index(UpdateAppUserDto updateAppUserDto)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(updateAppUserDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("https://localhost:7046/api/AppUsers", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                return RedirectToAction("Index", "AdminProfile", new { area = "Admin" });
            }

            TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu.";
            return RedirectToAction("Index", "AdminProfile", new { area = "Admin" });
        }
    }
}
