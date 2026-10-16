namespace CarBook.Dto.CarPricingDtos
{
    public class ResultCarPricingGroupedDto
    {
        public int CarID { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string CoverImageUrl { get; set; }
        public Dictionary<string, decimal> Prices { get; set; } = new();
    }
}
