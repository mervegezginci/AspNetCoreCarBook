using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CarBook.Application.Features.Mediator.Queries.ReservationQueries;
using CarBook.Application.Features.Mediator.Results.ReservationResults;
using CarBook.Application.Interfaces.ReservationInterfaces;

namespace CarBook.Application.Features.Mediator.Handlers.ReservationHandlers
{
    public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, GetReservationByIdQueryResult?>
    {
        private readonly IReservationRepository _repository;

        public GetReservationByIdQueryHandler(IReservationRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetReservationByIdQueryResult?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            var x = await _repository.GetReservationWithDetailsByIdAsync(request.Id);
            if (x == null) return null;

            return new GetReservationByIdQueryResult
            {
                ReservationID = x.ReservationID,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                Phone = x.Phone,
                CarID = x.CarID,
                CarBrandAndModel = x.Car != null ? (x.Car.Brand != null ? x.Car.Brand.Name + " " + x.Car.Model : x.Car.Model) : "Belirtilmemiş",
                CarImageUrl = x.Car != null ? x.Car.CoverImageUrl : "",
                PickUpLocationID = x.PickUpLocationID,
                PickUpLocationName = x.PickUpLocation != null ? x.PickUpLocation.Name : "Belirtilmemiş",
                DropOffLocationID = x.DropOffLocationID,
                DropOffLocationName = x.DropOffLocation != null ? x.DropOffLocation.Name : "Belirtilmemiş",
                Age = x.Age,
                DriverLicenseYear = x.DriverLicenseYear,
                Description = x.Description,
                Status = x.Status
            };
        }
    }
}
