using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class PedidoJogo : EntityBase
    {
        public Guid PedidoId { get; private set; }
        public Guid JogoId { get; private set; }
        public string NomeJogo { get; private set; }
        public decimal ValorJogo { get; private set; }

        protected PedidoJogo()
        {
        }

        public PedidoJogo(Guid pedidoId, Guid jogoId, string nomeJogo, decimal valorJogo)
        {
            PedidoId = pedidoId;
            JogoId = jogoId;
            NomeJogo = nomeJogo;
            ValorJogo = valorJogo;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotEmpty(PedidoId, MensagensDominio.PedidoSemUsuario); // Using an existing message or default
            AssertionConcern.AssertArgumentNotEmpty(JogoId, MensagensDominio.JogoNaoEncontrado);
            AssertionConcern.AssertArgumentNotNull(NomeJogo, MensagensDominio.JogoNomeObrigatorio);

            if (ValorJogo < 0)
                throw new DomainException("O valor do item não pode ser negativo");
        }
    }
}
