using CarBook.Dto.BlogDtos;
using CarBook.Dto.TagCloudDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("Admin/AdminTagCloud")]
    public class AdminTagCloudController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminTagCloudController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7046/api/TagClouds");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultTagCloudDto>>(jsonData);
                return View(values ?? new List<ResultTagCloudDto>());
            }
            return View(new List<ResultTagCloudDto>());
        }

        [HttpGet]
        [Route("TagCloudListByBlog/{id}")]
        public async Task<IActionResult> TagCloudListByBlog(int id)
        {
            ViewBag.BlogID = id;

            var client = _httpClientFactory.CreateClient();

            var blogResponse = await client.GetAsync($"https://localhost:7046/api/Blogs/{id}");
            if (blogResponse.IsSuccessStatusCode)
            {
                var jsonBlog = await blogResponse.Content.ReadAsStringAsync();
                var blog = JsonConvert.DeserializeObject<GetBlogByIdDto>(jsonBlog);
                if (blog != null)
                {
                    ViewBag.BlogTitle = blog.Title;
                }
            }

            var assignedTags = new List<string>();
            var assignedResponse = await client.GetAsync($"https://localhost:7046/api/TagClouds/GetTagClodByBlogId?id={id}");
            if (assignedResponse.IsSuccessStatusCode)
            {
                var jsonAssigned = await assignedResponse.Content.ReadAsStringAsync();
                var assignedItems = JsonConvert.DeserializeObject<List<ResultTagCloudDto>>(jsonAssigned);
                if (assignedItems != null)
                {
                    assignedTags = assignedItems.Select(x => x.Title.Trim()).ToList();
                }
            }
            ViewBag.AssignedTags = assignedTags;

            var allTags = new List<string>();
            var allResponse = await client.GetAsync("https://localhost:7046/api/TagClouds");
            if (allResponse.IsSuccessStatusCode)
            {
                var jsonAll = await allResponse.Content.ReadAsStringAsync();
                var allItems = JsonConvert.DeserializeObject<List<ResultTagCloudDto>>(jsonAll);
                if (allItems != null)
                {
                    allTags = allItems.Select(x => x.Title.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
            }

            var predefinedTags = new List<string>
            {
                "Araç Kiralama", "Güvenlik", "Sözleşme", "Sürüş", "Uzun Yol",
                "Araç Seçimi", "Konfor", "Seyahat", "Yakıt Tasarrufu", "Ekonomik Sürüş",
                "Tatil", "Rehber", "Otomobil", "Teknoloji", "Trafik", "Kasko", "Yolculuk", "İpuçları"
            };

            var distinctTags = allTags
                .Union(predefinedTags, StringComparer.OrdinalIgnoreCase)
                .Union(assignedTags, StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            return View(distinctTags);
        }

        [HttpPost]
        [Route("TagCloudListByBlog/{id?}")]
        [Route("TagCloudListByBlog")]
        public async Task<IActionResult> TagCloudListByBlog(int blogId, List<string> selectedTags, string? newTags)
        {
            if (blogId <= 0 && int.TryParse(Request.Form["blogId"], out int parsedId))
            {
                blogId = parsedId;
            }

            if (blogId <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir blog seçiniz.";
                return RedirectToAction("Index", "AdminBlog");
            }

            selectedTags ??= new List<string>();

            if (!string.IsNullOrWhiteSpace(newTags))
            {
                var parts = newTags.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var clean = part.Trim();
                    if (!string.IsNullOrEmpty(clean) && !selectedTags.Contains(clean, StringComparer.OrdinalIgnoreCase))
                    {
                        selectedTags.Add(clean);
                    }
                }
            }

            var client = _httpClientFactory.CreateClient();

            var existingResponse = await client.GetAsync($"https://localhost:7046/api/TagClouds/GetTagClodByBlogId?id={blogId}");
            var existingItems = new List<ResultTagCloudDto>();
            if (existingResponse.IsSuccessStatusCode)
            {
                var json = await existingResponse.Content.ReadAsStringAsync();
                existingItems = JsonConvert.DeserializeObject<List<ResultTagCloudDto>>(json) ?? new List<ResultTagCloudDto>();
            }

            var toDelete = existingItems.Where(e => !selectedTags.Contains(e.Title.Trim(), StringComparer.OrdinalIgnoreCase)).ToList();
            foreach (var item in toDelete)
            {
                await client.DeleteAsync($"https://localhost:7046/api/TagClouds?id={item.TagCloudID}");
            }

            var existingTitles = existingItems.Select(e => e.Title.Trim()).ToList();
            var toAdd = selectedTags.Where(s => !existingTitles.Contains(s.Trim(), StringComparer.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            foreach (var title in toAdd)
            {
                var createDto = new CreateTagCloudDto
                {
                    BlogID = blogId,
                    Title = title.Trim()
                };
                var jsonPayload = JsonConvert.SerializeObject(createDto);
                var stringContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                await client.PostAsync("https://localhost:7046/api/TagClouds", stringContent);
            }

            TempData["SuccessMessage"] = "Blog etiketleri başarıyla güncellendi.";
            return RedirectToAction("TagCloudListByBlog", new { id = blogId });
        }

        [HttpGet]
        [Route("CreateTagCloud/{id?}")]
        [Route("CreateTagCloud")]
        public async Task<IActionResult> CreateTagCloud(int? id, int? blogId)
        {
            int targetBlogId = id ?? blogId ?? 0;
            ViewBag.SelectedBlogId = targetBlogId;

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7046/api/Blogs/GetAllBlogsWithAuthorList");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAllBlogsWithAuthorDto>>(jsonData);
                List<SelectListItem> blogValues = (from x in values
                                                    select new SelectListItem
                                                    {
                                                        Text = x.Title,
                                                        Value = x.BlogID.ToString(),
                                                        Selected = (x.BlogID == targetBlogId)
                                                    }).ToList();
                ViewBag.BlogValues = blogValues;
            }
            return View(new CreateTagCloudDto { BlogID = targetBlogId });
        }

        [HttpPost]
        [Route("CreateTagCloud/{id?}")]
        [Route("CreateTagCloud")]
        public async Task<IActionResult> CreateTagCloud(CreateTagCloudDto createTagCloudDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createTagCloudDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("https://localhost:7046/api/TagClouds", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                if (createTagCloudDto.BlogID > 0)
                {
                    return RedirectToAction("TagCloudListByBlog", "AdminTagCloud", new { area = "Admin", id = createTagCloudDto.BlogID });
                }
                return RedirectToAction("Index", "AdminTagCloud", new { area = "Admin" });
            }
            return View(createTagCloudDto);
        }

        [Route("RemoveTagCloud/{id}")]
        public async Task<IActionResult> RemoveTagCloud(int id, int? blogId)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7046/api/TagClouds?id={id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                if (blogId.HasValue && blogId.Value > 0)
                {
                    return RedirectToAction("TagCloudListByBlog", "AdminTagCloud", new { area = "Admin", id = blogId.Value });
                }
                return RedirectToAction("Index", "AdminTagCloud", new { area = "Admin" });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("UpdateTagCloud/{id}")]
        public async Task<IActionResult> UpdateTagCloud(int id)
        {
            var client = _httpClientFactory.CreateClient();

            var blogResponse = await client.GetAsync("https://localhost:7046/api/Blogs/GetAllBlogsWithAuthorList");
            if (blogResponse.IsSuccessStatusCode)
            {
                var jsonBlog = await blogResponse.Content.ReadAsStringAsync();
                var blogList = JsonConvert.DeserializeObject<List<ResultAllBlogsWithAuthorDto>>(jsonBlog);
                List<SelectListItem> blogValues = (from x in blogList
                                                    select new SelectListItem
                                                    {
                                                        Text = x.Title,
                                                        Value = x.BlogID.ToString()
                                                    }).ToList();
                ViewBag.BlogValues = blogValues;
            }

            var responseMessage = await client.GetAsync($"https://localhost:7046/api/TagClouds/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<UpdateTagCloudDto>(jsonData);
                return View(values);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("UpdateTagCloud/{id}")]
        public async Task<IActionResult> UpdateTagCloud(UpdateTagCloudDto updateTagCloudDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateTagCloudDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("https://localhost:7046/api/TagClouds", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "AdminTagCloud", new { area = "Admin" });
            }
            return View(updateTagCloudDto);
        }
    }
}
