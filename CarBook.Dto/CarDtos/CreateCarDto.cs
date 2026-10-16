using System.ComponentModel.DataAnnotations;

namespace CarBook.Dto.CarDtos
{
    public class CreateCarDto
    {
        [Required(ErrorMessage = "Lütfen bir marka seçiniz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli bir marka seçiniz.")]
        public int BrandID { get; set; }

        [Required(ErrorMessage = "Model bilgisi boş geçilemez.")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kapak fotoğrafı linki boş geçilemez.")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kilometre bilgisi boş geçilemez.")]
        [Range(0, int.MaxValue, ErrorMessage = "Km 0 veya daha büyük olmalıdır.")]
        public int Km { get; set; }

        [Required(ErrorMessage = "Vites türü boş geçilemez.")]
        public string Transmission { get; set; } = string.Empty;

        [Required(ErrorMessage = "Koltuk sayısı boş geçilemez.")]
        [Range(1, 100, ErrorMessage = "Koltuk sayısı en az 1 olmalıdır.")]
        public byte Seat { get; set; }

        [Required(ErrorMessage = "Bagaj sayısı boş geçilemez.")]
        public byte Luggage { get; set; }

        [Required(ErrorMessage = "Yakıt türü boş geçilemez.")]
        public string Fuel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Büyük kapak fotoğrafı linki boş geçilemez.")]
        public string BigImageUrl { get; set; } = string.Empty;
    }
}
