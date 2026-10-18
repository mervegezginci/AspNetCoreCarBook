using CarBook.Application.Features.Mediator.Queries.CarPricingQueries;
using CarBook.Domain.Entities;
using CarBook.Persistence.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarBook.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarPricingsController : ControllerBase
    {
        private readonly CarBookContext _context;
        private readonly IMediator _mediator;

        public CarPricingsController(CarBookContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarPricingListWithCar()
        {
            var values = await _context.CarPricings
                .Include(x => x.Car)
                .ThenInclude(x => x.Brand)
                .Include(x => x.Pricing)
                .Select(x => new
                {
                    CarPricingID = x.CarPricingID,
                    CarID = x.CarID,
                    BrandName = x.Car != null && x.Car.Brand != null ? x.Car.Brand.Name : "",
                    Model = x.Car != null ? x.Car.Model : "",
                    CoverImageUrl = x.Car != null ? x.Car.CoverImageUrl : "",
                    PricingID = x.PricingID,
                    PricingName = x.Pricing != null ? x.Pricing.Name : "",
                    Amount = x.Amount
                })
                .ToListAsync();

            return Ok(values);
        }

        [HttpGet("GetCarPricingWithTimePeriodList")]
        public async Task<IActionResult> GetCarPricingWithTimePeriodList()
        {
            try
            {
                var values = await _mediator.Send(new GetCarPricingWithTimePeriodQuery());
                if (values != null && values.Any())
                {
                    return Ok(values);
                }
            }
            catch { }

            var list = await _context.CarPricings
                .Include(x => x.Car)
                .ThenInclude(x => x.Brand)
                .Include(x => x.Pricing)
                .ToListAsync();

            var grouped = list
                .GroupBy(x => new
                {
                    x.CarID,
                    BrandName = x.Car != null && x.Car.Brand != null ? x.Car.Brand.Name : "",
                    Model = x.Car != null ? x.Car.Model : "",
                    CoverImageUrl = x.Car != null ? x.Car.CoverImageUrl : ""
                })
                .Select(g => new
                {
                    CarID = g.Key.CarID,
                    Model = g.Key.Model,
                    Brand = g.Key.BrandName,
                    CoverImageUrl = g.Key.CoverImageUrl,
                    DailyAmount = g.FirstOrDefault(x => x.Pricing != null && x.Pricing.Name.Contains("Günlük"))?.Amount ?? g.FirstOrDefault(x => x.PricingID == 1)?.Amount ?? 0,
                    WeeklyAmount = g.FirstOrDefault(x => x.Pricing != null && x.Pricing.Name.Contains("Haftalık"))?.Amount ?? g.FirstOrDefault(x => x.PricingID == 2)?.Amount ?? 0,
                    MonthlyAmount = g.FirstOrDefault(x => x.Pricing != null && x.Pricing.Name.Contains("Aylık"))?.Amount ?? g.FirstOrDefault(x => x.PricingID == 3)?.Amount ?? 0
                })
                .ToList();

            return Ok(grouped);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarPricing(int id)
        {
            var value = await _context.CarPricings
                .Include(x => x.Car)
                .ThenInclude(x => x.Brand)
                .Include(x => x.Pricing)
                .Where(x => x.CarPricingID == id)
                .Select(x => new
                {
                    CarPricingID = x.CarPricingID,
                    CarID = x.CarID,
                    BrandName = x.Car != null && x.Car.Brand != null ? x.Car.Brand.Name : "",
                    Model = x.Car != null ? x.Car.Model : "",
                    CoverImageUrl = x.Car != null ? x.Car.CoverImageUrl : "",
                    PricingID = x.PricingID,
                    PricingName = x.Pricing != null ? x.Pricing.Name : "",
                    Amount = x.Amount
                })
                .FirstOrDefaultAsync();

            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCarPricing(CarPricing carPricing)
        {
            _context.CarPricings.Add(carPricing);
            await _context.SaveChangesAsync();
            return Ok("Araç fiyatlandırması başarıyla eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCarPricing(CarPricing carPricing)
        {
            _context.CarPricings.Update(carPricing);
            await _context.SaveChangesAsync();
            return Ok("Araç fiyatlandırması başarıyla güncellendi");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCarPricing(int id)
        {
            var value = await _context.CarPricings.FindAsync(id);
            if (value != null)
            {
                _context.CarPricings.Remove(value);
                await _context.SaveChangesAsync();
            }
            return Ok("Araç fiyatlandırması silindi");
        }
    }
}
