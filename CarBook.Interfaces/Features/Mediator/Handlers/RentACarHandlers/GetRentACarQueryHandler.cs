using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarBook.Application.Features.Mediator.Queries.RentACarQueries;
using CarBook.Application.Features.Mediator.Results.RentACarResults;
using CarBook.Application.Interfaces.RentACarInterfaces;

namespace CarBook.Application.Features.Mediator.Handlers.RentACarHandlers
{
    public class GetRentACarQueryHandler : IRequestHandler<GetRentACarQuery, List<GetRentACarQueryResult>>
    {
        private readonly IRentACarRepository _repository;
        public GetRentACarQueryHandler(IRentACarRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<GetRentACarQueryResult>> Handle(GetRentACarQuery request, CancellationToken cancellationToken)
        {
            var values = request.LocationID > 0
                ? await _repository.GetByFilterAsync(x => x.LocationID == request.LocationID && x.Available == true)
                : await _repository.GetByFilterAsync(x => x.Available == true);

            var results = values
                .GroupBy(x => x.CarID)
                .Select(g => g.First())
                .Select(y => new GetRentACarQueryResult
                {
                    CarId = y.CarID,
                    Brand = y.Car.Brand.Name,
                    Model = y.Car.Model,
                    CoverImageUrl = y.Car.CoverImageUrl
                }).ToList();

            return results;
        }
    }
}
