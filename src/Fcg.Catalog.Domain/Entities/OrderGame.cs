using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class OrderGame : EntityBase
    {
        public Guid OrderId { get; private set; }
        public Guid GameId { get; private set; }
        public string GameName { get; private set; }
        public decimal GameAmount { get; private set; }

        protected OrderGame()
        {
        }

        public OrderGame(Guid orderId, Guid gameId, string gameName, decimal gameAmount)
        {
            this.OrderId = orderId;
            this.GameId = gameId;
            GameName = gameName;
            GameAmount = gameAmount;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotEmpty(OrderId, MensagensDominio.PedidoSemUsuario); // Using an existing message or default
            AssertionConcern.AssertArgumentNotEmpty(GameId, MensagensDominio.JogoNaoEncontrado);
            AssertionConcern.AssertArgumentNotNull(GameName, MensagensDominio.JogoNomeObrigatorio);

            if (GameAmount < 0)
                throw new DomainException("O Amount do item não pode ser negativo");
        }
    }
}
