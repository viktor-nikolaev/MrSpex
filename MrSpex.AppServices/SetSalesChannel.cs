using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MrSpex.Domain;
using MrSpex.SharedKernel;

namespace MrSpex.AppServices
{
    public static class SetSalesChannel
    {
        public record Command
        (
            string SalesChannelId,
            IReadOnlyList<string> Locations
        ) : IRequest<Response>;

        public record Response(bool Acknowledged = true);

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.SalesChannelId).NotEmpty();
                RuleFor(x => x.Locations).NotEmpty();
                RuleForEach(x => x.Locations).NotEmpty();
            }
        }

        private class Handler : IRequestHandler<Command, Response>
        {
            private readonly ISalesChannelRepository _salesChannelRepository;
            private readonly IUnitOfWork _uow;

            public Handler(ISalesChannelRepository salesChannelRepository, IUnitOfWork uow)
            {
                _salesChannelRepository = salesChannelRepository;
                _uow = uow;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancel)
            {
                var existing = await _salesChannelRepository
                    .Get(SalesChannelSpecs.WithSalesChannelId(command.SalesChannelId), cancel);
                
                if (existing is not null)
                {
                    existing.ChangeLocations(command.Locations);
                    _salesChannelRepository.Update(existing);
                }
                else
                {
                    var salesChannel = new SalesChannel(command.SalesChannelId, command.Locations);
                    _salesChannelRepository.Add(salesChannel);
                }

                await _uow.Commit(cancel);

                return new Response();
            }
        }
    }
}