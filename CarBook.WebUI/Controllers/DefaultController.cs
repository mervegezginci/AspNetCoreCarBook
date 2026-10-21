using CarBook.Dto.LocationDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace CarBook.WebUI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DefaultController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7046/api/Locations");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultLocationDto>>(jsonData);
                List<SelectListItem> locationValues = (from x in values
                                                       select new SelectListItem
                                                       {
                                                           Text = x.Name,
                                                           Value = x.LocationID.ToString()
                                                       }).ToList();
                ViewBag.LocationValues = locationValues;
                ViewBag.v = locationValues;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(string locationID, string dropOffLocationID, string pickDate, string offDate, string timePick, string book_pick_date, string book_off_date, string time_pick, string time_off)
        {
            var pDate = !string.IsNullOrEmpty(pickDate) ? pickDate : book_pick_date;
            var oDate = !string.IsNullOrEmpty(offDate) ? offDate : book_off_date;
            var tPick = !string.IsNullOrEmpty(timePick) ? timePick : time_pick;
            var tOff = !string.IsNullOrEmpty(time_off) ? time_off : "";

            TempData["bookpickdate"] = pDate;
            TempData["bookoffdate"] = oDate;
            TempData["timepick"] = tPick;
            TempData["timeoff"] = tOff;
            TempData["locationID"] = locationID;
            TempData["dropOffLocationID"] = dropOffLocationID;

            return RedirectToAction("Index", "RentACarList", new { id = locationID, locationID = locationID });
        }
    }
}
