using MediatR;

namespace CarBook.Application.Features.Mediator.Commands.ReservationCommands
{
    public class RemoveReservationCommand : IRequest
    {
        public int Id { get; set; }
        public RemoveReservationCommand(int id)
        {
            Id = id;
        }
    }
}
