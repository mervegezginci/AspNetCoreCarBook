using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarBook.Application.Features.Mediator.Queries.ReservationQueries;
using CarBook.Application.Features.Mediator.Results.ReservationResults;
using CarBook.Application.Interfaces.ReservationInterfaces;

namespace CarBook.Application.Features.Mediator.Handlers.ReservationHandlers
{
    public class GetReservationByPhoneQueryHandler : IRequestHandler<GetReservationByPhoneQuery, List<GetReservationQueryResult>>
    {
        private readonly IReservationRepository _repository;

        public GetReservationByPhoneQueryHandler(IReservationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetReservationQueryResult>> Handle(GetReservationByPhoneQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetReservationsByPhoneAsync(request.Phone);
            return values.Select(x => new GetReservationQueryResult
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
            }).ToList();
        }
    }
}
