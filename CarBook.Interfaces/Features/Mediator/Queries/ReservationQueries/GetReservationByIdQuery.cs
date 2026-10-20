using MediatR;
using CarBook.Application.Features.Mediator.Results.ReservationResults;

namespace CarBook.Application.Features.Mediator.Queries.ReservationQueries
{
    public class GetReservationByIdQuery : IRequest<GetReservationByIdQueryResult>
    {
        public int Id { get; set; }
        public GetReservationByIdQuery(int id)
        {
            Id = id;
        }
    }
}
