using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CarBook.Application.Features.Mediator.Commands.ReservationCommands;
using CarBook.Application.Interfaces.ReservationInterfaces;

namespace CarBook.Application.Features.Mediator.Handlers.ReservationHandlers
{
    public class UpdateReservationStatusCommandHandler : IRequestHandler<UpdateReservationStatusCommand>
    {
        private readonly IReservationRepository _repository;

        public UpdateReservationStatusCommandHandler(IReservationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateReservationStatusCommand request, CancellationToken cancellationToken)
        {
            await _repository.ChangeStatusAsync(request.Id, request.Status);
            return Unit.Value;
        }
    }
}
