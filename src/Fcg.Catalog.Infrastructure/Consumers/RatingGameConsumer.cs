using Fcg.Catalog.Application.Features.Catalog.Commands.Players.RatingGame;
using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using MediatR;

namespace Fcg.Catalog.Infrastructure.Consumers
{
    public class RatingGameConsumer : IConsumer<IGameRatingIntegrationEvent>
    {
        private readonly ISender _sender;

        public RatingGameConsumer(ISender sender)
        {
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<IGameRatingIntegrationEvent> context)
        {
            var message = context.Message;
            var command = new RatingGameCommand(message.GameId, message.UserId, message.Rating, message.OccurredAt);
            await _sender.Send(command, context.CancellationToken);
        }
    }
}
