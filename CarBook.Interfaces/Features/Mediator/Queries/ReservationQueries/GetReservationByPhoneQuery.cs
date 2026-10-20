using MediatR;
using System.Collections.Generic;
using CarBook.Application.Features.Mediator.Results.ReservationResults;

namespace CarBook.Application.Features.Mediator.Queries.ReservationQueries
{
    public class GetReservationByPhoneQuery : IRequest<List<GetReservationQueryResult>>
    {
        public string Phone { get; set; }
        public GetReservationByPhoneQuery(string phone)
        {
            Phone = phone;
        }
    }
}
