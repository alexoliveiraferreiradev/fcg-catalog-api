using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Pedido : AggregateRoot
    {
        public Guid UsuarioId { get; private set; }
        public PedidoStatus Status { get; private set; }
        public Preco ValorTotal { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime DataAlteracao { get; private set; }
        private List<PedidoJogo> _jogos;
        public IReadOnlyCollection<PedidoJogo> Jogos => _jogos;

        protected Pedido()
        {
        }

        public Pedido(Guid usuarioId)
        {
            UsuarioId = usuarioId;
            _jogos = new List<PedidoJogo>();
            Status = PedidoStatus.Rascunho;
            DataCadastro = DateTime.UtcNow;
            DataAlteracao = DataCadastro;
            ValidarEntidade();
        }

        protected override void ValidarEntidade()
        {
            if (UsuarioId == Guid.Empty) throw new DomainException(MensagensDominio.PedidoSemUsuario);
        }

        public void AdicionarItem(Guid jogoId, string nomeJogo, decimal valorJogo)
        {
            if (Status != PedidoStatus.Rascunho) throw new DomainException(MensagensDominio.PedidoJogoNaoRascunhos);
            if (jogoId == Guid.Empty) throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            if (_jogos.Any(j => j.JogoId == jogoId)) throw new DomainException(MensagensDominio.PedidoJogoJaAdicionado);

            _jogos.Add(new PedidoJogo(Id, jogoId, nomeJogo, valorJogo));
        }

        public void FinalizarPedido()
        {
            if (Status != PedidoStatus.Rascunho) throw new DomainException(MensagensDominio.PedidoNaoRascunhos);
            if (!_jogos.Any()) throw new DomainException(MensagensDominio.PedidoSemJogos);
            Status = PedidoStatus.Finalizado;
            CalcularValorTotal();
            DataAlteracao = DateTime.UtcNow;
        }

        private void CalcularValorTotal()
        {
            ValorTotal = new Preco(_jogos.Sum(j => j.ValorJogo));
        }
    }
}
