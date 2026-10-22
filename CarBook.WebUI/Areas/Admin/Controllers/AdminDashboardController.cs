using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/AdminDashboard")]
    [Route("Admin/Dashboard")]
    public class AdminDashboardController : Controller
    {
        [Route("Index")]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
