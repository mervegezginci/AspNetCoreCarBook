using MediatR;
using System.Collections.Generic;
using CarBook.Application.Features.Mediator.Results.ReservationResults;

namespace CarBook.Application.Features.Mediator.Queries.ReservationQueries
{
    public class GetReservationQuery : IRequest<List<GetReservationQueryResult>>
    {
    }
}
