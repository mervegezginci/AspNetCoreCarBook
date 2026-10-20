using CarBook.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarBook.Application.Interfaces.ReservationInterfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetReservationsWithDetailsAsync();
        Task<List<Reservation>> GetReservationsByPhoneAsync(string phone);
        Task<Reservation?> GetReservationWithDetailsByIdAsync(int id);
        Task ChangeStatusAsync(int id, string status);
    }
}
