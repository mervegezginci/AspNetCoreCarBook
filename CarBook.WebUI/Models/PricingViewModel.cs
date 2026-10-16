using CarBook.Dto.CarPricingDtos;
using CarBook.Dto.PricingDtos;

namespace CarBook.WebUI.Models
{
    public class PricingViewModel
    {
        public List<ResultCarPricingGroupedDto> CarPricings { get; set; } = new();
        public List<ResultPricingDto> PricingTypes { get; set; } = new();
    }
}
