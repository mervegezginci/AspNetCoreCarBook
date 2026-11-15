using CarBook.Dto.FooterAddressDtos;
using CarBook.Dto.SocialMediaDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarBook.WebUI.ViewComponents.UILayoutViewComponents
{
    public class _FooterUILayoutComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _FooterUILayoutComponentPartial(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();

            ResultFooterAddressDto? footerAddress = null;

            // FooterAddress Çek
            try
            {
                var responseMessage = await client.GetAsync("https://localhost:7046/api/FooterAddresses");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultFooterAddressDto>>(jsonData);
                    footerAddress = values?.FirstOrDefault();
                }
            }
            catch { }

            // SocialMedias Çek
            try
            {
                var socialResponse = await client.GetAsync("https://localhost:7046/api/SocialMedias");
                if (socialResponse.IsSuccessStatusCode)
                {
                    var socialJson = await socialResponse.Content.ReadAsStringAsync();
                    var socialValues = JsonConvert.DeserializeObject<List<ResultSocialMediaDto>>(socialJson);
                    ViewBag.SocialMedias = socialValues ?? new List<ResultSocialMediaDto>();
                }
                else
                {
                    ViewBag.SocialMedias = new List<ResultSocialMediaDto>();
                }
            }
            catch
            {
                ViewBag.SocialMedias = new List<ResultSocialMediaDto>();
            }

            return View(footerAddress ?? new ResultFooterAddressDto());
        }
    }
}