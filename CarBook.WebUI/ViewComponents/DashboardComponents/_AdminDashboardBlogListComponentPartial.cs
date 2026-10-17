using CarBook.Dto.BlogDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.ViewComponents.DashboardComponents
{
    public class _AdminDashboardBlogListComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _AdminDashboardBlogListComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var responseMessage = await client.GetAsync("https://localhost:7046/api/Blogs/GetAllBlogsWithAuthorsList");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultAllBlogsWithAuthorDto>>(jsonData);
                    var recentValues = values?.TakeLast(5).Reverse().ToList();
                    return View(recentValues ?? new List<ResultAllBlogsWithAuthorDto>());
                }
            }
            catch { }

            return View(new List<ResultAllBlogsWithAuthorDto>());
        }
    }
}
