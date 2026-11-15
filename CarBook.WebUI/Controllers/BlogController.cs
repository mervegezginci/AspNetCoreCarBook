using CarBook.Dto.BlogDtos;
using CarBook.Dto.CommentDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CarBook.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BlogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.v1 = "Bloglar";
            ViewBag.v2 = "Yazarlarımızın Blogları";

            var client = _httpClientFactory.CreateClient();

            var responseMessage = await client.GetAsync(
                "https://localhost:7046/api/Blogs/GetAllBlogsWithAuthorsList");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                var values = JsonConvert.DeserializeObject<
                    List<ResultAllBlogsWithAuthorDto>>(jsonData);

                return View(values);
            }

            return View(new List<ResultAllBlogsWithAuthorDto>());
        }

        public IActionResult BlogDetail(int id)
        {
            ViewBag.v1 = "Bloglar";
            ViewBag.v2 = "Blog Detayı ve Yorumlar";
            ViewBag.blogid = id;
            return View();
        }

        [HttpGet]
        public IActionResult AddComment(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                return RedirectToAction("BlogDetail", new { id = id.Value });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CreateCommentDto createCommentDto, string? Comment)
        {
            if (string.IsNullOrEmpty(createCommentDto.Description) && !string.IsNullOrEmpty(Comment))
            {
                createCommentDto.Description = Comment;
            }

            createCommentDto.CreatedDate = DateTime.Now;

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createCommentDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7046/api/Comments/CreateCommentWithMediator", stringContent);
            if (!responseMessage.IsSuccessStatusCode)
            {
                await client.PostAsync("https://localhost:7046/api/Comments", stringContent);
            }

            return RedirectToAction("BlogDetail", "Blog", new { id = createCommentDto.BlogID });
        }

    }
}