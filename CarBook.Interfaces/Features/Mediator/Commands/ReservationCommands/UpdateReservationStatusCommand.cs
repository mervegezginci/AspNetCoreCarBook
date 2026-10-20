using MediatR;

namespace CarBook.Application.Features.Mediator.Commands.ReservationCommands
{
    public class UpdateReservationStatusCommand : IRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public UpdateReservationStatusCommand(int id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}
