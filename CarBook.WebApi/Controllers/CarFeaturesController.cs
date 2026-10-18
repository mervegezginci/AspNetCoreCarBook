using CarBook.Domain.Entities;
using CarBook.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarBook.WebApi.Controllers
{
    public class CreateCarFeatureDto
    {
        public int CarID { get; set; }
        public int FeatureID { get; set; }
        public bool Available { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CarFeaturesController : ControllerBase
    {
        private readonly CarBookContext _context;

        public CarFeaturesController(CarBookContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarFeaturesByCarId(int id)
        {
            var allFeatures = await _context.Features.ToListAsync();
            var carFeatures = await _context.CarFeatures
                .Where(x => x.CarID == id)
                .ToListAsync();

            var values = allFeatures.Select(f =>
            {
                var existing = carFeatures.FirstOrDefault(cf => cf.FeatureID == f.FeatureId);
                return new
                {
                    CarFeatureId = existing != null ? existing.CarFeatureID : 0,
                    CarID = id,
                    FeatureID = f.FeatureId,
                    FeatureName = f.Name,
                    Available = existing != null && existing.Available
                };
            }).ToList();

            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCarFeatureByCarId([FromBody] List<CreateCarFeatureDto> carFeatures)
        {
            if (carFeatures != null && carFeatures.Any())
            {
                var carId = carFeatures.First().CarID;
                var existing = _context.CarFeatures.Where(x => x.CarID == carId);
                _context.CarFeatures.RemoveRange(existing);

                var newEntities = carFeatures.Select(x => new CarFeature
                {
                    CarID = x.CarID,
                    FeatureID = x.FeatureID,
                    Available = x.Available
                }).ToList();

                await _context.CarFeatures.AddRangeAsync(newEntities);
                await _context.SaveChangesAsync();
                return Ok("Araca ait özellikler başarıyla kaydedildi.");
            }
            return BadRequest("Veri bulunamadı.");
        }

        [HttpGet("CarFeatureChangeAvailableToTrue")]
        public async Task<IActionResult> CarFeatureChangeAvailableToTrue(int id)
        {
            var value = await _context.CarFeatures.FindAsync(id);
            if (value != null)
            {
                value.Available = true;
                await _context.SaveChangesAsync();
                return Ok("Durum aktif yapıldı.");
            }
            return NotFound();
        }

        [HttpGet("CarFeatureChangeAvailableToFalse")]
        public async Task<IActionResult> CarFeatureChangeAvailableToFalse(int id)
        {
            var value = await _context.CarFeatures.FindAsync(id);
            if (value != null)
            {
                value.Available = false;
                await _context.SaveChangesAsync();
                return Ok("Durum pasif yapıldı.");
            }
            return NotFound();
        }
    }
}
