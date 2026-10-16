namespace CarBook.Dto.CarFeatureDtos
{
    public class CreateCarFeatureDto
    {
        public int FeatureID { get; set; }
        public bool Available { get; set; }
        public int CarID { get; set; }
    }
}
