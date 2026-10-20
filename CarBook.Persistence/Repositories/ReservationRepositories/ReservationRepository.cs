using Microsoft.EntityFrameworkCore;
using CarBook.Application.Interfaces.ReservationInterfaces;
using CarBook.Domain.Entities;
using CarBook.Persistence.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarBook.Persistence.Repositories.ReservationRepositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly CarBookContext _context;

        public ReservationRepository(CarBookContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetReservationsWithDetailsAsync()
        {
            return await _context.Reservations
                .Include(x => x.Car)
                    .ThenInclude(c => c.Brand)
                .Include(x => x.PickUpLocation)
                .Include(x => x.DropOffLocation)
                .OrderByDescending(x => x.ReservationID)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByPhoneAsync(string phone)
        {
            var cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            return await _context.Reservations
                .Include(x => x.Car)
                    .ThenInclude(c => c.Brand)
                .Include(x => x.PickUpLocation)
                .Include(x => x.DropOffLocation)
                .Where(x => x.Phone.Contains(cleanPhone) || x.Phone.Contains(phone))
                .OrderByDescending(x => x.ReservationID)
                .ToListAsync();
        }

        public async Task<Reservation?> GetReservationWithDetailsByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(x => x.Car)
                    .ThenInclude(c => c.Brand)
                .Include(x => x.PickUpLocation)
                .Include(x => x.DropOffLocation)
                .FirstOrDefaultAsync(x => x.ReservationID == id);
        }

        public async Task ChangeStatusAsync(int id, string status)
        {
            var res = await _context.Reservations.FindAsync(id);
            if (res != null)
            {
                res.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
