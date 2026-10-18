using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using CarBook.Dto.CarDtos;
using CarBook.Dto.BrandDtos;

namespace CarBook.WebUI.ViewComponents.CarDetailViewComponents
{
	public class _CarDetailMainCarFeatureComponentPartial : ViewComponent
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public _CarDetailMainCarFeatureComponentPartial(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IViewComponentResult> InvokeAsync(int id)
		{
			ViewBag.carid = id;
			var client = _httpClientFactory.CreateClient();
			var resposenMessage = await client.GetAsync($"https://localhost:7046/api/Cars/{id}");
			if (resposenMessage.IsSuccessStatusCode)
			{
				var jsonData = await resposenMessage.Content.ReadAsStringAsync();
				var values = JsonConvert.DeserializeObject<ResultCarWithBrandsDtos>(jsonData);

				if (values != null && string.IsNullOrEmpty(values.BrandName) && values.BrandID > 0)
				{
					try
					{
						var brandRes = await client.GetAsync($"https://localhost:7046/api/Brands/{values.BrandID}");
						if (brandRes.IsSuccessStatusCode)
						{
							var bJson = await brandRes.Content.ReadAsStringAsync();
							var bVal = JsonConvert.DeserializeObject<ResultBrandDto>(bJson);
							if (bVal != null) values.BrandName = bVal.name;
						}
					}
					catch { }
				}

				return View(values ?? new ResultCarWithBrandsDtos());
			}
			return View(new ResultCarWithBrandsDtos());
		}
	}
}
