using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CarBook.Application.Features.Mediator.Commands.ReservationCommands;
using CarBook.Application.Interfaces;
using CarBook.Domain.Entities;

namespace CarBook.Application.Features.Mediator.Handlers.ReservationHandlers
{
    public class RemoveReservationCommandHandler : IRequestHandler<RemoveReservationCommand>
    {
        private readonly IRepository<Reservation> _repository;

        public RemoveReservationCommandHandler(IRepository<Reservation> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveReservationCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value != null)
            {
                await _repository.RemoveAsync(value);
            }
            return Unit.Value;
        }
    }
}
